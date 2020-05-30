using UnityEngine.UI;
using UnityEngine;

public class PCGameplayScreen : UIScreen
{
    public static string ID = "ui_pc_gameplay_screen";

    public Text messageBox;
    private CooldownTimer cooldownTimer;
    public float cooldownTimeInSeconds = 3; // in secs

    void Start()
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
        cooldownTimer.Start();
    }

    // Update is called once per frame
    void Update()
    {
        cooldownTimer.Update(Time.deltaTime);
    }

    public override void Initialize()
    {
        mId = ID;
        cooldownTimer = new CooldownTimer(cooldownTimeInSeconds);
        cooldownTimer.TimerCompleteEvent += OnTimerComplete;
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
}
