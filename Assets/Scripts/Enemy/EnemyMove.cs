using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove
{
    public bool isStart;

    private Transform mTransform, mChildTransform;

    private int mMoveNumber = 4;//移动次数

    private float mTimeOfOnceMove = 0;//一次移动需要的时间

    private float mtime = 0;//计时器

    private Vector3 mStartPos = Vector3.zero, mEndPos = Vector3.zero, mDistance = Vector3.zero;

    private float rotateSpeed = 10;
    private bool mIsFront = true;

    public EnemyMove(Transform transform, Vector3 startPos, Vector3 endPos, float attackTime)
    {
        mTimeOfOnceMove = attackTime / mMoveNumber;
        rotateSpeed = rotateSpeed / attackTime;
        rotateSpeed = rotateSpeed / GameController.instance.UNIT_TIME;
        mIsFront = true;
        mtime = 0;
        mTransform = transform;
        mChildTransform = transform.GetChild(0).transform;
        mStartPos = startPos;
        mEndPos = endPos;
        if (mEndPos != Vector3.zero)
        {
            mDistance = mEndPos - mStartPos;
        }
        else
        {
            mDistance = Vector3.zero;
        }
        isStart = false;
    }

    public void OnUpdate()
    {
        if (!isStart)
            return;
        if (mTransform != null)
        {
            mChildTransform.Rotate(Vector3.forward * rotateSpeed, Space.Self);
        }
        if (mTransform != null && mDistance != Vector3.zero)
        {
            if (mMoveNumber > 0)
            {
                mtime += Time.deltaTime;
                if (mIsFront)
                {
                    mTransform.localPosition = mStartPos + (mtime / mTimeOfOnceMove) * mDistance;

                }
                else
                {
                    mTransform.localPosition = mEndPos + (mtime / mTimeOfOnceMove) * -mDistance;
                }
                if (mtime >= mTimeOfOnceMove)
                {
                    mMoveNumber--;
                    mtime = 0;
                    mIsFront = !mIsFront;
                }
            }
        }
    }
}
