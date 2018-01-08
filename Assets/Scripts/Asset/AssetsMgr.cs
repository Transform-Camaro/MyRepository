using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetsMgr : MonoBehaviour
{
    public delegate void ProgressEvent(string url, float progress);
    public event ProgressEvent OnProgress;

    public delegate void CompleteEvent(string url, AssetBundle assetBundle, bool existing, int index);
    public event CompleteEvent OnComplete;

    private static AssetsMgr instance;

    private LoadAssetBundle loader;
    private Hashtable htLoadedAssets;
    private bool isLocked = false;//加载的时候锁定

    private static List<string> errorAssets;//错误资源路径

    private static List<AssetInfo> assetList;

    private AssetInfo mAssetInfo;

    public static AssetsMgr GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        isLocked = false;
        loader = gameObject.AddComponent<LoadAssetBundle>();
        loader.OnComplete += OnLoadComplete;
        loader.OnReset += OnLoadReset;
        instance = this;
        htLoadedAssets = new Hashtable();
        errorAssets = new List<string>();
        assetList = new List<AssetInfo>();
    }


    public void LoadAsset(string url, bool progress = false, int index = 0)
    {
        if (errorAssets.Contains(url))
        {
            OnComplete(url, null, false, index);
            return;
        }
        if (htLoadedAssets.ContainsKey(url))
        {
            OnComplete(url, null, true, index);
            return;
        }
        AssetInfo info;
        if (assetList.Count > 0)
        {
            bool isExisting = false;
            for (int i = 0; i < assetList.Count; i++)
            {
                info = assetList[i];
                if (info.url == url && index == info.index)
                {
                    isExisting = true;
                    break;
                }
            }
            info = null;
            if (isExisting)
            {
                return;
            }
        }
        info = new AssetInfo();
        info.url = url;
        info.progress = progress;
        info.index = index;
        assetList.Add(info);
    }


    public T GetAsset<T>(string url)
    {
        if (htLoadedAssets.ContainsKey(url))
        {
            return (T)htLoadedAssets[url];
        }
        T rel = default(T);
        return rel;
    }
    public void SetAsset<T>(string url, T asset)
    {
        if (htLoadedAssets.ContainsKey(url))
        {
            return;
        }
        htLoadedAssets[url] = asset;
    }

    public void RemoveAsset(string url)
    {
        if (htLoadedAssets.ContainsKey(url))
        {
            htLoadedAssets.Remove(url);
        }
    }
    public void RemoveAllAsset()
    {
        if (htLoadedAssets.Count > 0)
        {
            foreach (string item in htLoadedAssets.Keys)
            {
                if (htLoadedAssets[item] != null && htLoadedAssets[item] as AssetBundle)
                {
                    (htLoadedAssets[item] as AssetBundle).Unload(true);
                }
            }
            htLoadedAssets.Clear();
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
    }

    private void OnLoadReset(string url)
    {
        if (mAssetInfo.progress)
        {
            loader.OnProgress -= OnLoadProgress;
        }
        isLocked = false;
    }

    private void OnLoadComplete(string url, AssetBundle assetBundle, bool existing, int index)
    {
        if (OnComplete == null)
        {
            if (assetBundle != null)
            {
                assetBundle.Unload(false);
            }
            return;
        }

        if (!existing)
        {
            if (!errorAssets.Contains(url))
            {
                errorAssets.Add(url);
            }
            Debug.Log("加载失败，路径错误");
        }

        OnComplete(url, assetBundle, existing, index);

        if (assetBundle != null)
        {
            assetBundle.Unload(false);
        }

    }
    private void OnLoadProgress(string url, float progress)
    {
        if (OnProgress != null)
        {
            OnProgress(url, progress);
        }
    }

    void Update()
    {
        if (!isLocked)
        {
            if (assetList.Count > 0)
            {
                if (htLoadedAssets.ContainsKey(assetList[0].url))
                {
                    OnComplete(assetList[0].url, null, true, assetList[0].index);
                    assetList.RemoveAt(0);
                    return;
                }

                isLocked = true;

                mAssetInfo = assetList[0];
                assetList.RemoveAt(0);
                loader.Load(mAssetInfo.url, mAssetInfo.index);
                if (mAssetInfo.progress)
                {
                    loader.OnProgress += OnLoadProgress;
                }
            }
            else
            {
                mAssetInfo = null;
            }
        }
    }
}

class AssetInfo
{
    public string url;
    public bool progress;
    public int index;
}
