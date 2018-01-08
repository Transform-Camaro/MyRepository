using System;
using System.Collections.Generic;
using UnityEngine;

public class LoadGameBeginObject : MonoBehaviour
{

    public delegate void OnLoadGameBeginComplete(GameObject sceneModel, GameObject startBtn);
    public event OnLoadGameBeginComplete loadComplete;

    private float modleScale = 0.01f;

    private GameObject sceneModle, startbtn, companyLogo;

    private Transform mparent;
    private UIController mUIController;

    private List<FunctionProgress> mActionProgress;

    public void LoadBegin(Transform goParent, UIController uicontroller)
    {
        mparent = goParent;
        mUIController = uicontroller;

        mActionProgress = new List<FunctionProgress>()
        {
            new FunctionProgress(LoadSceneModel,0),
            new FunctionProgress(LoadStartButton,0),
            new FunctionProgress(LoadCompanyLogo,0)
        };
        for (int i = 0; i < mActionProgress.Count; i++)
        {
            mActionProgress[i].mAction.Invoke();
        }
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
            sceneModle = Instantiate<GameObject>(go);
            sceneModle.transform.SetParent(mparent);
            sceneModle.transform.localPosition = Vector3.zero + Vector3.down * 0.4f;
            sceneModle.transform.localRotation = Quaternion.Euler(0, 180, 0);
            sceneModle.transform.localScale = Vector3.one * modleScale;
            sceneModle.SetActive(true);
        }
        progressCount(LoadSceneModel, 1);

    }

    private void LoadCompanyLogo()
    {
        string path = AssetsPaths.CompanyLogo;
        AssetsMgr.GetInstance().OnComplete += OnLoadLogoComplete;
        AssetsMgr.GetInstance().LoadAsset(path);
    }

    private void OnLoadLogoComplete(string url, AssetBundle assetBundle, bool existing, int index)
    {
        if (url.CompareTo(AssetsPaths.CompanyLogo) != 0)
        {
            return;
        }
        AssetsMgr.GetInstance().OnComplete -= OnLoadLogoComplete;

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
            companyLogo = Instantiate<GameObject>(go);
            companyLogo.transform.SetParent(mparent);
            companyLogo.transform.localPosition = new Vector3(0, -0.4f, -0.8f);
            companyLogo.transform.localRotation = Quaternion.Euler(-90, 180, 0);
            companyLogo.transform.localScale = Vector3.one;
            companyLogo.SetActive(true);
        }

        progressCount(LoadCompanyLogo, 1);
    }

    private void LoadStartButton()
    {
        string path = AssetsPaths.GameBeginStartBtn;
        AssetsMgr.GetInstance().OnComplete += OnLoadStartBtnComplete;
        AssetsMgr.GetInstance().LoadAsset(path);
    }

    private void OnLoadStartBtnComplete(string url, AssetBundle assetBundle, bool existing, int index)
    {
        if (url.CompareTo(AssetsPaths.GameBeginStartBtn) != 0)
        {
            return;
        }
        AssetsMgr.GetInstance().OnComplete -= OnLoadStartBtnComplete;

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
            startbtn = Instantiate<GameObject>(go);
            startbtn.transform.SetParent(mparent);
            startbtn.transform.localPosition = new Vector3(0, 0, -0.5f);
            startbtn.transform.localRotation = Quaternion.Euler(-90, -180, 0);
            startbtn.transform.localScale = Vector3.one * modleScale;
            startbtn.SetActive(true);
        }

        progressCount(LoadStartButton, 1);
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
            if (loadComplete != null)
            {
                loadComplete(sceneModle, startbtn);
            }
        }
    }


    void OnDestroy()
    {
        mActionProgress.Clear();
        mActionProgress = null;
        sceneModle = null;
        startbtn = null;
    }
}
