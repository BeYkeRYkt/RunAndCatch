using UnityEngine.UI;
using UnityEngine;

public class MobileGameplayScreen : UIScreen, IGameStateListener
{
    public static string ID = "ui_mobile_gameplay_screen";

    public Text roundTimer;

    public Text messageBox;
    private CooldownTimer cooldownTimer;
    public float cooldownTimeInSeconds = 5; // in secs
    public VariableJoystick joystick;

    // Update is called once per frame
    void Update()
    {
        roundTimer.text = "Time: " + GameRoomManager.Instance.GetTimeCooldown();

        cooldownTimer.Update(Time.deltaTime);
    }

    public override void Initialize()
    {
        mId = ID;
        cooldownTimer = new CooldownTimer(cooldownTimeInSeconds);
        cooldownTimer.TimerCompleteEvent += OnTimerComplete;

        messageBox.gameObject.SetActive(false);

        ClientGameManager.Instance.RegisterListener(this);
    }

    void OnTimerComplete()
    {
        messageBox.gameObject.SetActive(false);
    }

    public void OnPauseButtonPress()
    {
        //Hide();
        ClientGameManager manager = ClientGameManager.Instance;
        manager.PauseGame();
    }

    public void OnGameStarted()
    {
        ClientGameManager manager = ClientGameManager.Instance;
        if (manager.playerRole == PlayerRole.VICTIM)
        {
            messageBox.text = "You are the victim! Keep as long as possible from the hunter!";
        }
        else
        {
            messageBox.text = "You are a hunter! Catch the rest of the players!";
        }

        messageBox.gameObject.SetActive(true);
        cooldownTimer.Start();
    }

    public void OnGameStopped()
    {
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
        //throw new System.NotImplementedException();
    }
}
