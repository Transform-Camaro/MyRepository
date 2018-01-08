using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeDimensionsTime
{
    private enum timeType
    {
        minute_ten,
        minute_one,
        second_ten,
        second_one
    }
    private Dictionary<int, List<GameObject>> mTimeModlePool;//对象池
    private GameObject[] textModle;

    private GameObject minute_oneGO;
    private GameObject minute_tenGO;
    private GameObject second_oneGO;
    private GameObject second_tenGO;

    sbyte o_minute_one = -1;
    sbyte o_minute_ten = -1;
    sbyte o_second_ten = -1;
    sbyte o_second_one = -1;

    sbyte minute_one = 0;
    sbyte minute_ten = 0;
    sbyte second_ten = 0;
    sbyte second_one = 0;
    sbyte minute = 0;
    sbyte second = 0;
    private sbyte[] posx = new sbyte[] { 20, 10, -10, -20 };

    private Transform mTransform;

    public void SetInfo(Transform transform, GameObject[] modle, GameObject sign)
    {
        textModle = modle;
        mTransform = transform;
        for (int i = 0; i < textModle.Length; i++)
        {
            textModle[i].transform.SetParent(mTransform);
            textModle[i].transform.localScale = Vector3.one;
        }
        sign.transform.SetParent(mTransform);
        sign.transform.localPosition = Vector3.zero;
        sign.transform.localScale = Vector3.one;
        sign.transform.localRotation = Quaternion.identity;
        sign.SetActive(true);
    }

    public void OnAwake()
    {
        if (mTimeModlePool == null)
        {
            mTimeModlePool = new Dictionary<int, List<GameObject>>();
            for (int i = 0; i < textModle.Length; i++)
            {
                textModle[i].SetActive(false);
                mTimeModlePool.Add(i, new List<GameObject>() { textModle[i] });
            }
        }

        SetTime(0);
    }
    public void SetTime(int time)
    {
        minute = (sbyte)(time / 60);
        second = (sbyte)(time % 60);
        if (minute < 10)
        {
            minute_ten = 0;
            minute_one = minute;
        }
        else
        {
            minute_ten = (sbyte)(minute / 10);
            minute_one = (sbyte)(minute % 10);
        }
        if (second < 10)
        {
            second_ten = 0;
            second_one = second;
        }
        else
        {
            second_ten = (sbyte)(second / 10);
            second_one = (sbyte)(second % 10);
        }
        if (o_minute_ten != minute_ten)//分_十位
        {
            RecycleModle(o_minute_ten, minute_tenGO);
            minute_tenGO = GetModle(minute_ten, timeType.minute_ten);
            o_minute_ten = minute_ten;
            // Debug.Log("o_minute_ten___" + o_minute_ten);
        }
        if (o_minute_one != minute_one)//分_个位
        {
            RecycleModle(o_minute_one, minute_oneGO);
            minute_oneGO = GetModle(minute_one, timeType.minute_one);
            o_minute_one = minute_one;
            // Debug.Log("o_minute_one___" + o_minute_one);
        }
        if (o_second_ten != second_ten)//秒_十位
        {
            RecycleModle(o_second_ten, second_tenGO);
            second_tenGO = GetModle(second_ten, timeType.second_ten);
            o_second_ten = second_ten;
            // Debug.Log("o_second_ten___" + o_second_ten);
        }
        if (o_second_one != second_one)//秒_个位
        {
            RecycleModle(o_second_one, second_oneGO);
            second_oneGO = GetModle(second_one, timeType.second_one);
            o_second_one = second_one;
            // Debug.Log("second_one___" + second_one);
        }
    }

    private GameObject GetModle(int index, timeType timeType)
    {
        GameObject go = null;

        if (mTimeModlePool.ContainsKey(index) && mTimeModlePool[index].Count > 0)
        {
            go = mTimeModlePool[index][0];
            mTimeModlePool[index].RemoveAt(0);
        }
        else
        {
            go = GameObject.Instantiate<GameObject>(textModle[index]);
        }
        go.transform.SetParent(mTransform);
        go.transform.localPosition = new Vector3(posx[(int)timeType], 0, 0);
        go.transform.localScale = Vector3.one;
        go.transform.localRotation = Quaternion.identity;
        go.SetActive(true);
        return go;
    }
    private void RecycleModle(int index, GameObject go)
    {
        if (go == null) return;
        if (mTimeModlePool.ContainsKey(index))
        {
            mTimeModlePool[index].Add(go);
        }
        else
        {
            // mTimeModlePool.Add(index, new List<GameObject>() { go });
            mTimeModlePool[index].Add(go);
        }
        go.SetActive(false);
    }
}
