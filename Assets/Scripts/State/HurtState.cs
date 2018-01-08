using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtState : BaseState
{

    private Player mPlayer;

    private float mFrozenTime;

    private StateMachine mStateMachine;

    private Animator mAnimator;
    private Animation mAnimation;

    private EnemyType mtype;

    float[] mHurtLength = new float[]
    {
        PlayerAnimatorInfo.Hurt_Lightning_Length,
        PlayerAnimatorInfo.Hurt_Wind_Length,
        PlayerAnimatorInfo.Hurt_Wind_Length,
        PlayerAnimatorInfo.Hurt_Rock_Length,
        PlayerAnimatorInfo.Hurt_Fire_Length
    };
    string[] mHurtString = new string[]
    {
        PlayerAnimatorInfo.Hurt_Lightning,
        PlayerAnimatorInfo.Hurt_Wind,
        PlayerAnimatorInfo.Hurt_Wind,
        PlayerAnimatorInfo.Hurt_Rock,
        PlayerAnimatorInfo.Hurt_Fire,
    };

    public HurtState(Player player, EnemyType type, float frozenTime)
    {
        mtype = type;
        mPlayer = player;
        mFrozenTime = frozenTime;
        mStateMachine = mPlayer.mStateMachine;
        if (player.isAnimator)
        {
            mAnimator = mPlayer.mAnimator;
            mAnimator.SetInteger(PlayerAnimatorInfo.hurt_type, (int)type + 1);
            mAnimator.speed = (mHurtLength[(int)type] / 30) / mFrozenTime;
        }
        else
        {
            mAnimation = mPlayer.mAnimation;
            AnimationState anims = null;
            mAnimation.Play(mHurtString[(int)type]);
            anims = mAnimation[mHurtString[(int)type]];
            anims.speed = (mHurtLength[(int)type] / 30) / mFrozenTime;
        }
        mPlayer.PlayEffectOnHurt(mtype);
    }
    public override bool OnBlock()
    {
        return false;
    }

    public override void OnEnter()
    {
    }
    public override void OnExecute()
    {
        mFrozenTime -= Time.deltaTime;

        if (mFrozenTime <= 0)
        {
            OnExit();
        }
    }

    public override void OnExit()
    {
        if (mPlayer.isAnimator)
        {
            mAnimator.SetInteger(PlayerAnimatorInfo.hurt_type, 0);
            mAnimator.speed = 1;
        }
        else
        {

        }
        mPlayer.StopEffectOnHurt(mtype);
        mStateMachine.RemoveCurrentState(this);
        mPlayer.OnTakeDamageFinish();
    }
}
