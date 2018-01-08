using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class EnemyConfig : ScriptableObject
{
    //每轮生成怪物数量
    public int enemyCount;

    [SerializeField, Header("EnemyType Of Time")]
    public List<CreateInfo> mCreateInfoList;

    [SerializeField, Header("Enemy Info")]
    public List<EnemyInfo> mEnemyInfo;

}

[System.Serializable]
public class EnemyInfo
{
    [SerializeField, Header("Enemy Type ")]
    public EnemyType type;

    [SerializeField, Header("Time of Hint ")]
    public float hintTime;

    [SerializeField, Header("Time of Attack ")]
    public float attackTime;

    [SerializeField, Header("Time of Frozen ")]
    public float frozenTime;

    [SerializeField, Header("Damage ")]
    public int attack;

    [SerializeField, Header("Color of Hint ")]
    public TileColor hintTileColor;

    [SerializeField, Header("Range of Attack ")]
    public List<EnemyRangeEnum> mRange;

    [SerializeField, Header("Probability Of The Enemy")]
    public int Probability;
}

[System.Serializable]
public class CreateInfo
{
    [SerializeField, Header("Interval Time")]
    public float intervalTime;

    [SerializeField, Header("EnemyTypes Of EveryTime")]
    public List<EnemyType> EnemyTypes;

    [SerializeField, Header("Time Point")]
    public int TimePoint;
}

public enum EnemyRangeEnum
{
    self,
    left,
    right,
    up,
    down
}
public enum TileColor
{
    no,
    purple,
    blue,
    red,
    yellow
}
public enum EnemyType
{
    Lightning,
    WindLeft,
    WindRight,
    Rock,
    Fire
}



