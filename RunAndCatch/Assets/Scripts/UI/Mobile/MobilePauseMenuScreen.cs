using Photon.Pun;
using UnityEngine.UI;
using UnityEngine;
using System;

public class MobilePauseMenuScreen : UIScreen
{
    public static string ID = "ui_mobile_pause_menu";

    // ping text
    public Text pingText;

    // pause text
    public Text pauseText;

    // continue text
    public Text continueText;

    // disconnect text
    public Text disconnectText;

    // timer bar
    public Image timerBar;
    public Color disabledColor;
    public Color enabledColor;

    // player count
    public Text playersCount;

    public void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        // update ping text
        if (ClientLobbyManager.Instance.IsConnected())
        {
            pingText.text = LanguageManager.Instance.langReader.getString("ui_ping_text") + ": " + PhotonNetwork.GetPing() + " ms";
        }

        // update players
        playersCount.text = Convert.ToString(EntityManager.Instance.GetPlayerEntities().Count);

        // update timer bar
        GameManager gameManager = GameManager.Instance;
        float value = gameManager.GetTimeCooldown() / (float)gameManager.roundTime * 100f;
        SetCircleValue(value);
    }

    public override void Initialize()
    {
        mId = ID;

        // update ping text
        if (ClientLobbyManager.Instance.IsConnected())
        {
            pingText.text = LanguageManager.Instance.langReader.getString("ui_ping_text") + ": " + PhotonNetwork.GetPing() + " ms";
        }

        // update color
        if (RoomManager.Instance.state == GameRoomState.RUNNING)
        {
            timerBar.color = enabledColor;
        }

        // update pause text
        pauseText.text = LanguageManager.Instance.langReader.getString("ui_pause_menu_pause_text").ToUpper();

        // update continue button text
        continueText.text = LanguageManager.Instance.langReader.getString("ui_pause_menu_continue_text").ToUpper();

        // update disconnect button text
        disconnectText.text = LanguageManager.Instance.langReader.getString("ui_pause_menu_disconnect_text").ToUpper();
    }

    public override void OnShowScreen()
    {
        base.OnShowScreen();
        UpdateUI();
    }

    public void SetCircleValue(float currentValue)
    {
        if (currentValue > 100)
        {
            currentValue = 100;
        }
        timerBar.fillAmount = currentValue / 100;
    }

    public float GetCircleValue()
    {
        return timerBar.fillAmount * 100;
    }

    public void OnLeaveButtonPressed()
    {
        UIManager uiManager = UIManager.Instance;
        uiManager.CloseGUI();
        ClientNetworkManager manager = FindObjectOfType<ClientNetworkManager>();
        manager.Leave();
    }

    public void OnPauseButtonPressed()
    {
        //UIManager uiManager = UIManager.Instance;
        //uiManager.CloseGUI();
        //uiManager.OpenGUI(MobileGameplayScreen.ID);

        GameManager manager = GameManager.Instance;
        manager.UnpauseGame();
    }
}
