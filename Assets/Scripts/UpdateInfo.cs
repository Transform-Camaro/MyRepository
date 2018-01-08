using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class UpdateInfo : MonoBehaviour
{
    GameConfig config;
    public InputField munitTime;

    public InputField mtimeOfOneGrid;

    public Toggle mplyaerIsInvincible;

    GUIStyle guistyle = new GUIStyle();

    void Start()
    {
        config = GameController.instance.gameConfig;

        munitTime.contentType = InputField.ContentType.DecimalNumber;
        mtimeOfOneGrid.contentType = InputField.ContentType.DecimalNumber;

        munitTime.text = config.UNIT_TIME.ToString();
        mtimeOfOneGrid.text = config.playerInfo.timeOfOneGrid.ToString();

        mplyaerIsInvincible.isOn = config.playerInfo.isInvincible;

        guistyle.fontSize = 40;
        guistyle.normal.textColor = Color.white;
    }

    public void OnUnitTimeChange()
    {
        if (munitTime.text.Length == 0)
        {
            munitTime.text = config.UNIT_TIME.ToString();
            return;
        }
        float value = float.Parse(munitTime.text);
        if (value <= 0)
        {
            munitTime.text = config.UNIT_TIME.ToString();
        }
        else
        {
            config.UNIT_TIME = value;
        }
    }
    public void OnTimeOfOneGridChange()
    {
        if (mtimeOfOneGrid.text.Length == 0)
        {
            mtimeOfOneGrid.text = config.playerInfo.timeOfOneGrid.ToString();
            return;
        }

        float value = float.Parse(mtimeOfOneGrid.text);
        if (value <= 0)
        {
            mtimeOfOneGrid.text = config.playerInfo.timeOfOneGrid.ToString();
        }
        else
        {
            config.playerInfo.timeOfOneGrid = value;
        }
    }

    public void OnPlayerIsInvincibleChange()
    {
        config.playerInfo.isInvincible = mplyaerIsInvincible.isOn;
    }
}
