
using System;
using UnityEngine;
using UnityEngine.UI;

public class MobileConnectToServerScreen : UIScreen
{
    public static string ID = "ui_mobile_connect_to_servers_menu";

    // cloud image
    public Image mImage;

    // status message
    public Text mStatusText;

    // reconnect message
    public Text mReconnectText;

    // timer text
    public Text mTimerText;

    // icon cloud sprite
    public Sprite ic_cloud;

    // icon cloud off sprite
    public Sprite ic_cloud_off;

    public override void Initialize()
    {
        mId = ID;
        showConnectingState();
    }

    public void showConnectingState()
    {
        mImage.sprite = ic_cloud;

        mStatusText.text = LanguageManager.Instance.langReader.getString("ui_connecting_screen_connecting_process_text");

        if(mReconnectText.gameObject.activeSelf)
        {
            mReconnectText.gameObject.SetActive(false);
        }

        if (mTimerText.gameObject.activeSelf)
        {
            mTimerText.gameObject.SetActive(false);
        }
    }

    public void showFailedState()
    {
        mImage.sprite = ic_cloud_off;

        mStatusText.text = LanguageManager.Instance.langReader.getString("ui_connecting_screen_servers_is_not_available_text");

        if (!mReconnectText.gameObject.activeSelf)
        {
            mReconnectText.text = LanguageManager.Instance.langReader.getString("ui_connecting_screen_restart_connecting_text");
            mReconnectText.gameObject.SetActive(true);
        }

        if (!mTimerText.gameObject.activeSelf)
        {
            mTimerText.gameObject.SetActive(true);
        }
    }

    public void SetTime(int seconds)
    {
        mTimerText.text = Convert.ToString(seconds);
    }
}
