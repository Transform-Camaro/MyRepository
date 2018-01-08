using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileAttackState
{
    private Tile mTile;

    public float attackTime;
    public int damage;
    public float frozenTime;
    public EnemyType enemtType;

    public TileAttackState()
    {
    }
    public void OnEnter(Tile tile)
    {
        mTile = tile;
    }
    public void OnUpdate()
    {

        if (attackTime > 0)
        {
            attackTime -= Time.deltaTime;
            if (attackTime <= 0)
            {
                OnExit();
            }
        }
    }

    public void OnExit()//通知退出
    {
        mTile.OnAttTimeEnd(this);
    }
}
