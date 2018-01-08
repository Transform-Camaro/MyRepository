using UnityEngine;

public class AssetsPaths
{
    //资源的本地路径根目录
    public static string LocalPath = "file://" + Application.streamingAssetsPath + "/AssetBundles";


    //图片打包目录
    public static string GameAtlasOutPath = Application.streamingAssetsPath + "/AssetBundles/atlas/main";


    public const string GameUIPath = "/ui/gameui";
    public const string GameLoadUIPath = "/ui/gameloadui";
    //public const string GameBeginUIPath = "/ui/gamebeginui";
    public const string GameUIFinishPath = "/ui/gamefinishui";
    public const string GameObjectsPath = "/go/gameobjects";
    // public const string GameBeginObjectsPath = "/go/gamebeginobjects";
    public const string GameEnemyPath = "/go/enemy";
    public const string GameTileAssetPath = "/go/tile";

    public const string GameSceneModel = "/go/gamescenemodel";
    public const string GameBeginStartBtn = "/go/gamebeginstartbtn";
    public const string GamePlayerModel = "/go/gameplayermodel";
    public const string GamePlayerBlood = "/go/gameplayerblood";
    public const string GameTime = "/go/gametime";
    public const string GameTimeCountDown = "/go/countdown";

    public const string CompanyLogo = "/go/companylogo";

    public const string GamePlayerBackPackEffect = "/effect/backpackeffect";
    public const string GameLightningEffect = "/effect/lightningeffect";
    public const string GameFireEffect = "/effect/fireeffect";
}
