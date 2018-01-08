using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FunctionProgress
{
    public FunctionProgress(Action action, float progress)
    {
        mAction = action;
        mProgress = progress;
    }
    public Action mAction;
    public float mProgress;
}
public class LoadGameObject : MonoBehaviour
{
    private const float modleScale = 0.1f;

    public delegate void OnLoadGameBeginComplete(
        GameObject ui,
        GameObject msceneModle,
        GameObject mplayerModel,
        GameObject timesign,
        GameObject[] timeModel,
        GameObject[] mbloodModel,
        GameObject[] countDownModel,
        GameObject mplayerEffect,
        Dictionary<EnemyType, GameObject> EffectDictionaryOfTheEnemy
        );
    public event OnLoadGameBeginComplete mloadComplete;

    private UIController mUIController;

    private Transform mparent;

    private GameObject mGameUI;
    private GameObject msceneModle;
    private GameObject mplayerModel;
    private GameObject[] mbloodModel;
    private GameObject[] timeModel;
    private GameObject timesign;
    private GameObject[] countDownModel;
    private GameObject mplayerEffect;
    private Dictionary<EnemyType, GameObject> EffectDictionaryOfTheEnemy;

    private const int BLOODNUM = 2;
    private const int TIMENUM = 10;
    private const int CDNUM = 4;

    private int mbloodNum;
    private int mtimeNum;
    private int mcountdownNum;


    private List<FunctionProgress> mActionProgress;

    void Awake()
    {
        mbloodNum = 0;
        mtimeNum = 0;
        mcountdownNum = 0;
        mbloodModel = new GameObject[BLOODNUM];
        timeModel = new GameObject[TIMENUM];
        countDownModel = new GameObject[CDNUM];
        if (EffectDictionaryOfTheEnemy == null)
        {
            EffectDictionaryOfTheEnemy = new Dictionary<EnemyType, GameObject>()
            {
                { EnemyType.Lightning, null},
                { EnemyType.WindLeft, null},
                { EnemyType.WindRight, null},
                { EnemyType.Rock, null},
                { EnemyType.Fire, null}
            };
        }
    }


    public void LoadBegin(Transform goParent, UIController uiController)
    {
        mparent = goParent;
        mUIController = uiController;
        if (mActionProgress == null)
        {
            mActionProgress = new List<FunctionProgress>()
            {
                new FunctionProgress(LoadGameUI,0),
                new FunctionProgress(LoadSceneModel, 0),
                new FunctionProgress(LoadPlayerModel, 0),
                new FunctionProgress(LoadPlayerBlood, 0),
                new FunctionProgress(LoadPlayerBackPackEffect,0),
                new FunctionProgress(LoadTime, 0),
                new FunctionProgress(LoadCountDownTime, 0),
                new FunctionProgress(LoadLightningEffect,0),
                new FunctionProgress(LoadFireEffect,0)
        };
        }
        for (int i = 0; i < mActionProgress.Count; i++)
        {
            mActionProgress[i].mAction.Invoke();
        }
    }

    private void LoadGameUI()
    {
        mUIController.onGameUIComplete += OnLoadGameUIComplete;
        mUIController.LoadGameUI();
    }
    private void OnLoadGameUIComplete(string url, GameObject go)
    {
        if (url.CompareTo(AssetsPaths.GameUIPath) != 0)
        {
            return;
        }
        mUIController.onGameUIComplete -= OnLoadGameUIComplete;

        mGameUI = go;
        progressCount(LoadGameUI, 1);
    }

    private void LoadSceneModel()
    {
        string path = AssetsPaths.GameSceneModel;
        AssetsMgr.GetInstance().OnComplete += OnLoadGameSceneModelComplete;
        AssetsMgr.GetInstance().LoadAsset(path);
    }
    private void OnLoadGameSceneModelComplete(string url, AssetBundle assetBundle, bool existing, int index)
    {
        if (url.CompareTo(AssetsPaths.GameSceneModel) != 0)
        {
            return;
        }
        AssetsMgr.GetInstance().OnComplete -= OnLoadGameSceneModelComplete;

        GameObject go = null;
        if (assetBundle != null)
        {
            go = assetBundle.LoadAsset<GameObject>(System.IO.Path.GetFileNameWithoutExtension(url));
            if (go != null)
            {
                AssetsMgr.GetInstance().SetAsset(url, go);
            }
            assetBundle.Unload(false);
        }
        else
        {
            if (existing)
            {
                go = AssetsMgr.GetInstance().GetAsset<GameObject>(url);
            }
        }
        if (go != null)
        {
            msceneModle = Instantiate<GameObject>(go);
            msceneModle.transform.SetParent(mparent);
            msceneModle.transform.localPosition = Vector3.zero + Vector3.down * 4.1f;
            msceneModle.transform.localRotation = Quaternion.Euler(0, 180, 0);
            msceneModle.transform.localScale = Vector3.one * modleScale;
        }
        progressCount(LoadSceneModel, 1);
    }
    private void LoadFireEffect()
    {
        string path = AssetsPaths.GameFireEffect;
        AssetsMgr.GetInstance().OnComplete += OnLoadFireEffectComplete;
        AssetsMgr.GetInstance().LoadAsset(path);
    }
    private void OnLoadFireEffectComplete(string url, AssetBundle assetBundle, bool existing, int index)
    {

        if (url.CompareTo(AssetsPaths.GameFireEffect) != 0)
        {
            return;
        }
        AssetsMgr.GetInstance().OnComplete -= OnLoadFireEffectComplete;
        GameObject go = null;
        if (assetBundle != null)
        {
            go = assetBundle.LoadAsset<GameObject>(System.IO.Path.GetFileNameWithoutExtension(url));
            if (go != null)
            {
                AssetsMgr.GetInstance().SetAsset(url, go);
            }
            assetBundle.Unload(false);
        }
        else
        {
            if (existing)
            {
                go = AssetsMgr.GetInstance().GetAsset<GameObject>(url);
            }
        }
        if (go != null)
        {
            GameObject fireEffect = Instantiate<GameObject>(go);
            fireEffect.SetActive(true);
            EffectDictionaryOfTheEnemy[EnemyType.Fire] = fireEffect;
        }
        progressCount(LoadFireEffect, 1);
    }

    private void LoadLightningEffect()
    {
        string path = AssetsPaths.GameLightningEffect;
        AssetsMgr.GetInstance().OnComplete += OnLoadLightningEffectComplete;
        AssetsMgr.GetInstance().LoadAsset(path);
    }
    private void OnLoadLightningEffectComplete(string url, AssetBundle assetBundle, bool existing, int index)
    {

        if (url.CompareTo(AssetsPaths.GameLightningEffect) != 0)
        {
            return;
        }

        AssetsMgr.GetInstance().OnComplete -= OnLoadLightningEffectComplete;

        GameObject go = null;
        if (assetBundle != null)
        {
            go = assetBundle.LoadAsset<GameObject>(System.IO.Path.GetFileNameWithoutExtension(url));
            if (go != null)
            {
                AssetsMgr.GetInstance().SetAsset(url, go);
            }
            assetBundle.Unload(false);
        }
        else
        {
            if (existing)
            {
                go = AssetsMgr.GetInstance().GetAsset<GameObject>(url);
            }
        }
        if (go != null)
        {
            GameObject lightningEffect = Instantiate<GameObject>(go);
            lightningEffect.SetActive(true);
            EffectDictionaryOfTheEnemy[EnemyType.Lightning] = lightningEffect;
        }
        progressCount(LoadLightningEffect, 1);
    }
    private void LoadPlayerBackPackEffect()
    {
        string path = AssetsPaths.GamePlayerBackPackEffect;
        AssetsMgr.GetInstance().OnComplete += OnLoadGamePlayerEffectComplete;
        AssetsMgr.GetInstance().LoadAsset(path);
    }
    private void OnLoadGamePlayerEffectComplete(string url, AssetBundle assetBundle, bool existing, int index)
    {
        if (url.CompareTo(AssetsPaths.GamePlayerBackPackEffect) != 0)
        {
            return;
        }
        AssetsMgr.GetInstance().OnComplete -= OnLoadGamePlayerEffectComplete;

        GameObject go = null;
        if (assetBundle != null)
        {
            go = assetBundle.LoadAsset<GameObject>(System.IO.Path.GetFileNameWithoutExtension(url));
            if (go != null)
            {
                AssetsMgr.GetInstance().SetAsset(url, go);
            }
            assetBundle.Unload(false);
        }
        else
        {
            if (existing)
            {
                go = AssetsMgr.GetInstance().GetAsset<GameObject>(url);
            }
        }
        if (go != null)
        {
            mplayerEffect = Instantiate<GameObject>(go);
            mplayerEffect.SetActive(true);
        }
        progressCount(LoadPlayerBackPackEffect, 1);
    }

    private void LoadPlayerModel()
    {
        string path = AssetsPaths.GamePlayerModel;
        AssetsMgr.GetInstance().OnComplete += OnLoadGamePlayerComplete;
        AssetsMgr.GetInstance().LoadAsset(path);
    }

    private void OnLoadGamePlayerComplete(string url, AssetBundle assetBundle, bool existing, int index)
    {
        if (url.CompareTo(AssetsPaths.GamePlayerModel) != 0)
        {
            return;
        }
        AssetsMgr.GetInstance().OnComplete -= OnLoadGamePlayerComplete;

        GameObject go = null;
        if (assetBundle != null)
        {
            go = assetBundle.LoadAsset<GameObject>(System.IO.Path.GetFileNameWithoutExtension(url));
            if (go != null)
            {
                AssetsMgr.GetInstance().SetAsset(url, go);
            }
            assetBundle.Unload(false);
        }
        else
        {
            if (existing)
            {
                go = AssetsMgr.GetInstance().GetAsset<GameObject>(url);
            }
        }
        if (go != null)
        {
            mplayerModel = Instantiate<GameObject>(go);
            mplayerModel.transform.SetParent(mparent);
            mplayerModel.transform.localPosition = new Vector3(0, 0, -2);
            mplayerModel.transform.localRotation = Quaternion.Euler(0, 180, 0);
            mplayerModel.transform.localScale = Vector3.one * modleScale;
            mplayerModel.SetActive(true);
        }
        progressCount(LoadPlayerModel, 1);
    }


    private void LoadPlayerBlood()
    {
        string path = AssetsPaths.GamePlayerBlood;
        AssetsMgr.GetInstance().OnComplete += OnLoadPlayerBloodComplete;
        for (int i = 0; i < BLOODNUM; i++)
        {
            AssetsMgr.GetInstance().LoadAsset(path + i);
        }
    }

    private void OnLoadPlayerBloodComplete(string url, AssetBundle assetBundle, bool existing, int index)
    {
        if (!url.Contains(AssetsPaths.GamePlayerBlood))
        {
            return;
        }
        GameObject go = null;
        if (assetBundle != null)
        {
            go = assetBundle.LoadAsset<GameObject>(System.IO.Path.GetFileNameWithoutExtension(url));
            if (go != null)
            {
                AssetsMgr.GetInstance().SetAsset(url, go);
            }
            assetBundle.Unload(false);
        }
        else
        {
            if (existing)
            {
                go = AssetsMgr.GetInstance().GetAsset<GameObject>(url);
            }
        }
        if (go != null)
        {
            GameObject _go = Instantiate<GameObject>(go);
            _go.SetActive(false);
            mbloodModel[mbloodNum] = _go;
        }
        mbloodNum++;
        if (mbloodNum >= BLOODNUM)
        {
            AssetsMgr.GetInstance().OnComplete -= OnLoadPlayerBloodComplete;
        }
        progressCount(LoadPlayerBlood, (float)mbloodNum / BLOODNUM);
    }

    private void LoadTime()
    {
        string path = AssetsPaths.GameTime;
        AssetsMgr.GetInstance().OnComplete += OnLoadTimeComplete;

        AssetsMgr.GetInstance().LoadAsset(path);

        for (int i = 0; i < TIMENUM; i++)
        {
            AssetsMgr.GetInstance().LoadAsset(path + i);
        }
    }

    private void OnLoadTimeComplete(string url, AssetBundle assetBundle, bool existing, int index)
    {
        if (!url.Contains(AssetsPaths.GameTime))
        {
            return;
        }
        GameObject go = null;
        if (assetBundle != null)
        {
            go = assetBundle.LoadAsset<GameObject>(System.IO.Path.GetFileNameWithoutExtension(url));
            if (go != null)
            {
                AssetsMgr.GetInstance().SetAsset(url, go);
            }
            assetBundle.Unload(false);
        }
        else
        {
            if (existing)
            {
                go = AssetsMgr.GetInstance().GetAsset<GameObject>(url);
            }
        }
        if (go != null)
        {
            GameObject _go = Instantiate<GameObject>(go);
            _go.SetActive(false);
            if (go.name == "gametime")
            {
                timesign = _go;
            }
            else
            {
                timeModel[mtimeNum] = _go;
                mtimeNum++;
            }
        }
        if (mtimeNum >= TIMENUM)
        {
            AssetsMgr.GetInstance().OnComplete -= OnLoadTimeComplete;
        }
        progressCount(LoadTime, (float)mtimeNum / TIMENUM);
    }

    private void LoadCountDownTime()
    {
        string path = AssetsPaths.GameTimeCountDown;
        AssetsMgr.GetInstance().OnComplete += OnLoadCountDownComplete;

        for (int i = 0; i < CDNUM; i++)
        {
            AssetsMgr.GetInstance().LoadAsset(path + i);
        }
    }

    private void OnLoadCountDownComplete(string url, AssetBundle assetBundle, bool existing, int index)
    {
        if (!url.Contains(AssetsPaths.GameTimeCountDown))
        {
            return;
        }
        GameObject go = null;
        if (assetBundle != null)
        {
            go = assetBundle.LoadAsset<GameObject>(System.IO.Path.GetFileNameWithoutExtension(url));
            if (go != null)
            {
                AssetsMgr.GetInstance().SetAsset(url, go);
            }
            assetBundle.Unload(false);
        }
        else
        {
            if (existing)
            {
                go = AssetsMgr.GetInstance().GetAsset<GameObject>(url);
            }
        }
        if (go != null)
        {
            GameObject _go = Instantiate<GameObject>(go);
            _go.SetActive(false);
            countDownModel[mcountdownNum] = _go;
        }
        mcountdownNum++;
        if (mcountdownNum >= CDNUM)
        {
            AssetsMgr.GetInstance().OnComplete -= OnLoadCountDownComplete;

        }
        progressCount(LoadCountDownTime, (float)mcountdownNum / CDNUM);
    }

    private void progressCount(Action action, float progress)
    {
        float progressCount = 0;

        if (progress <= 1)
        {
            for (int i = 0; i < mActionProgress.Count; i++)
            {
                if (action == mActionProgress[i].mAction)
                {
                    mActionProgress[i].mProgress = progress;
                }
                progressCount += mActionProgress[i].mProgress;
            }
        }

        mUIController.GetGameLoadUI.SetSliderValue(progressCount / mActionProgress.Count);


        if (progressCount == mActionProgress.Count)
        {
            if (mloadComplete != null)
            {
                mloadComplete(mGameUI, msceneModle,
                    mplayerModel, timesign, timeModel,
                    mbloodModel, countDownModel, mplayerEffect,
                    EffectDictionaryOfTheEnemy);
            }
        }
    }

    void OnDestroy()
    {
        mActionProgress.Clear();
        mActionProgress = null;
        mGameUI = null;
        msceneModle = null;
        mplayerModel = null;
        timesign = null;
        timeModel = null;
        mbloodModel = null;
        countDownModel = null;
        mplayerEffect = null;
    }

}
