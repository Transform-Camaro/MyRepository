using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieState : BaseState
{
    private Player mplayer;
    private StateMachine mStateMachine;

    private Animation mAnimation;

    private Animator mAnimator;

    public DieState(Player player)
    {
        mplayer = player;
        mStateMachine = player.mStateMachine;
        if (player.isAnimator)
        {
            mAnimator = player.mAnimator;
        }
        else
        {
            mAnimation = player.mAnimation;
        }
    }

    public override bool OnBlock()
    {
        return true;
    }

    public override void OnEnter()
    {
        //执行动画
        if (mplayer.isAnimator)
        {
            //  mAnimator.SetInteger(PlayerAnimatorInfo.state_type, (int)Player.PlayerStateType.die);

            mAnimator.SetTrigger(PlayerAnimatorInfo.state_Die);
        }
        else
        {
            mAnimation.Play(PlayerAnimatorInfo.Die);
        }
    }

    public override void OnExecute()
    {
    }

    public override void OnExit()
    {
        mStateMachine.RemoveCurrentState(this);
    }
}
