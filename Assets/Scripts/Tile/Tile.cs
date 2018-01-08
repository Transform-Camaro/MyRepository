using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    private GameObject mTileColor;
    private string mTileColorPath;
    private Transform mParent;

    public delegate void OnPlayerStay(TileAttackState info);
    public OnPlayerStay onPlayerStay;

    public List<TileAttackState> mAttState = new List<TileAttackState>();
    public List<TileHintState> mHintState = new List<TileHintState>();

    public Vector2 TilePos//地块的索引(第几排第几个)
    {
        get;
        set;
    }
    public Vector3 V3pos//地块坐标(相对坐标记录一下)
    {
        get;
        set;
    }

    private bool isOccupy = false;//是否被临时占据


    public void SetOccupy(bool istrue)//临时占据
    {
        isOccupy = istrue;

        //在随机选3个敌人的位置，和敌人出生的时候使用
    }

    //设置占据状态
    public bool OccupyState//是否占据(不能生成怪物) false 没有被占据  true 被占据
    {
        get
        {
            bool isTrue = true;
            if (isOccupy)
            {
                isTrue = true;
            }
            else
            {
                if (mAttState.Count > 0 || mHintState.Count > 0)//true = 被占据
                {
                    isTrue = true;
                }
                else
                {
                    isTrue = false;
                }
            }
            return isTrue;
        }
    }

    public Tile(Vector3 v3pos, Vector2 v2pos, Transform parent)
    {
        V3pos = v3pos;
        TilePos = v2pos;
        mParent = parent;
    }

    private TileColor currentcolor = TileColor.no;//当前颜色

    public void ResetTileInfo()//重置地块属性
    {
        mTileColor = null;
        mTileColorPath = null;
        isOccupy = false;
        //从配置表里读颜色
        //defaultTileColor = GameController.instance.enemyConfig.defaultTiletColor;
        //attackTileColor = GameController.instance.enemyConfig.attackTileColor;
        currentcolor = TileColor.no;
        SetTileColor(currentcolor);
        onPlayerStay = null;

        for (int i = 0; i < mHintState.Count; i++)
        {
            mHintState[i] = null;
        }
        mHintState.Clear();

        for (int i = 0; i < mAttState.Count; i++)
        {
            mAttState[i] = null;
        }
        mAttState.Clear();
    }



    //public void SetTransform(Vector3 pos, Quaternion rot)
    //{
    //    transform.localPosition = pos;
    //    transform.localRotation = rot;
    //}

    //在V3相对坐标位置生成一个颜色的地块
    public void SetTileColor(TileColor color)//怪物0,1,2,3,4，正常不显示提示=-1
    {
        if (color == TileColor.no)
        {
            if (mTileColor != null)
                mTileColor.SetActive(false);
            currentcolor = color;
            return;
        }
        if (currentcolor == color)
        {
            return;
        }
        if (mTileColor != null)
        {
            GameObject.Destroy(mTileColor.gameObject);
        }
        //tileColor[] name = GameController.instance.gameConfig.colorTileName;
        //EnemyInfo[] mEnemyInfo = GameController.instance.enemyConfig.mEnemyInfo;
        // mTileColorPath = AssetsPaths.GameTileAssetPath + name[(int)(mEnemyInfo[color].hintTileColor)];
        currentcolor = color;
        mTileColorPath = AssetsPaths.GameTileAssetPath + currentcolor;
        AssetsMgr.GetInstance().OnComplete += OnLoadComplete;
        AssetsMgr.GetInstance().LoadAsset(mTileColorPath);
    }

    private void OnLoadComplete(string url, AssetBundle assetBundle, bool existing, int index)
    {
        if (url.CompareTo(mTileColorPath) != 0)
        {
            return;
        }
        AssetsMgr.GetInstance().OnComplete -= OnLoadComplete;
        GameObject go = null;
        if (existing)
        {
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
                go = AssetsMgr.GetInstance().GetAsset<GameObject>(url);
            }
        }

        if (go != null)
        {
            mTileColor = GameObject.Instantiate(go);
            mTileColor.transform.SetParent(mParent);
            mTileColor.transform.localPosition = V3pos + Vector3.up * -0.1f;//偏移向下一点
            mTileColor.transform.localRotation = Quaternion.Euler(-90, 0, 0);
            mTileColor.transform.localScale = new Vector3(0.1f, 0.1f, 0.03f);
            mTileColor.SetActive(true);
        }

    }

    //设置弃用颜色
    public void SetDiscardState()
    {
        //Color discardTileColor = GameController.instance.enemyConfig.discardTiletColor;
        //SetTileColor(discardTileColor);
    }

    //设置提示颜色
    public void SetHintState(TileHintState State)//怪物类型0,1,2,3,4  //攻击类型1,2,3,4,5
    {

        State.OnEnter(this);

        mHintState.Add(State);

        SetTileColor(GetTileColorNow());

    }


    public void OnHintTimeEnd(TileHintState state)//提示状态结束
    {


        //  SetAttackState(attState);//去攻击状态

        mHintState.Remove(state);
        state = null;
        SetTileColor(GetTileColorNow());
    }
    //设置攻击状态
    public void SetAttackState(TileAttackState State)
    {
        State.OnEnter(this);

        mAttState.Add(State);//添加攻击状态

        //SetTileEffect(State.effectType);//播放特效

        TileAttackState state = GetMaxDamageInAttackState();
        if (onPlayerStay != null && state != null)
        {
            onPlayerStay(state);
        }
    }

    //根据类型播放特效
    //private void SetTileEffect(EnemyType effect)
    //{

    //}

    public void OnAttTimeEnd(TileAttackState attstate)//攻击状态结束
    {
        mAttState.Remove(attstate);
        attstate = null;
        //   SetTileColor(GetTileColorNow());
    }

    public TileAttackState GetMaxDamageInAttackState()//寻找攻击力最高的攻击状态
    {
        TileAttackState state = null;

        int damage = -1;

        if (mAttState.Count > 0)
        {
            for (int i = 0; i < mAttState.Count; i++)
            {
                //if (mAttState[i].damage > damage)//伤害判定
                //{
                //    damage = mAttState[i].damage;
                //    state = mAttState[i];
                //}
                if ((int)mAttState[i].enemtType > damage)//怪物类型判定
                {
                    damage = mAttState[i].damage;
                    state = mAttState[i];
                }
            }
        }
        return state;
    }
    private TileColor GetTileColorNow()
    {
        TileColor color = TileColor.no;
        int damage = -1;

        if (mHintState.Count > 0)
        {
            for (int i = 0; i < mHintState.Count; i++)
            {
                if (mHintState[i].damage > damage)
                {
                    damage = mHintState[i].damage;
                    color = mHintState[i].hintColor;

                }
            }
        }
        return color;
    }

    public void OnUpdate()
    {
        if (mHintState.Count > 0)
        {
            for (int i = 0; i < mHintState.Count; i++)
            {
                mHintState[i].OnUpdate();
            }
        }

        if (mAttState.Count > 0)
        {
            for (int i = 0; i < mAttState.Count; i++)
            {
                mAttState[i].OnUpdate();
            }
        }
    }
}
