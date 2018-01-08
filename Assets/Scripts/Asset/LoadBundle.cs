using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadBundle : MonoBehaviour
{

    public delegate void CompleteEvent(GameObject go);
    public CompleteEvent OnComplete;

    public ILoadBundle load;

    public string path
    {
        get;
        set;
    }
    private int index = 0;

    public void DoLoadBundle()
    {
        index = gameObject.GetInstanceID();
        GameObject go = AssetsMgr.GetInstance().GetAsset<GameObject>(path);
        if (go != null)
        {
            // Debug.Log("get asset in hsloadAsset  =>" + path);
            if (OnComplete != null)
            {
                OnComplete(go);
            }
            if (load != null)
            {
                load.LoadBundleBack(go);
            }
        }
        else
        {
            // Debug.Log("load new assetbundle   =>" + path);
            AssetsMgr.GetInstance().OnComplete += OnLoadComplete;

            AssetsMgr.GetInstance().LoadAsset(path, false, index);
        }
    }

    public void OnLoadComplete(string url, AssetBundle assetBundle, bool existing, int index)
    {
        if (url.CompareTo(path) != 0)
        {
            return;
        }

        if (index != this.index)
        {
            return;
        }

        AssetsMgr.GetInstance().OnComplete -= OnLoadComplete;

        GameObject go = null;

        if (existing)
        {

            if (assetBundle == null)
            {
                go = AssetsMgr.GetInstance().GetAsset<GameObject>(url);
            }
            else
            {
                go = assetBundle.LoadAsset<GameObject>(System.IO.Path.GetFileNameWithoutExtension(url));
                if (go != null)
                {
                    AssetsMgr.GetInstance().SetAsset(url, go);
                }
                assetBundle.Unload(false);
            }
        }
        if (go != null)
        {
            if (OnComplete != null)
            {
                OnComplete(go);
            }
            if (load != null)
            {
                load.LoadBundleBack(go);
            }
        }

    }

}
