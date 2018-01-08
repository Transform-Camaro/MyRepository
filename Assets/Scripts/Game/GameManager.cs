using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameManager
{
    private GameObject mGameobject;
    private Vuforia.DefaultTrackableEventHandler mDefaultTrackableEventHandler;


    public static GameManager instance;                                                    //单利.
    private int currentLevel = 1;                                                           //当前的关数

    public int GetCurrentLevel
    {
        get
        {
            return currentLevel;
        }
    }

    private EnemyConfig enemyConfig;

    [HideInInspector]
    public GameUI mGameUI;

    [HideInInspector]
    public float mGameTime = 0;//游戏的时间
    private int o_mCountDown = 0;
    private float mCountDown = 0;//倒计时

    private GameObject mCountDownGO;
    private GameObject[] mCountDownModel;//倒计时的模型

    private bool isTimeing = false;//是否计时
    private bool isTrackingFound = true;//是否能追踪到识别图
    private bool isLevelUp = false;//计算难度升级
    private bool isGameStart = false;//游戏是否在进行中

    public ThreeDimensionsBlood mThreeDimensionsBlood;//3D血
    public ThreeDimensionsTime mThreeDimensionsTime;//3D时间
    public PlayerController mPlayerController;//角色控制器
    public Player mPlayer;//游戏角色
    public TileManager mTileManager;//地块生成器
    public EnemySpawner mEnemySpawner;//敌人生成器
 

    public bool GameStart//游戏开始或玩家死亡
    {
        get
        {
            return isGameStart;
        }
    }
    private bool isgameing;
    public bool IsGameing
    {
        get
        {
            isgameing = false;

            if (isTrackingFound && isTimeing)
            {
                isgameing = true;
            }

            return isgameing;
        }
    }
    public void Pause(bool ispause)//暂停功能
    {
        isTimeing = ispause;
    }


    //如果游戏结束，计算积分，重新开始按钮出现
    public void OnPlayerDie()
    {
        isGameStart = false;
        isTimeing = false;
        mGameUI.OnGameOver();

        GameController.instance.LoadFinish(mGameTime);
    }
    public GameManager(GameObject gameobject)
    {
        mGameobject = gameobject;
    }

    public void OnAwake()
    {
        instance = this;
        isLevelUp = false;
        enemyConfig = GameController.instance.enemyConfig;

        mPlayerController = new PlayerController(mPlayer);

        mGameUI = GameController.instance.uiController.mGameUI;

        mDefaultTrackableEventHandler = GameController.instance.GetDefaultTrackableEventHandler;

    }


    private void TrackingIsFound(bool isFound)
    {
        isTrackingFound = isFound;
        mGameobject.SetActive(isTrackingFound);
    }


    public void OnDestroy()
    {
        instance = null;
        mPlayerController.OnManagerDestory();
        mPlayerController = null;
        GameController.instance.GetDefaultTrackableEventHandler.isTrackingFound -= TrackingIsFound;
        mThreeDimensionsBlood.OnManagerDestory();
    }

    public void ResetGameInfo()//重置所有信息，回到初始
    {
        mTileManager.OnAwake();
        mEnemySpawner.OnAwake();
        mPlayer.OnAwake();
        mThreeDimensionsBlood.OnAwake();
        mThreeDimensionsTime.OnAwake();

        mThreeDimensionsBlood.SetPlayerBloodModle(GameController.instance.gameConfig.playerInfo.playerHealth);

        currentLevel = 1;
        isLevelUp = true;
        mGameTime = 0;

        mTileManager.ResetAllTileInfo();//地块

        mPlayer.ResetPlayerInfo();//玩家

        mEnemySpawner.ResetAllEnemyInfo();//怪物
    }

    public void StartGame()
    {
        //通知开始游戏
        if (GameController.instance.IsFrist)
        {
            //显示动图

            Debug.Log("是新手，需要添加新手引导---------------");

            OnHelpIsClose();
        }
        else
        {
            OnTimeStart();
        }

        //倒计时

    }
    public void OnHelpIsClose()
    {
        GameController.instance.IsFrist = false;
        OnTimeStart();
    }
    private void OnTimeStart()
    {
        mCountDown = GameController.instance.gameConfig.GAMESTART_TIME;

    }
    private void OnTimeEnd()//倒计时结束，游戏开始
    {
        mCountDownGO.SetActive(false);
        isGameStart = true;
        isTimeing = true;

        mDefaultTrackableEventHandler.isTrackingFound += TrackingIsFound;
        isTrackingFound = mDefaultTrackableEventHandler.GetTrackFound;//初始化

    }

    public void OnUpdate()
    {
        if (mCountDown > 0)
        {
            mCountDown -= Time.deltaTime;

            SetCountDownModle((int)mCountDown);
            if (mCountDown <= 0)
            {
                OnTimeEnd();
            }
        }
        if (IsGameing)
        {
            mGameTime += Time.deltaTime;
            if (isLevelUp)
            {
                if (currentLevel < enemyConfig.mCreateInfoList.Count)
                {
                    if (mGameTime >= enemyConfig.mCreateInfoList[currentLevel].TimePoint)
                    {
                        currentLevel++;
                        if (currentLevel > enemyConfig.mCreateInfoList.Count - 1)
                        {
                            isLevelUp = false;
                        }
                    }
                }
            }

            mThreeDimensionsTime.SetTime((int)mGameTime);

            if (mPlayer != null)
                mPlayer.OnUpdate();

            if (mEnemySpawner != null)
                mEnemySpawner.OnUpdate();

            if (mTileManager != null)
                mTileManager.OnUpdate();
        }
    }
    private void SetCountDownModle(int time)
    {
        if (mCountDownGO != null)
        {
            mCountDownGO.SetActive(false);
        }
        if (time != o_mCountDown)
        {
            o_mCountDown = time;
            mCountDownGO = mCountDownModel[time];
        }
        mCountDownGO.transform.localPosition = Vector3.zero;
        mCountDownGO.SetActive(true);
    }

    public void SetCountDownModel(GameObject[] countDownModel, Transform transform)
    {
        mCountDownModel = countDownModel;
        for (int i = 0; i < mCountDownModel.Length; i++)
        {
            mCountDownModel[i].transform.SetParent(transform);
            mCountDownModel[i].transform.localPosition = Vector3.zero;
            mCountDownModel[i].transform.localScale = Vector3.one;
            mCountDownModel[i].transform.localRotation = Quaternion.identity;
        }

    }
}
