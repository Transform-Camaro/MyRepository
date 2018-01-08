using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseState
{
    public Player mPlayer;

    private StateMachine mStateMachine;

    //  private Animator mAnimator;

    public IdleState(Player player)
    {
        mPlayer = player;
        mStateMachine = player.mStateMachine;
    }

    public override bool OnBlock()
    {
        return true;
    }

    public override void OnEnter()
    {
        //播放动画
    }

    public override void OnExecute()
    {

    }

    public override void OnExit()
    {
        mStateMachine.RemoveCurrentState(this);
    }
}
