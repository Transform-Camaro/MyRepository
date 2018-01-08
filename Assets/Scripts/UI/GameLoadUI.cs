using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLoadUI : MonoBehaviour
{
    private Dictionary<string, Sprite> mSpriteDict;

    private Transform mTransform;
    public delegate void LoadProgress(float value);
    public LoadProgress loadProgress;

    public delegate void OnLoadEnd();
    public OnLoadEnd loadEnd;

    private float maxValue = 0;
    private float SliderValue = 0;

    private float sliderSpeed = 0.2f;

    private Bezier.Bezier mBezier;

    private List<Image> mSprite;//人踩的亮灯

    private Sprite sprite0;
    private Sprite sprite1;
    //  private Sprite followSprite;

    private Image mLoadingGo;//人
                             //  private List<Image> mImageList;//存小点的list

    // private const int imageLength = 8;//一共几个小点
    //  private const float alphaMin = 1 / (float)imageLength;//每个小点透明递减
    // private List<Vector3> imagePos;//存的领头的移动坐标
    //  private const int firstDistance = 4;//第一个小点距离人的距离
    // private const int imageDistance = 10;//每个点之间的距离 =》单位是 imagePos 的数量
    //private Vector3 offset = new Vector3(20, 20, 0);//小点跟人偏移的距离

    private const float NUM = 7;//分成几份
    private const float startPos = -450;//左边坐标
    private const float endPos = 450;//右边坐标
    private const float hight = 150;//高度
    private const float Scale = 1 / NUM;//每一份的大小
    private float unit_X = (Mathf.Abs(startPos) + Mathf.Abs(endPos)) / NUM;//单位长度

    void Awake()
    {
        mTransform = this.transform;
    }
    void Start()
    {
        RectTransform rt = GetComponent<RectTransform>();
        rt.sizeDelta = UIController.instance.ScreenSize;

        mTransform.localPosition = Vector3.zero;

        mBezier = new Bezier.Bezier(new Vector3(startPos, 0, 0),
            new Vector3(0, hight, 0),
            new Vector3(0, hight, 0),
            new Vector3(unit_X + startPos, 0, 0));//一个跳转格子的

        sprite0 = GetSpriteOnName("loading0");
        sprite1 = GetSpriteOnName("loading1");
        // followSprite = GetSpriteOnName("loading4");

        if (mLoadingGo == null)
        {
            mLoadingGo = new GameObject().AddComponent<Image>();
            mLoadingGo.transform.SetParent(mTransform);
            mLoadingGo.sprite = GetSpriteOnName("loading3");
            mLoadingGo.raycastTarget = false;
            SetPosition(0);
        }

        if (mSprite == null)
            mSprite = new List<Image>();

        for (int i = 0; i < NUM; i++)
        {
            GameObject go = new GameObject();
            go.transform.SetParent(mTransform);

            float x = (i - (float)(NUM * 0.5f) + 0.5f) * unit_X;

            go.transform.localScale = Vector3.one * 0.35f;

            go.transform.localPosition = new Vector3(x, -60, 0);

            go.SetActive(true);

            Image img = go.AddComponent<Image>();

            img.sprite = sprite0;

            img.raycastTarget = false;

            mSprite.Add(img);
        }


        //if (mImageList == null)
        //    mImageList = new List<Image>();

        //for (int i = 0; i < imageLength; i++)
        //{
        //    GameObject go = new GameObject();
        //    go.gameObject.SetActive(true);
        //    go.transform.SetParent(mTransform);
        //    go.transform.localScale = Vector3.one * 0.2f;
        //    Image image = go.AddComponent<Image>();
        //    mImageList.Add(image);
        //    image.sprite = followSprite;
        //    image.raycastTarget = false;
        //    image.color = new Color(image.color.r, image.color.g, image.color.b, 1 - i * alphaMin);
        //}
        //if (imagePos == null)
        //    imagePos = new List<Vector3>();
       
    }

    public void SetSliderValue(float _value)
    {
        maxValue = _value;
    }

    void Update()
    {
        if (SliderValue < maxValue)
        {
            SliderValue += Time.deltaTime * sliderSpeed;

            SetPosition(SliderValue);

            if (loadProgress != null)
            {
                loadProgress(SliderValue);
            }
            if (SliderValue >= 1)//加载结束
            {
                maxValue = 0;
                //  imagePos.Clear();
                if (loadEnd != null)
                {
                    loadEnd();
                }
            }
        }
    }
    void OnDestroy()
    {
        mBezier = null;
        mSpriteDict.Clear();
        mSpriteDict = null;
    }
    //计算人的跳，跟随小点的跳，控制人踩灯的亮
    private void SetPosition(float _value)
    {
        float offsetUnitX = _value + Scale * 0.5f;//单位距离，偏移一半

        float offsetX = unit_X * 0.5f;//整体偏移  , 一半的单位距离

        int unitNum = (int)(offsetUnitX / Scale);//单位数量

        float unitValue = offsetUnitX % Scale;//当前的 ，单位进度

        unitValue = unitValue / Scale;

        Vector3 v3 = mBezier.GetPointAtTime(unitValue);//计算位置，单位进度

        Vector3 goPos = new Vector3(unitNum * unit_X - offsetX, 0, 0) + v3;

        //if (imagePos.Count > (imageDistance * imageLength))
        //{
        //    imagePos.RemoveAt(0);
        //}

        //imagePos.Add(goPos - offset);//存路径点，跟随小点需要

        mLoadingGo.transform.localPosition = goPos;

        for (int i = 0; i < unitNum; i++)
        {
            if (i < mSprite.Count)
            {
                mSprite[i].sprite = sprite1;
            }
        }
        //for (int i = 0; i < imagePos.Count; i++)
        //{
        //    if (i < mImageList.Count)
        //    {
        //        int index = imagePos.Count - firstDistance - i;
        //        index = index - i * imageDistance;
        //        if (index >= 0)
        //        {
        //            mImageList[i].transform.localPosition = imagePos[index];
        //        }
        //    }
        //}
    }
    private Sprite GetSpriteOnName(string name)
    {
        Sprite sprite = null;

        if (mSpriteDict == null)
        {
            mSpriteDict = new Dictionary<string, Sprite>();
            string path = AssetsPaths.GameAtlasOutPath;
            AssetBundle mAssetBundle = AssetBundle.LoadFromFile(path);
            Sprite[] sp = mAssetBundle.LoadAllAssets<Sprite>();
            for (int i = 0; i < sp.Length; i++)
            {
                mSpriteDict.Add(sp[i].name, sp[i]);
            }
            mAssetBundle.Unload(false);
            mAssetBundle = null;
        }

        if (mSpriteDict != null && mSpriteDict.ContainsKey(name))
        {
            sprite = mSpriteDict[name];
        }
        return sprite;
    }

}
