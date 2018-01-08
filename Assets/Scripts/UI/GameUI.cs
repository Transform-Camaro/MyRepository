using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using com.mob;
using System;

public class GameUI : MonoBehaviour
{

    // public Text mInvincibleText;
    // public Text mFrozenText;
    //public Text mHpText;
    // public Text mGameTimeText;
    // public Text mTimeText;//倒计时

    public Text mRecoringButtonText;//按钮显示文字

    public GameObject mExitPanel;//退出游戏的面板

    //  private float mTime = 0;//倒计时
    //  private bool isTime = false;//倒计时
    // private bool isStart = false;//开始游戏
    private bool isRecoring = false;//开始录屏

    //  public delegate void OnTimeEnd();
    // public event OnTimeEnd onTimeEnd;

    private string shareRECCallBackName = "Camera";
    private string shareRECRegisterKay = "1d16d9c262f7c";

    void Start()
    {
        RectTransform rt = GetComponent<RectTransform>();
        rt.sizeDelta = UIController.instance.ScreenSize;
        this.transform.localPosition = Vector3.zero;

        mExitPanel.SetActive(false);
    }


    public void OnGameOver()//游戏结束
    {
        this.gameObject.SetActive(false);
        //结束界面打开

       // Debug.Log("游戏结束,  结束界面打开 ");
        if (isRecoring)
        {
            StartOrStopRecoring();
            Debug.Log("正在录屏，退出录屏");
        }
    }

    public void OnExitPanelOpenAndClose(bool isopen)//打开和关闭退出按钮
    {
        if (GameManager.instance.GameStart)
        {
            mExitPanel.SetActive(isopen);

            GameManager.instance.Pause(!isopen);//暂停游戏时间
        }
    }

    public void OnExitButtonClick()//退出，回到初始界面
    {
        mExitPanel.SetActive(false);

        // GameManager.instance.ResetGameInfo();
        GameManager.instance.mEnemySpawner.ResetAllEnemyInfo();
        GameController.instance.ExitGame(GameStage.GameBegin);
       // GameController.instance.LoadUIStart(GameStage.GameBegin);
    }

    public void PrintScreenButton()
    {
        //Application.CaptureScreenshot("Screenshot",0);

        PrintScreen.CaptureCamera(GameController.instance.mainCamera, new Rect(0, 0, Screen.width, Screen.height));

    }

    public void OnMuteButtonClick()//静音，非静音按钮
    {
        GameController.instance.IsMute = !GameController.instance.IsMute;
    }


    public void OnVideoButtonClick()//录屏
    {
        ShareREC.setCallbackObjectName(shareRECCallBackName);
        ShareREC.registerApp(shareRECRegisterKay);

        StartOrStopRecoring();
    }

    public void StartOrStopRecoring()
    {
        if (!isRecoring)
        {
            ShareREC.startRecoring();
            mRecoringButtonText.text = "停   止";
            isRecoring = true;
        }
        else
        {
            FinishedRecordEvent evt = new FinishedRecordEvent(recordFinishedHandler);
            ShareREC.stopRecording(evt);
            mRecoringButtonText.text = "录   屏";
            isRecoring = false;
        }
    }

    private void recordFinishedHandler(Exception ex)
    {
        if (ex == null)
        {
            // ShareREC.playLastRecording();
        }
    }
}
