using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LoadAssetBundle : MonoBehaviour
{

    public delegate void ProgressEvent(string url, float progress);
    public event ProgressEvent OnProgress;

    public delegate void CompleteEvent(string url, AssetBundle assetBundle, bool existing, int index);
    public event CompleteEvent OnComplete;

    public delegate void ResetEvent(string url);
    public event ResetEvent OnReset;


    private UnityWebRequest www;
    private bool isLoading;
    private float progress;
    private string loadingFile = string.Empty;

    public void Load(string url, int index)
    {
        if (isLoading)
        {
            return;
        }
        isLoading = true;
        progress = -1;
        loadingFile = url;
        url = AssetsPaths.LocalPath + url;
        //Debug.Log("load url =>   " + url);
        StartCoroutine(LoadHandler(url, index));
    }


    private IEnumerator LoadHandler(string path, int index)
    {
        www = UnityWebRequest.GetAssetBundle(path);

        yield return www.Send();

        DownloadHandlerAssetBundle ab = (DownloadHandlerAssetBundle)www.downloadHandler;
        bool existing = true;
        if (www.error != null && (www.error.IndexOf("404") == 0 || www.error.IndexOf("Couldn't open") == 0))
        {
            existing = false;
        }

        if (www.error == null)
        {
            OnComplete(loadingFile, ab.assetBundle, existing, index);
        }
        else
        {
            //文件不存在的时候访问www.assetBundle会出问题
            if (OnProgress != null)
            {
                OnProgress(loadingFile, 1);
            }

            OnComplete(loadingFile, null, existing, index);
        }

        isLoading = false;
        progress = -1;
        www = null;
        OnReset(loadingFile);
    }

    void Update()
    {
        if (isLoading)
        {
            if (OnProgress != null)
            {
                if (progress != www.uploadProgress)
                {
                    progress = www.uploadProgress;
                    //Debug.Log(www.url + ">>>>" + www.progress);
                    OnProgress(loadingFile, www.uploadProgress);
                }
            }
        }
    }

}
