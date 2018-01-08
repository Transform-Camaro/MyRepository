using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner
{

    private EnemyConfig enemyConfig;

    private int mEnemyCount = 0;//每轮生成怪物的数量
    [HideInInspector]
    public List<Vector2> mEnemyPos = new List<Vector2>() { };

    private float mGameTime = 0;//计时

    private Dictionary<EnemyType, List<GameObject>> mEnemyPool = new Dictionary<EnemyType, List<GameObject>>();//对象池

    public List<Enemy> mEnemyList;

    private List<EnemyInfo> mEnemyInfo;

    private Vector2[] attackDirectionExample;//在面板定义的攻击范围 类型

    List<EnemyType> mAllEnemyTypeList;//用于生成怪物

    private TileManager mTileManager;

    private TileManager GetTileManager
    {

        get
        {
            if (mTileManager == null)
            {
                mTileManager = GameManager.instance.mTileManager;
            }

            return mTileManager;
        }
    }
    public Transform mTransform;

    public void SetTransform(Transform transform)
    {
        mTransform = transform;
    }
    public void OnAwake()
    {
        enemyConfig = GameController.instance.enemyConfig;
        mEnemyCount = enemyConfig.enemyCount;
        mEnemyInfo = enemyConfig.mEnemyInfo;
        attackDirectionExample = GameController.instance.gameConfig.mDirectionExample;
        mEnemyList = new List<Enemy>();
        LoadEnemyOnAwake();
    }

    public void LoadEnemyOnAwake()
    {
        loadOnAwakeNumber = 0;
        AssetsMgr.GetInstance().OnComplete += LoadEnemyOnAwakeComplete;
        for (int i = 0; i < mEnemyInfo.Count; i++)
        {
            string url = AssetsPaths.GameEnemyPath + i;
            AssetsMgr.GetInstance().LoadAsset(url);
        }
    }
    private int loadOnAwakeNumber = 0;
    public void LoadEnemyOnAwakeComplete(string url, AssetBundle assetBundle, bool existing, int index)
    {
        if (!url.Contains(AssetsPaths.GameEnemyPath))
        {
            //Debug.Log(url + " ((( url Not include => return )))  " + AssetsPaths.GameEnemyPath);
            return;
        }
        loadOnAwakeNumber++;
        if (loadOnAwakeNumber >= mEnemyInfo.Count)
        {
            AssetsMgr.GetInstance().OnComplete -= LoadEnemyOnAwakeComplete;

            //  Debug.Log(" AssetsMgr.GetInstance().OnComplete -= LoadEnemyOnAwakeComplete");
        }
        if (existing)
        {
            if (assetBundle != null)
            {
                GameObject go = assetBundle.LoadAsset<GameObject>(System.IO.Path.GetFileNameWithoutExtension(url));
                if (go != null)
                {
                    //Debug.Log("url =  " + url + " ||   go = " + go.name);
                    AssetsMgr.GetInstance().SetAsset(url, go);
                }
            }
            else
            {
                // Debug.Log("existing = true , assetBundle == null , please  get asset in hsloadAsset  ");
            }
        }
        else
        {
            //Debug.Log("existing = false , errorUrl  !!! ");
        }
    }


    //每波袭击的间隔时间
    private float IntervalTimeOfEnemyAttack()
    {
        float time = GameController.instance.UNIT_TIME *
            enemyConfig.mCreateInfoList[GameManager.instance.GetCurrentLevel - 1].intervalTime;
        return time;
    }

    private EnemyType[] GetEnemyType()
    {
        EnemyType[] EemyTypes = new EnemyType[mEnemyCount];

        List<EnemyType> temp = enemyConfig.mCreateInfoList[GameManager.instance.GetCurrentLevel - 1].EnemyTypes;

        if (mAllEnemyTypeList == null)
        {
            mAllEnemyTypeList = new List<EnemyType>();
        }
        for (int i = 0; i < temp.Count; i++)
        {
            int tempNum = 0;

            for (int k = 0; k < enemyConfig.mEnemyInfo.Count; k++)
            {
                EnemyType type = enemyConfig.mEnemyInfo[k].type;
                if (type == temp[i])
                {
                    tempNum = enemyConfig.mEnemyInfo[k].Probability;
                    break;
                }
            }

            for (int j = 0; j < tempNum; j++)
            {
                mAllEnemyTypeList.Add(temp[i]);
            }
        }

        for (int i = 0; i < mEnemyCount; i++)
        {
            EemyTypes[i] = mAllEnemyTypeList[Random.Range(0, mAllEnemyTypeList.Count)];
        }

        mAllEnemyTypeList.Clear();
        return EemyTypes;
    }

    public void OnUpdate()
    {
        mGameTime -= Time.deltaTime;
        if (mGameTime < 0)
        {
            mGameTime = IntervalTimeOfEnemyAttack();
            Spawn();
        }
        if (mEnemyList.Count > 0)
        {
            for (int i = 0; i < mEnemyList.Count; i++)
            {
                mEnemyList[i].OnUpdate();
            }
        }
    }

    public void ResetAllEnemyInfo()//重置怪物
    {
        mEnemyPos.Clear();

        //Debug.Log("clear mEnemyList  ==>" + mEnemyList.Count);

        for (int i = mEnemyList.Count - 1; i >= 0; i--)
        {
            mEnemyList[i].Disappear();
        }

        mGameTime = IntervalTimeOfEnemyAttack();
    }

    public void Spawn()//生成怪物
    {
        EnemyType[] enemyTypes = GetEnemyType();

        List<List<Tile>> tileList = new List<List<Tile>>();

        for (int i = 0; i < mEnemyCount; i++)
        {
            List<Tile> temp = RandomPos(enemyTypes[i]);//获取一个正确的坐标，范围怪的坐标，攻击范围
            if (temp != null)
            {
                tileList.Add(temp);
            }
        }
        //生成怪物
        EnemyAppearInfo info = new EnemyAppearInfo();

        for (int i = 0; i < mEnemyPos.Count; i++)
        {
            Tile tile = GetTileManager.GetTile((int)mEnemyPos[i].x, (int)mEnemyPos[i].y);

            info.enemyType = enemyTypes[i];
            info.tile = tile;
            info.tileList = tileList[i];
            CreatEnemy(info);
        }
        enemyTypes = null;
        mEnemyPos.Clear();
        info = null;
    }

    private List<Tile> RandomPos(EnemyType enemyType)
    {
        List<Tile> tileList = GetTileManager.GetUnoccupied();
        List<Tile> list = null;

        if (tileList.Count > 0)
        {
            int randomIndex = Random.Range(0, tileList.Count);
            Tile tile = tileList[randomIndex];

            //  pos = new Vector2(tile.TilePos.x, tile.TilePos.y);

            mEnemyPos.Add(new Vector2((int)tile.TilePos.x, (int)tile.TilePos.y));

            //Debug.Log("time = " + time + ",   pos = " + tile.TilePos.x + "," + tile.TilePos.y);

            list = GetAttackRange(enemyType, (int)tile.TilePos.x, (int)tile.TilePos.y);

            for (int K = 0; K < list.Count; K++)
            {
                list[K].SetOccupy(true);//临时占据
            }
        }
        else
        {
            list = null;

        }
        return list;
    }

    //生成怪物
    private void CreatEnemy(EnemyAppearInfo info)
    {

        Enemy enemy = new Enemy(info, this);
        GameObject go = null;
        if (mEnemyPool.ContainsKey(info.enemyType) && mEnemyPool[info.enemyType].Count > 0)
        {
            //从对象池里取
            List<GameObject> list = mEnemyPool[info.enemyType];
            go = list[0];
            list.Remove(list[0]);
            enemy.SetEnemyTrans(mTransform, go);
            enemy.Appear();
        }
        else
        {
            //Debug.Log("mEnemyPool is no resource  , i need  creat new enemy ");
            //生成新的怪物
            enemy.LoadBundle();
        }
    }

    public void RecycleEnemy(EnemyType type, Enemy enemy)//回收怪物
    {
        if (mEnemyPool.ContainsKey(type))
        {
            mEnemyPool[type].Add(enemy.mEnemyGo);
            // Debug.Log("add  enemy.mEnemyGo  in  mEnemyPool  key = " + type);
        }
        else
        {
            mEnemyPool[type] = new List<GameObject>() { enemy.mEnemyGo };
            // Debug.Log("creat  enemy.mEnemyGo  in  mEnemyPool  key = " + type);
        }
        enemy.mEnemyGo.SetActive(false);
        mEnemyList.Remove(enemy);
        enemy = null;
    }

    public void AddEnemyOnAppear(Enemy enemy)
    {
        mEnemyList.Add(enemy);
    }

    public List<Tile> GetAttackRange(EnemyType enemyType, int X, int Y)//获取一个怪的攻击范围
    {
        List<Tile> mRangeTile = new List<Tile>();
        Tile tile;
        List<Vector2> range = new List<Vector2>();

        List<EnemyRangeEnum> mRange = null;
        for (int i = 0; i < mEnemyInfo.Count; i++)
        {
            if (mEnemyInfo[i].type == enemyType)
            {
                mRange = mEnemyInfo[i].mRange;//查找怪的攻击范围，在config里定义
                break;
            }
        }

        for (int i = 0; i < mRange.Count; i++)
        {
            int index = (int)mRange[i];
            range.Add(attackDirectionExample[index]);
        };

        for (int i = 0; i < range.Count; i++)
        {
            tile = GetTileManager.GetTile(X + (int)range[i].x, Y + (int)range[i].y);
            if (tile != null)
            {
                mRangeTile.Add(tile);
            }
        }
        range = null;
        tile = null;
        return mRangeTile;
    }

}
