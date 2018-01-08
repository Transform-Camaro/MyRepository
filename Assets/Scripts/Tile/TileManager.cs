using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager
{
    private Transform sTransform;

    private TileLine[] sList;

    private List<Tile> mUnoccupiedTileList = new List<Tile>();//没有被占据的地块

    private Dictionary<int, Tile> mTileDict = new Dictionary<int, Tile>();//存取地块

    private List<int> discardTile;

    //private string mTileName = null;

    // private string tilePath;

    private float hight, width, depth;//高和宽和深度 间隔

    public void SetTransform(Transform transform)
    {
        sTransform = transform;
    }

    public void OnAwake()
    {

        GetConfig();

        LoadTileAssetOnAwake();

        CreatTiles();
    }
    public void OnUpdate()
    {

        if (mTileDict.Count > 0)
        {
            foreach (Tile mTile in mTileDict.Values)
            {
                if (mTile.OccupyState)
                {
                    mTile.OnUpdate();
                }
            }
        }
    }
    private int loadTileNum = 0;
    private void LoadTileAssetOnAwake()
    {
        string path = AssetsPaths.GameTileAssetPath;
        //tileColor[] name = GameController.instance.gameConfig.colorTileName;

        string[] colorName = System.Enum.GetNames(typeof(TileColor));

        loadTileNum = colorName.Length;

        AssetsMgr.GetInstance().OnComplete += LoadTileOnAwakeComplete;
        for (int i = 1; i < loadTileNum; i++)// i = 1  少一个no
        {
            AssetsMgr.GetInstance().LoadAsset(path + colorName[i]);
        }
    }
    private void LoadTileOnAwakeComplete(string url, AssetBundle assetBundle, bool existing, int index)
    {
        if (!url.Contains(AssetsPaths.GameTileAssetPath))
        {
            //Debug.Log(url + " ((( url Not include => return )))  " + AssetsPaths.GameTileAssetPath);
            return;
        }
        loadTileNum--;
        if (loadTileNum <= 1)// = 1  少一个no
        {
            AssetsMgr.GetInstance().OnComplete -= LoadTileOnAwakeComplete;
            // Debug.Log("AssetsMgr.GetInstance().OnComplete -= LoadTileOnAwakeComplete");
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
            //Debug.Log("existing = false , errorUrl  !!!! ");
        }
    }


    private void GetConfig()
    {
        GameConfig gameConfig = GameController.instance.gameConfig;
        hight = gameConfig.TilesInfo.TileDistance.y;//高间隔
        width = gameConfig.TilesInfo.TileDistance.x;//宽间隔
        depth = gameConfig.TilesInfo.TileDistance.z;//深间隔
        discardTile = gameConfig.TilesInfo.discardTile;//不能用的地块
        sList = gameConfig.TilesInfo.TileLineList;//地块排列
    }
    public void CreatTiles()
    {

        for (int i = 0; i < sList.Length; i++)
        {
            for (int k = 0; k < sList[i].indexOfLine.Length; k++)
            {
                Vector3 pos = new Vector3((k + 0.5f - sList[i].indexOfLine.Length / 2) * width + sTransform.localPosition.x,
                    (i + 0.5f - (float)sList.Length / 2) * hight + sTransform.localPosition.y,
                    (i + 0.5f - (float)sList.Length / 2) * depth + sTransform.localPosition.z);

                //   GameObject go = Instantiate(Resources.Load(tilePath), sTransform) as GameObject;

                //   go.name = mTileName + sList[i].indexOfLine[k];
                //  go.SetActive(true);

                //  Tile tile = go.GetComponent<Tile>();
                Tile tile = new Tile(pos + new Vector3(0, (Mathf.Abs(i - 1)) * 0.1f, 0)//根据地形微调一下坐标
                    , new Vector2(i + 1, k + 1)
                    , sTransform);

                //tile.SetTransform(pos, Quaternion.identity); //Quaternion.Euler(-45, 0, 0)
                //tile.TilePos = new Vector2(i + 1, k + 1);

                bool isDiscard = false;//是否是不能使用的地块
                for (int j = 0; j < discardTile.Count; j++)
                {
                    if (discardTile[j] == sList[i].indexOfLine[k])//废弃的地块
                    {
                        isDiscard = true;
                        break;
                    }
                }
                if (isDiscard)
                {
                    tile.SetDiscardState();
                }
                else
                {
                    tile.ResetTileInfo();
                    mTileDict.Add(sList[i].indexOfLine[k], tile);
                }
            }
        }
    }

    public Tile GetTile(int x, int y)//返回一个地块
    {
        int index = 0;
        if (x > sList.Length || y > sList[0].indexOfLine.Length || y < 1 || x < 1)
        {
            //Debug.Log("超出范围");
            return null;
        }
        index = sList[x - 1].indexOfLine[y - 1];

        for (int i = 0; i < discardTile.Count; i++)
        {
            if (index == discardTile[i])
            {
                return null;
            }
        }

        Tile tile = null;



        if (mTileDict.ContainsKey(index))
        {
            tile = mTileDict[index];
        }
        else
        {
            Debug.Log("not found the tile , index = " + index);
            // tile = sTransform.FindChild(mTileName + index).GetComponent<Tile>();
            // mTileDict.Add(index, tile);
        }
        return tile;
    }

    public void ResetAllTileInfo()//重置所有地块
    {
        foreach (Tile item in mTileDict.Values)
        {
            item.ResetTileInfo();
        }
    }

    public List<Tile> GetUnoccupied()//查找未被占据的地块
    {
        mUnoccupiedTileList.Clear();

        foreach (Tile item in mTileDict.Values)
        {
            if (!item.OccupyState)//没有被占据
            {
                mUnoccupiedTileList.Add(item);
            }
        }
        return mUnoccupiedTileList;
    }

    void OnDestroy()
    {
        mTileDict.Clear();
        mUnoccupiedTileList.Clear();
    }
}
