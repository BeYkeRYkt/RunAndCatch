using UnityEngine.UI;
using Photon.Pun;
using UnityEngine;
using System;

public class MobileGameplayScreen : UIScreen, IGameStateListener
{
    public static string ID = "ui_mobile_gameplay_screen";

    // ping text
    public Text pingText;

    // timer
    // icon sprite for timer
    public Sprite ic_timer;

    // timer bar
    public Image timerBar;
    public Image timerBarIcon;
    public Color disabledColor;
    public Color enabledColor;

    // player count
    public Text playersCount;

    // message box
    public Text messageBox;
    private CooldownTimer cooldownTimer;
    public float cooldownTimeInSeconds = 5; // in secs

    // joystick
    public VariableJoystick joystick;

    public void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        // update timer message box
        cooldownTimer.Update(Time.deltaTime);

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

        // set timer
        cooldownTimer = new CooldownTimer(cooldownTimeInSeconds);
        cooldownTimer.TimerCompleteEvent += OnTimerComplete;

        messageBox.gameObject.SetActive(false);

        GameManager.Instance.RegisterListener(this);
    }

    public override void OnShowScreen()
    {
        base.OnShowScreen();
        UpdateUI();
    }

    void OnTimerComplete()
    {
        messageBox.gameObject.SetActive(false);
    }

    public void OnPauseButtonPress()
    {
        //Hide();
        GameManager manager = GameManager.Instance;
        manager.PauseGame();
    }

    public void OnGameStarted()
    {
        GameManager manager = GameManager.Instance;
        if (manager.playerRole == PlayerRole.VICTIM)
        {
            //messageBox.text = "You are the victim! Keep as long as possible from the hunter!";
            messageBox.text = LanguageManager.Instance.langReader.getString("ui_gameplay_victim_text");
        }
        else if (manager.playerRole == PlayerRole.HUNTER)
        {
            //messageBox.text = "You are a hunter! Catch the rest of the players!";
            messageBox.text = LanguageManager.Instance.langReader.getString("ui_gameplay_hunter_text");
        }
        else
        {
            messageBox.text = "";
        }

        messageBox.gameObject.SetActive(true);
        cooldownTimer.Start();
    }

    public void OnGameStopped()
    {
        joystick.ResetInput();
        //throw new System.NotImplementedException();
    }

    public void OnGamePaused()
    {
        //throw new System.NotImplementedException();
    }

    public void OnGameUnpaused()
    {
        //throw new System.NotImplementedException();
    }

    public void OnGameOver()
    {
        joystick.ResetInput();
        //throw new System.NotImplementedException();
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
}
