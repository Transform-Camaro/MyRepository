using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeDimensionsBlood
{
    private GameObject emptyBloodGO;//空血条
    private GameObject fullBloodGO;//

    private int PlayerHp;

    private Dictionary<int, GameObject> bloodDict;
    private Transform mTransform;

    public void SetInfo(Transform transfrom, GameObject[] blood)
    {
        mTransform = transfrom;
        fullBloodGO = blood[0];
        emptyBloodGO = blood[1];
    }
    public void OnAwake()
    {
        //emptyBloodGO.SetActive(false);
        //fullBloodGO.SetActive(false);
        PlayerHp = GameController.instance.gameConfig.playerInfo.playerHealth;
        if (bloodDict == null)
        {
            bloodDict = new Dictionary<int, GameObject>();
            for (int i = 0; i < PlayerHp; i++)
            {
                SetBloodInfo(i, fullBloodGO);
            }
        }
    }
    public void OnManagerDestory()
    {
        foreach (GameObject item in bloodDict.Values)
        {
            GameObject.Destroy(item);
        }
        bloodDict.Clear();
        bloodDict = null;
    }
    public void SetPlayerBloodModle(int hp)
    {
        int num = PlayerHp - hp;

        if (num <= bloodDict.Count)
        {
            for (int i = 0; i < bloodDict.Count; i++)
            {
                if (i < num)
                {
                    SetBloodInfo(i, emptyBloodGO);
                }
                else
                {
                    SetBloodInfo(i, fullBloodGO);
                }
            }
        }
    }
    private void SetBloodInfo(int index, GameObject _go)
    {
        if (bloodDict.ContainsKey(index))
        {
            if (bloodDict[index].name.Contains(_go.name))
            {
                return;
            }
        }
        //  Debug.Log("Instantiate " + index + "__" + _go.name);

        GameObject go = GameObject.Instantiate<GameObject>(_go);
        go.transform.SetParent(mTransform);
        go.transform.localPosition = new Vector3((index - (float)PlayerHp / 2 + 0.5f) * 15, 0, 0);
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
        go.SetActive(true);
        if (bloodDict.ContainsKey(index))
        {
            GameObject.Destroy(bloodDict[index]);
        }
        bloodDict[index] = go;
    }
}
