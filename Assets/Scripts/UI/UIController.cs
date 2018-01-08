using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    private GameLoadUI mGameLoadUI;

    public GameLoadUI GetGameLoadUI
    {
        get
        {
            if (mGameLoadUI == null)
            {
                GameObject loadui = new GameObject();
                loadui.transform.SetParent(this.transform);
                loadui.transform.localPosition = Vector3.zero;
                loadui.SetActive(true);
                loadui.AddComponent<RectTransform>();
                mGameLoadUI = loadui.AddComponent<GameLoadUI>();
                mGameLoadUI.loadEnd += OnLoadEnd;
            }
            return mGameLoadUI;
        }
    }

    [HideInInspector]
    public GameUI mGameUI;
    // private GameBeginUI mGameBeginUI;
    //  private GameFinishUI mGameFinishUI;


    public delegate void OnUILoadEnd();
    public event OnUILoadEnd onUILoadEnd;

    public delegate void OnGameUIComplete(string url, GameObject go);
    public event OnGameUIComplete onGameUIComplete;

    public delegate void OnGameFinishUIComplete(string url, GameObject go);
    public event OnGameFinishUIComplete onGameFinishUIComplete;

    public Vector2 ScreenSize
    {
        get
        {
            return this.transform.GetComponent<RectTransform>().sizeDelta;
        }
    }

    void Awake()
    {
        instance = this;
    }

    public void OnLoadEnd()
    {
        if (mGameLoadUI != null)
        {
            mGameLoadUI.loadEnd -= OnLoadEnd;
            Destroy(mGameLoadUI.gameObject);
            mGameLoadUI = null;
        }
        onUILoadEnd();
    }

    public void LoadGameUI()
    {
        string path = AssetsPaths.GameUIPath;
        AssetsMgr.GetInstance().OnComplete += OnLoadGameUIComplete;
        AssetsMgr.GetInstance().LoadAsset(path);

    }
    public void OnLoadGameUIComplete(string url, AssetBundle assetBundle, bool existing, int index)
    {
        if (url.CompareTo(AssetsPaths.GameUIPath) != 0)
        {
            return;
        }
        //Debug.Log(url + "__" + AssetsPaths.GameUIPath);
        AssetsMgr.GetInstance().OnComplete -= OnLoadGameUIComplete;
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
        GameObject gameui = null;
        if (go != null)
        {
            gameui = Instantiate(go);
            mGameUI = gameui.GetComponent<GameUI>();
            gameui.SetActive(false);
            gameui.transform.SetParent(this.transform);
            //GameController.instance.AddAssetInList(url, gameui.gameObject);
        }

        if (onGameUIComplete != null)
        {
            onGameUIComplete(url, gameui);
        }

    }


    public void LoadGameFinishUI()
    {
        string path = AssetsPaths.GameUIFinishPath;
        AssetsMgr.GetInstance().OnComplete += OnLoadGameFinishUIComplete;
        AssetsMgr.GetInstance().LoadAsset(path);
    }
    public void OnLoadGameFinishUIComplete(string url, AssetBundle assetBundle, bool existing, int index)
    {
        if (url.CompareTo(AssetsPaths.GameUIFinishPath) != 0)
        {
            return;
        }
        // Debug.Log(url + "__" + AssetsPaths.GameUIFinishPath);
        AssetsMgr.GetInstance().OnComplete -= OnLoadGameFinishUIComplete;

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
        GameObject gameFinishUI = null;
        if (go != null)
        {
            gameFinishUI = Instantiate(go);
            gameFinishUI.gameObject.SetActive(false);
            gameFinishUI.transform.SetParent(this.transform);
            // GameFinishUI mGameFinishUI = gameFinishUI.GetComponent<GameFinishUI>();
            // GameController.instance.AddAssetInList(url, mGameFinishUI.gameObject);
        }
        if (onGameFinishUIComplete != null)
        {
            onGameFinishUIComplete(url, gameFinishUI);
        }
    }
}
