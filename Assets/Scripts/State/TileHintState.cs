using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHintState
{
    private Tile mTile;

    public float hintTime;
    public EnemyType enemyType;
    public TileColor hintColor;
    public int damage;

    //public float attackTime;
    //public float frozenTime;
    //public int effectType;

    public TileHintState()
    {
    }


    public void OnEnter(Tile tile)
    {
        mTile = tile;
    }

    public void OnUpdate()
    {
        if (hintTime > 0)
        {
            hintTime -= Time.deltaTime;
            if (hintTime <= 0)
            {
                OnExit();
            }
        }
    }

    public void OnExit()//通知换颜色，去攻击状态
    {
        mTile.OnHintTimeEnd(this);
    }

}
