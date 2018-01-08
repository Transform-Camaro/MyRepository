using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyAppearInfo
{
    public EnemyType enemyType;
    public List<Tile> tileList;
    public Tile tile;//站的位置
}
public class Enemy : ILoadBundle
{
    protected EnemySpawner mEnemySpawner;

    protected EnemyType mEnemyType = 0;//怪物类型

    protected Vector3 mPos;//怪物的坐标

    private List<Tile> mRangeTile;//攻击范围

    private Tile mTile;//怪的位置

    protected float mHintTime, mAttackTime, mFrozenTime = 0;//提示时间，攻击时间，冰冻时间

    protected int mDamage = 0;//伤害

    protected bool isHintState, isAttackState = false;//是否开始进行状态

    protected TileColor mhintColor = TileColor.no;

    public GameObject root;

    private EnemyMove mEnemyMove;

    public GameObject mEnemyGo
    {
        get;
        set;
    }

    private ParticleSystem[] mParticleSystem;
    private ParticleSystem[] GetParticleSystems
    {
        get
        {
            if (mParticleSystem == null)
            {
                mParticleSystem = mEnemyGo.transform.GetComponentsInChildren<ParticleSystem>();
            }
            return mParticleSystem;
        }
    }
    private void GetEnemyInfoByConfig(EnemyType Type)//从配置表里读取怪物信息
    {
        EnemyInfo info = null;
        List<EnemyInfo> infos = GameController.instance.enemyConfig.mEnemyInfo;
        for (int i = 0; i < infos.Count; i++)
        {
            if (infos[i].type == Type)
            {
                info = infos[i];
                break;
            }
        }
        float unit_time = GameController.instance.UNIT_TIME;
        mHintTime = info.hintTime * unit_time;//提示时间
        mAttackTime = info.attackTime * unit_time;//攻击时间
        mFrozenTime = info.frozenTime * unit_time;//冰冻时间

        mhintColor = info.hintTileColor;
        mDamage = info.attack;//伤害

    }
    public void SetEnemyTrans(Transform parent, GameObject go)
    {
        if (go == null)
        {
            return;
        }
        mEnemyGo = go;
        mEnemyGo.transform.SetParent(parent);
        mEnemyGo.transform.localPosition = mPos;
        mEnemyGo.transform.localScale = Vector3.one;
        mEnemyGo.transform.localRotation = Quaternion.Euler(0, 0, 0);
        mEnemyGo.SetActive(false);

        if (mEnemyType == EnemyType.Fire || mEnemyType == EnemyType.Rock)
        {
            for (int i = 0; i < mEnemyGo.transform.childCount; i++)
            {
                Transform transform = mEnemyGo.transform.GetChild(i);
                if (i < mRangeTile.Count)
                {
                    transform.gameObject.SetActive(true);
                    transform.localPosition = mRangeTile[i].V3pos - mPos;
                    transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
                else
                {
                    transform.gameObject.SetActive(false);
                }
            }
        }

        if (mEnemyType == EnemyType.WindLeft || mEnemyType == EnemyType.WindRight)
        {
            Vector3 endPos = Vector3.zero;

            for (int i = 0; i < mRangeTile.Count; i++)
            {
                if (mRangeTile[i] != mTile)
                {
                    endPos = mRangeTile[i].V3pos;
                }
            }
            mEnemyMove = new EnemyMove(mEnemyGo.transform, mTile.V3pos, endPos, mAttackTime);
        }
    }
    public Enemy(EnemyAppearInfo info, EnemySpawner enemySpawner)
    {
        mEnemySpawner = enemySpawner;
        mEnemyType = info.enemyType;//类型
        mRangeTile = info.tileList;//攻击范围
        mTile = info.tile;
        mPos = mTile.V3pos;//位置
        GetEnemyInfoByConfig(mEnemyType);
    }

    public void LoadBundle()
    {
        root = new GameObject();

        string url = AssetsPaths.GameEnemyPath + ((int)mEnemyType);
        //  Debug.Log(url);
        LoadBundle lb = root.AddComponent<LoadBundle>();
        lb.path = url;
        lb.load = this;
        lb.DoLoadBundle();
    }

    public void LoadBundleBack(GameObject go)
    {
        mEnemyGo = GameObject.Instantiate(go);

        if (mEnemyGo == null)
            return;
        if (root == null)
            return;

        SetEnemyTrans(mEnemySpawner.mTransform, mEnemyGo);
        Appear();
        GameObject.Destroy(root);
    }

    public virtual void Appear()//类型，X,Y
    {
        //提示怪物出现
        //生成怪物
        //播放出生动画

        //  GetEnemyInfoByConfig(mEnemyType);

        mEnemySpawner.AddEnemyOnAppear(this);

        #region --格子提示
        //  for (int i = 0; i < mRangeTile.Count; i++)
        {
            TileHintState state;
            state = new TileHintState();
            state.hintTime = mHintTime;
            state.hintColor = mhintColor;
            state.enemyType = mEnemyType;
            state.damage = mDamage;
            //state.frozenTime = mFrozenTime;
            //state.effectType = mEffectType;
            //state.attackTime = mAttackTime;
            //  mRangeTile[i].SetHintState(state);//更改提示状态
            mTile.SetHintState(state);//更改提示状态
            state = null;
        }

        for (int i = 0; i < mRangeTile.Count; i++)
        {
            mRangeTile[i].SetOccupy(false);//取消临时占据
        }
        #endregion

        isHintState = true;//开始提示
    }

    protected virtual void Attack()//攻击
    {
        //播放动画

        TileAttackState attState = null;
        for (int i = 0; i < mRangeTile.Count; i++)
        {
            attState = new TileAttackState();
            attState.damage = mDamage;
            attState.attackTime = mAttackTime;
            attState.frozenTime = mFrozenTime;
            attState.enemtType = mEnemyType;

            mRangeTile[i].SetAttackState(attState);
            attState = null;
        }
        isHintState = false;
        isAttackState = true;//开始攻击

        //调整特效的持续时间
        if (GetParticleSystems != null)
        {
            //  Debug.Log(GetParticleSystems.Length);
            for (int i = 0; i < GetParticleSystems.Length; i++)
            {
                ParticleSystem.MainModule main = GetParticleSystems[i].main;
                main.duration = mAttackTime;
                GetParticleSystems[i].Play();
            }
        }
        mEnemyGo.SetActive(true);//打开
        if (mEnemyMove != null)
        {
            mEnemyMove.isStart = true;
        }
    }

    public void OnUpdate()
    {
        if (mEnemyMove != null)
        {
            mEnemyMove.OnUpdate();
        }
        if (isHintState)
        {
            if (mHintTime > 0)//提示时间
            {
                mHintTime -= Time.deltaTime;
                if (mHintTime <= 0)//提示时间到了
                {
                    //改攻击状态
                    Attack();
                }
            }
        }
        if (isAttackState)
        {
            if (mAttackTime > 0)//攻击时间
            {
                mAttackTime -= Time.deltaTime;
                if (mAttackTime <= 0)//攻击时间到了
                {
                    Disappear();//消失
                }
            }
        }
    }

    public virtual void Disappear()//敌人消失
    {
        if (mRangeTile != null)
        {
            mRangeTile.Clear();
            mRangeTile = null;
        }
        mEnemySpawner.RecycleEnemy(mEnemyType, this);
        mEnemyType = 0;
        mDamage = 0;
        mHintTime =
            mAttackTime =
            mFrozenTime = 0;
        isHintState = false;
        isAttackState = false;
        mPos = Vector3.zero;
        mhintColor = TileColor.no;
        mEnemySpawner = null;
        if (mEnemyMove != null)
            mEnemyMove = null;
    }


}
