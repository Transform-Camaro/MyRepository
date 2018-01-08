using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public Transform myTransform;

    private int mHealth = 0;
    private int Health
    {
        get
        {
            return mHealth;
        }
        set
        {
            mHealth = value;
            GameManager.instance.mThreeDimensionsBlood.SetPlayerBloodModle(value);
        }
    }

    //初始坐标
    private int mPosX = 1, mPosY = 1;

    private float tileOffset = -0.1f;
    private TileManager mTileManager;
    private Tile mCurrentTile;//当前的地块
    private Tile mNewTile;//将要去的地块
    private TileManager GetTileManager
    {
        get
        {
            if (mTileManager == null)
            {
                mTileManager = GameManager.instance.mTileManager;
            }

            return mTileManager;
        }
    }

    private Queue<Vector2> mMoveQueue;//行动逻辑

    private PlayerBackPackEffect[] mPlayerBackPackEffects;
    private Dictionary<EnemyType, ParticleSystem[]> mEffectOfEnemy;

    private Transform mEffectParent;
    private float mEffectScale = 5;

    public Animator mAnimator;
    public Animation mAnimation;
    public bool isAnimator = true;
    public float faceX = 1;
    public StateMachine mStateMachine;
    private HurtState mHurtState;
    private MoveState mMoveState;
    private IdleState mIdleState;
    private DieState mDieState;
    public enum PlayerStateType
    {
        idle = 0,
        jump = 1,
        hurt = 2,
        die = 3
    }
    private Transform FindChildByName(Transform mTransform, string name)
    {
        if (mTransform.name == name)
        {
            return mTransform;
        }
        if (mTransform.childCount < 1)
        {
            return null;
        }
        Transform returnTransform = null;
        for (int i = 0; i < mTransform.childCount; i++)
        {
            Transform temp = mTransform.GetChild(i).transform;

            returnTransform = FindChildByName(temp, name);
            if (returnTransform != null)
            {
                break;
            }
        }
        return returnTransform;
    }

    public void SetInfo(Transform transform, PlayerBackPackEffect[] effects,
        Dictionary<EnemyType, GameObject> effectOfTheEnemy)
    {
        myTransform = transform;
        mPlayerBackPackEffects = effects;

        if (mEffectOfEnemy == null)
        {
            mEffectOfEnemy = new Dictionary<EnemyType, ParticleSystem[]>();
        }

        List<EnemyInfo> infos = GameController.instance.enemyConfig.mEnemyInfo;
        foreach (KeyValuePair<EnemyType, GameObject> keyvalue in effectOfTheEnemy)
        {
            GameObject effect = keyvalue.Value;
            ParticleSystem[] ParticleSystems = null;
            if (effect != null)
            {
                effect.transform.SetParent(myTransform);
                effect.transform.localPosition = Vector3.zero;
                effect.transform.localRotation = Quaternion.Euler(0, 0, 0);
                effect.transform.localScale = Vector3.one;
                float duration = 0;
                for (int k = 0; k < infos.Count; k++)
                {
                    if (infos[k].type == keyvalue.Key)
                    {
                        duration = infos[k].frozenTime;
                        break;
                    }
                }
                duration = duration * GameController.instance.UNIT_TIME;
                ParticleSystems = effect.GetComponentsInChildren<ParticleSystem>();
                ParticleSystem tempPs = null;

                for (int i = 0; i < ParticleSystems.Length; i++)
                {
                    tempPs = ParticleSystems[i];
                    ParticleSystem.MainModule main = tempPs.main;
                    main.startLifetime = main.duration = duration;
                }
                mEffectOfEnemy.Add(keyvalue.Key, ParticleSystems);
            }
        }
        infos = null;
        if (mEffectParent == null)
        {
            mEffectParent = FindChildByName(myTransform, "Bone001");
        }

        for (int i = 0; i < mPlayerBackPackEffects.Length; i++)
        {
            mPlayerBackPackEffects[i].mTransform.SetParent(mEffectParent.transform);
            mPlayerBackPackEffects[i].mTransform.localRotation = Quaternion.Euler(180, 90, 0);
            mPlayerBackPackEffects[i].mTransform.localScale = Vector3.one * mEffectScale;
            mPlayerBackPackEffects[i].mTransform.localPosition = new Vector3(-9f, 0, (0.5f - i) * 4);
        }
        if (isAnimator)
        {
            mAnimator = myTransform.GetComponent<Animator>();
        }
        else
        {
            mAnimation = myTransform.GetComponent<Animation>();
            mAnimation.Play(PlayerAnimatorInfo.Right_Idle);
        }
    }
    public void OnAwake()
    {
        if (mMoveQueue == null)
            mMoveQueue = new Queue<Vector2>();
    }

    public void OnUpdate()
    {
        if (mStateMachine != null)
            mStateMachine.OnUpdate();
    }
    void OnDestroy()
    {
        if (mMoveQueue != null)
            mMoveQueue = null;
    }

    private void ResetStateMachine()//重置状态机
    {
        if (mStateMachine == null)
            mStateMachine = new StateMachine();

        mIdleState = new IdleState(this);
        mStateMachine.ChangeState(mIdleState);
        mDieState = null;
    }

    public void ResetPlayerInfo()//重置角色属性
    {
        ResetStateMachine();

        mPosX = mPosY = 1;
        mCurrentTile = GetTileManager.GetTile(mPosX, mPosY);
        if (mCurrentTile.onPlayerStay == null)
        {
            mCurrentTile.onPlayerStay += OnTakeDamage;
        }

        myTransform.localPosition = mCurrentTile.V3pos + Vector3.up * tileOffset;
        Health = GameController.instance.gameConfig.playerInfo.playerHealth;
        if (mNewTile != null)
        {
            mNewTile.onPlayerStay -= OnTakeDamage;
            mNewTile = null;
        }
        mMoveQueue.Clear();
    }

    //移动前
    public void OnMoveBegin(Vector2 v2)
    {
        if (mStateMachine.GetCurrentState() != mHurtState)
        {
            mMoveQueue.Enqueue(v2);
        }
        else
        {
            // Debug.Log("冰冻期间，不添加指令");
        }
        if (mStateMachine.GetCurrentStateBlock())//状态是否可以被打断
        {
            OnMove();
        }
    }
    //移动ing
    private void OnMove()
    {
        if (mMoveQueue.Count > 0)
        {
            Vector2 vector2 = mMoveQueue.Dequeue();

            //方向算坐标
            int x = mPosX;
            int y = mPosY;
            x += (int)vector2.x;
            y += (int)vector2.y;

            mNewTile = GetTileManager.GetTile(x, y);
            if (mNewTile == null)
            {
                OnMove();
                return;
            }
            mPosX = x;
            mPosY = y;
            //移动过去,执行动画
            Vector3 mTargetPos = GetTileManager.GetTile(mPosX, mPosY).V3pos + Vector3.up * tileOffset;
            if (mCurrentTile != null)
            {
                mCurrentTile.onPlayerStay -= OnTakeDamage;
            }
            if (isAnimator)
            {
                // mAnimator.SetInteger(PlayerAnimatorInfo.state_type, (int)PlayerStateType.jump);
                mAnimator.SetTrigger(PlayerAnimatorInfo.state_Jump);
            }
            mMoveState = new MoveState(this, mTargetPos);
            mStateMachine.ChangeState(mMoveState);

        }
        else
        {
            mIdleState = new IdleState(this);
            mStateMachine.ChangeState(mIdleState);
        }
    }

    public void PlayBackPackEffect()
    {
        for (int i = 0; i < mPlayerBackPackEffects.Length; i++)
        {
            mPlayerBackPackEffects[i].PlayEffect();
        }
    }

    public void PlayEffectOnHurt(EnemyType type)
    {
        if (mEffectOfEnemy.ContainsKey(type))
        {
            ParticleSystem tempPs;
            for (int i = 0; i < mEffectOfEnemy[type].Length; i++)
            {
                tempPs = mEffectOfEnemy[type][i];
                tempPs.Play();
            }
        }
    }
    public void StopEffectOnHurt(EnemyType type)
    {
        if (mEffectOfEnemy.ContainsKey(type))
        {
            ParticleSystem tempPs;
            for (int i = 0; i < mEffectOfEnemy[type].Length; i++)
            {
                tempPs = mEffectOfEnemy[type][i];
                tempPs.Stop();
            }
        }
    }

    public void OnMoveFinish()//移动后
    {
        if (isAnimator)
        {
            // mAnimator.SetInteger(PlayerAnimatorInfo.state_type, (int)PlayerStateType.idle);
        }
        else
        {
            if (faceX < 0)
            {
                mAnimation.Play(PlayerAnimatorInfo.Left_Idle);
            }
            else
            {
                mAnimation.Play(PlayerAnimatorInfo.Right_Idle);
            }
        }

        mMoveState = null;
        mCurrentTile = mNewTile;
        if (mCurrentTile != null)
        {
            mCurrentTile.onPlayerStay += OnTakeDamage;
        }

        TileAttackState state = mCurrentTile.GetMaxDamageInAttackState();

        if (state != null && state.damage > 0)
        {
            //受到伤害
            OnTakeDamage(state);
        }
        else
        {
            OnMove();
        }
    }
    public void OnTakeDamageFinish()
    {
        if (isAnimator)
        {
            //  mAnimator.SetInteger(PlayerAnimatorInfo.state_type, (int)PlayerStateType.idle);
        }
        else
        {
            if (faceX < 0)
            {
                mAnimation.Play(PlayerAnimatorInfo.Left_Idle);
            }
            else
            {
                mAnimation.Play(PlayerAnimatorInfo.Right_Idle);
            }
        }
        mHurtState = null;
        OnMove();
    }

    //受到伤害
    public void OnTakeDamage(TileAttackState state)
    {
        if (mStateMachine.GetCurrentStateBlock() && state.damage > 0)
        {
            if (!GameController.instance.gameConfig.playerInfo.isInvincible)
            {
                Health = Health - state.damage;
            }
            else
            {
                //Debug.Log(state.damage);
            }

            if (Health <= 0)
            {
                OnDie();
            }
            else
            {
                if (isAnimator)
                {
                    // mAnimator.SetInteger(PlayerAnimatorInfo.state_type, (int)PlayerStateType.hurt);
                    mAnimator.SetTrigger(PlayerAnimatorInfo.state_Hurt);
                }

                mHurtState = new HurtState(this, state.enemtType, state.frozenTime);
                mStateMachine.ChangeState(mHurtState);
            }
        }
    }
    //死亡
    private void OnDie()
    {

        mDieState = new DieState(this);

        mStateMachine.ChangeState(mDieState);

        GameManager.instance.OnPlayerDie();

        if (mCurrentTile != null)
        {
            mCurrentTile.onPlayerStay -= OnTakeDamage;
            mCurrentTile = null;
        }
    }


}
