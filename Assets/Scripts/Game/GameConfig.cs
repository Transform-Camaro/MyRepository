using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class GameConfig : ScriptableObject
{
    [SerializeField, Header("UNIT TIME")]
    public float UNIT_TIME;//单位时间

    [SerializeField, Header("Countdown of GameStart")]
    public float GAMESTART_TIME;//开始游戏倒计时

    [SerializeField, Header("Novice Player ")]
    public bool isFrist;// 是否新手

    [SerializeField, Header("Sound is Mute")]
    public bool isMute;//是否静音

    [SerializeField, Header("Player Info")]
    public PlayerInfo playerInfo;
  
    [SerializeField, Header("Tiles Info")]
    public TilesInfo TilesInfo;

    [SerializeField, Header("Customize (Self,Left,Right,Up,Down)")]
    public Vector2[] mDirectionExample;
}

[System.Serializable]
public class TileLine
{
    public int[] indexOfLine;
}

[System.Serializable]
public class PlayerInfo
{
    [SerializeField, Header("Player Health")]
    public int playerHealth;//玩家的血值

    [SerializeField, Header("Player Time Of One Grid")]
    public float timeOfOneGrid;//经过一格的时间

    [SerializeField, Header("Player Is Invincible ?")]
    public bool isInvincible;//无敌 
}

[System.Serializable]
public class TilesInfo
{

    [SerializeField, Header("Tile Line List")]
    public TileLine[] TileLineList;//地块行和列

    [SerializeField, Header("Tile Distance")]
    public Vector3 TileDistance;//间距

    [SerializeField, Header("Which One is Discard ?")]
    public List<int> discardTile; //不能用的地块             
}
