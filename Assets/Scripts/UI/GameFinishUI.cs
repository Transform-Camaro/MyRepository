using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameFinishUI : MonoBehaviour
{
    public Text mScoreText;

    void Awake()
    {
       // mScoreText = transform.GetComponentInChildren<Text>();
        mScoreText.text = "";

        RectTransform rt = GetComponent<RectTransform>();
        rt.sizeDelta = UIController.instance.ScreenSize;

        this.transform.localPosition = Vector3.zero;
    }

    public void SetScore(float time)
    {
        mScoreText.text = "得分 : " + (int)(time * 100);
    }

    public void OnGameAgainButtonClick()
    {
        //直接重置游戏并且开始
        //GameManager.instance.ResetGameInfo();//重置游戏内容
        //GameManager.instance.StartGame();


        //重新开始重新场景
        GameManager.instance.mEnemySpawner.ResetAllEnemyInfo();
        //  GameController.instance.LoadUIStart(GameStage.Game);
        GameController.instance.ExitGame(GameStage.Game);
    }

    public void OnExitButtonClick()//退出，回到初始界面
    {
        GameManager.instance.mEnemySpawner.ResetAllEnemyInfo();

      //  GameController.instance.LoadUIStart(GameStage.GameBegin);
        GameController.instance.ExitGame(GameStage.GameBegin);
    }

    public void OnShareButtonClick()//分享
    {

    }
}
