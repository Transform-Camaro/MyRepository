using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : BaseState
{
    private StateMachine mStateMachine;
    private Player mPlayer;
    private Transform mPlayerTransform;

    public Vector3 mTargetPos;
    private Vector3 mStartPos;
    private Vector3 mVelocity = Vector3.zero;


    private Animator mAnimator;
    private Animation mAnimation;

    //private float mUpdateNumber;

    private float mDurationTime;
    // private float mNormalTime;
    private float CurrentTime = 0;

    public MoveState(Player player, Vector3 targetPos)
    {
        mTargetPos = targetPos;
        mPlayer = player;
        mPlayerTransform = player.myTransform;
        mStateMachine = player.mStateMachine;
        if (mPlayer.isAnimator)
        {
            mAnimator = player.mAnimator;
        }
        else
        {
            mAnimation = player.mAnimation;
        }

        float mNormalTime = GameController.instance.gameConfig.playerInfo.timeOfOneGrid;
        mDurationTime = mNormalTime * GameController.instance.UNIT_TIME;
        //  mUpdateNumber = (int)(1 / Time.deltaTime) * mDuration;

        // Debug.Log(mUpdateNumber);

        //mVelocity = (mTargetPos - mPlayerTransform.localPosition) / mUpdateNumber;//计算速率

        mStartPos = mPlayerTransform.localPosition;
        mVelocity = mTargetPos - mStartPos;
        CurrentTime = 0;
    }

    public override bool OnBlock()
    {
        return false;
    }

    public override void OnEnter()
    {
        //播放动画
        if (mVelocity.x != 0)
        {
            if (mPlayer.isAnimator)
            {
                mAnimator.SetFloat(PlayerAnimatorInfo.posx, mVelocity.x);

                mAnimator.speed = ((float)PlayerAnimatorInfo.Jump_Right_Length / 30) / mDurationTime;

                mAnimator.SetFloat(PlayerAnimatorInfo.facex, mVelocity.x);
            }
            else
            {
                mPlayer.faceX = mVelocity.x;
                if (mVelocity.x > 0)
                {
                    AnimationState anims = mAnimation[PlayerAnimatorInfo.Jump_Right];
                    anims.speed = ((float)PlayerAnimatorInfo.Jump_Right_Length / 30) / mDurationTime;
                    mAnimation.Play(PlayerAnimatorInfo.Jump_Right);
                }
                else
                {
                    AnimationState anims = mAnimation[PlayerAnimatorInfo.Jump_Left];
                    anims.speed = ((float)PlayerAnimatorInfo.Jump_Left_Length / 30) / mDurationTime;
                    mAnimation.Play(PlayerAnimatorInfo.Jump_Left);
                }

            }
        }
        else
        {
            if (mPlayer.isAnimator)
            {
                mAnimator.SetFloat(PlayerAnimatorInfo.posy, mVelocity.y);

                mAnimator.speed = ((float)PlayerAnimatorInfo.Jump_Up_Length / 30) / mDurationTime;
            }
            else
            {
                if (mVelocity.y > 0)
                {
                    AnimationState anims = mAnimation[PlayerAnimatorInfo.Jump_Up];
                    anims.speed = ((float)PlayerAnimatorInfo.Jump_Up_Length / 30) / mDurationTime;
                    mAnimation.Play(PlayerAnimatorInfo.Jump_Up);
                }
                else
                {
                    AnimationState anims = mAnimation[PlayerAnimatorInfo.Jump_Down];
                    anims.speed = ((float)PlayerAnimatorInfo.Jump_Down_Length / 30) / mDurationTime;
                    mAnimation.Play(PlayerAnimatorInfo.Jump_Down);
                }
            }

        }
        mPlayer.PlayBackPackEffect();
    }


    public override void OnExecute()
    {

        //mPlayerTransform.localPosition += mVelocity;

        //mUpdateNumber--;

        //if (mUpdateNumber <= 0)
        //{
        //    mPlayerTransform.localPosition = mTargetPos;
        //    OnExit();
        //}
        CurrentTime += Time.deltaTime;

        mPlayerTransform.localPosition = mStartPos + (CurrentTime / mDurationTime) * mVelocity;

        if (CurrentTime >= mDurationTime)
        {
            mPlayerTransform.localPosition = mTargetPos;
            OnExit();
        }
    }

    public override void OnExit()
    {
        if (mPlayer.isAnimator)
        {
            mAnimator.SetFloat(PlayerAnimatorInfo.posx, 0);
            mAnimator.SetFloat(PlayerAnimatorInfo.posy, 0);
            mAnimator.speed = 1;
        }
        else
        {

        }
        mStateMachine.RemoveCurrentState(this);
        mPlayer.OnMoveFinish();
    }
}
