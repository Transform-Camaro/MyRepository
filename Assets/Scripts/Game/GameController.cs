using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using Vuforia;
public enum GameStage
{
    GameRecognition,
    GameBegin,
    Game,
    GameFinish
}

public class GameController : MonoBehaviour
{
    public GameStage mGameStage;

    public EnemyConfig enemyConfig;                                                //敌人配置文件

    public GameConfig gameConfig;                                                          //配置文件 

    private bool misFrist = true;

    public bool IsFrist
    {
        get
        {
            return misFrist;
        }
        set
        {
            misFrist = gameConfig.isFrist = value;
        }
    }

    public bool IsMute
    {
        get
        {
            return gameConfig.isMute;
        }
        set
        {
            gameConfig.isMute = value;
            if (value)
            {
                //静音
            }
            else
            {
                //开启声音
            }
        }
    }
    [HideInInspector]
    public float UNIT_TIME = 1f;//单位时间

    public static GameController instance;

    private string StartButtonName = "startButton";//3D开始按钮的名字

    private float mScore = 0;

    public UIController uiController;

    private GameObject mGameManagerGO;
    private GameManager mGameManager;

    public GameObject VuMark;

    public Camera mainCamera;
    public Text uitext;

    private const float modleScale = 0.1f;

    private DefaultTrackableEventHandler mDefaultTrackableEventHandler;

    public DefaultTrackableEventHandler GetDefaultTrackableEventHandler
    {
        get
        {
            if (mDefaultTrackableEventHandler == null)
            {
                mDefaultTrackableEventHandler = VuMark.GetComponent<DefaultTrackableEventHandler>();
            }
            return mDefaultTrackableEventHandler;
        }
    }

    private List<GameObject> loadAssetList = new List<GameObject>();

    private GameObject loadGameGO;
    private LoadGameObject loadGame;
    private LoadGameBeginObject loadGameBegin;


    public UpdateInfo mUpdateInfo;//甲方测试用

    void Awake()
    {
        instance = this;

        misFrist = gameConfig.isFrist;

        Screen.orientation = ScreenOrientation.LandscapeRight;
    }
    void Start()
    {
        Screen.orientation = ScreenOrientation.AutoRotation;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;

        IsMute = gameConfig.isMute;
        LoadGameRecognition();


        mUpdateInfo.gameObject.SetActive(true);

    }

    void Update()
    {
        if (mGameManager != null)
            mGameManager.OnUpdate();
    }

    public void Quit()
    {
        Application.Quit();
    }


    public void LoadUIStart(GameStage stage)  //加载界面
    {
        mGameStage = stage;

        for (int i = 0; i < loadAssetList.Count; i++)
        {
            Destroy(loadAssetList[i]);
        }
        loadAssetList.Clear();

        uiController.onUILoadEnd += onLoadEnd;
        // uiController.LoadStart();

        switch (stage)
        {
            case GameStage.GameRecognition:
                // LoadGameRecognition();
                break;
            case GameStage.GameBegin:
                LoadBegin();
                break;
            case GameStage.Game:
                LoadGame();
                break;
            case GameStage.GameFinish:
                // LoadFinish();
                break;
            default:
                break;
        }
    }



    private void onLoadEnd()//打开所有加载的资源
    {
        uiController.onUILoadEnd -= onLoadEnd;
        for (int i = 0; i < loadAssetList.Count; i++)
        {
            loadAssetList[i].SetActive(true);
        }
    }

    private void LoadGameRecognition()//扫描
    {
        mGameStage = GameStage.GameRecognition;
        GetDefaultTrackableEventHandler.isTrackingFound += TrackingIsFound;
    }

    private void TrackingIsFound(bool isFound)
    {
        if (isFound)
        {

            GetDefaultTrackableEventHandler.isTrackingFound -= TrackingIsFound;
            //LoadGame();
            LoadUIStart(GameStage.GameBegin);
        }
    }

    public void ExitGame(GameStage gameStage)//从游戏中退出
    {
        mGameManager.OnDestroy();
        mGameManager = null;
        LoadUIStart(gameStage);
    }

    #region  --LoadBeginPanel
    private void LoadBegin()//游戏开始界面
    {
        mUpdateInfo.gameObject.SetActive(true);

        loadGameGO = new GameObject();
        GameObject loadParent = new GameObject();
        loadParent.transform.SetParent(VuMark.transform.GetChild(0));
        loadParent.transform.localRotation = Quaternion.identity;
        loadParent.transform.localScale = Vector3.one;
        loadParent.transform.localPosition = Vector3.zero;
        loadParent.SetActive(false);
        loadAssetList.Add(loadParent);
        loadGameBegin = loadGameGO.AddComponent<LoadGameBeginObject>();
        loadGameBegin.loadComplete += OnLoadGameBeginComplete;
        loadGameBegin.LoadBegin(loadParent.transform, uiController);
    }
    private void OnLoadGameBeginComplete(GameObject sceneModel, GameObject startBtn)
    {
        loadGameBegin.loadComplete -= OnLoadGameBeginComplete;
        startBtn.name = StartButtonName;
        MouseOrTouch.OnTouchEnd += StartButtonOnClick;
        Destroy(loadGameBegin);
        Destroy(loadGameGO);
    }

    private void StartButtonOnPress(MouseOrTouch.MouseOrTouchInfo info)
    {
        if (info.currentGo != null && info.currentGo.name == StartButtonName)
        {

        }
        else
        {

        }
    }
    private void StartButtonOnClick(MouseOrTouch.MouseOrTouchInfo info)
    {
        if (info.currentGo != null && info.currentGo.name == StartButtonName)
        {
            mUpdateInfo.gameObject.SetActive(false);//修改配置文件关掉
            UNIT_TIME = gameConfig.UNIT_TIME;//开始游戏时检查一下

            MouseOrTouch.OnTouchEnd -= StartButtonOnClick;

            LoadUIStart(GameStage.Game);
        }
    }
    #endregion

    #region  --LoadGameScene
    private void LoadGame()//游戏界面
    {
        loadGameGO = new GameObject();
        mGameManagerGO = new GameObject();
        mGameManagerGO.transform.SetParent(VuMark.transform.GetChild(0));
        mGameManagerGO.transform.localRotation = Quaternion.identity;
        mGameManagerGO.transform.localScale = Vector3.one * 0.1f;
        mGameManagerGO.transform.localPosition = Vector3.zero;
        mGameManagerGO.SetActive(false);
        loadAssetList.Add(mGameManagerGO);
        loadGame = loadGameGO.AddComponent<LoadGameObject>();
        loadGame.mloadComplete += OnLoadGameObjectComplete;
        loadGame.LoadBegin(mGameManagerGO.transform, uiController);
    }

    private void OnLoadGameObjectComplete(GameObject ui,
        GameObject msceneModle,
        GameObject mplayerModel,
        GameObject timesign,
        GameObject[] timeModel,
        GameObject[] mbloodModel,
        GameObject[] countDownModel,
        GameObject mplayerEffect,
        Dictionary<EnemyType, GameObject> EffectDictionaryOfTheEnemy)
    {
        loadGame.mloadComplete -= OnLoadGameObjectComplete;

        loadAssetList.Add(ui);

        mGameManager = new GameManager(mGameManagerGO);

        SetPlayerInfo(mplayerModel, mplayerEffect, EffectDictionaryOfTheEnemy);
        SetTileEnemyInfo();
        SetBloodInfo(mbloodModel);
        SetTimeInfo(timeModel, timesign);
        SetCountDownTime(countDownModel);

        Destroy(loadGame);
        Destroy(loadGameGO);
        mGameManager.OnAwake();
        mGameManager.ResetGameInfo();//重置游戏数据
        uiController.onUILoadEnd += OnGameManageLoadComplete;
    }

    private void SetTimeInfo(GameObject[] timeModel, GameObject timesign)
    {

        GameObject timeManage = new GameObject();
        timeManage.transform.SetParent(mGameManagerGO.transform);
        timeManage.transform.localPosition = new Vector3(-4, -2.5f, -7);
        timeManage.transform.localRotation = Quaternion.Euler(180, 180, 0);
        timeManage.transform.localScale = Vector3.one * 0.08f;

        ThreeDimensionsTime time = new ThreeDimensionsTime();
        time.SetInfo(timeManage.transform, timeModel, timesign);

        mGameManager.mThreeDimensionsTime = time;
    }
    private void SetCountDownTime(GameObject[] countDownModel)
    {
        GameObject countdown = new GameObject();
        countdown.transform.SetParent(mGameManagerGO.transform);
        countdown.transform.localPosition = new Vector3(0, 0, -4);
        countdown.transform.localRotation = Quaternion.Euler(-90, 180, 0);
        countdown.transform.localScale = Vector3.one * 0.1f;

        mGameManager.SetCountDownModel(countDownModel, countdown.transform);
    }
    private void SetPlayerInfo(GameObject mplayerModel, GameObject effect1, Dictionary<EnemyType, GameObject> effectOfTheEnemy)
    {
        GameObject effect2 = Instantiate<GameObject>(effect1);
        PlayerBackPackEffect[] playerEffects = new PlayerBackPackEffect[2] {
            new PlayerBackPackEffect(effect1.transform),
            new PlayerBackPackEffect(effect2.transform)};

        Player playScript = new Player();
        playScript.SetInfo(mplayerModel.transform, playerEffects, effectOfTheEnemy);
        mGameManager.mPlayer = playScript;
    }

    private void SetTileEnemyInfo()
    {
        GameObject manage = new GameObject();
        manage.name = "TileEnemyManage";
        manage.transform.SetParent(mGameManagerGO.transform);
        manage.transform.localPosition = Vector3.zero;
        manage.transform.localRotation = Quaternion.identity;
        manage.transform.localScale = Vector3.one;
        TileManager mTileManager = new TileManager();
        EnemySpawner mEnemyspawner = new EnemySpawner();
        mEnemyspawner.SetTransform(manage.transform);
        mTileManager.SetTransform(manage.transform);
        mGameManager.mTileManager = mTileManager;
        mGameManager.mEnemySpawner = mEnemyspawner;
    }
    private void SetBloodInfo(GameObject[] mbloodModel)
    {
        GameObject bloodGo = new GameObject();
        for (int i = 0; i < mbloodModel.Length; i++)
        {
            mbloodModel[i].transform.SetParent(bloodGo.transform);
        }
        bloodGo.transform.SetParent(mGameManagerGO.transform);
        bloodGo.transform.localPosition = new Vector3(3, -2.5f, -7);
        bloodGo.transform.localRotation = Quaternion.Euler(90, 0, 0);
        bloodGo.transform.localScale = Vector3.one * 0.08f;
        ThreeDimensionsBlood tdBlood = new ThreeDimensionsBlood();
        tdBlood.SetInfo(bloodGo.transform, mbloodModel);
        mGameManager.mThreeDimensionsBlood = tdBlood;
    }
    public void OnGameManageLoadComplete()
    {
        uiController.onUILoadEnd -= OnGameManageLoadComplete;
        mGameManager.StartGame();
    }
    #endregion

    public void LoadFinish(float score)//加载游戏结束界面
    {
        mScore = score;

        uiController.onGameFinishUIComplete += onGameFinishUIComplete;
        uiController.LoadGameFinishUI();

    }
    public void onGameFinishUIComplete(string url, GameObject ui)//加载完结束界面,得分,动画
    {
        if (url.CompareTo(AssetsPaths.GameUIFinishPath) != 0)
        {
            return;
        }

        uiController.onGameFinishUIComplete -= onGameFinishUIComplete;
        ui.gameObject.SetActive(true);
        ui.GetComponent<GameFinishUI>().SetScore(mScore);
        loadAssetList.Add(ui);
    }
}

