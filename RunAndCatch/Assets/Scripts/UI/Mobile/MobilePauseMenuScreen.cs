using Photon.Pun;
using UnityEngine.UI;
using UnityEngine;

public class MobilePauseMenuScreen : UIScreen
{
    public static string ID = "ui_mobile_pause_menu";

    public Text pingText;
    public Text LogText;
    public Text timeText;

    // Update is called once per frame
    void Update()
    {
        pingText.text = "Ping: " + PhotonNetwork.GetPing();
        timeText.text = "Time: " + GameRoomManager.Instance.GetTimeCooldown();
    }

    public override void Initialize()
    {
        mId = ID;
    }

    public void DebugLog(string msg)
    {
        Debug.Log(msg);
        LogText.text += "\n";
        LogText.text += msg;
    }

    public void OnSpawnButtonPressed()
    {
        UIManager uiManager = UIManager.Instance;
        uiManager.CloseGUI();
        ClientGameManager manager = ClientGameManager.Instance;
        manager.SpawnEntityPlayer();
        manager.UnpauseGame();
        //uiManager.OpenGUI(MobileGameplayScreen.ID);
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

        ClientGameManager manager = ClientGameManager.Instance;
        manager.UnpauseGame();
    }
}
