using Photon.Pun;
using UnityEngine.UI;
using UnityEngine;

public class PCPauseMenuScreen : UIScreen
{
    public static string ID = "ui_pc_pause_menu";

    public Text pingText;
    public Text LogText;
    public Text timeText;

    // Update is called once per frame
    void Update()
    {
        pingText.text = "Ping: " + PhotonNetwork.GetPing();
        timeText.text = "Time: " + RoomManager.Instance.GetTimeCooldown();
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
        GameManager manager = GameManager.Instance;
        manager.SpawnEntityPlayer();
    }

    public void OnLeaveButtonPressed()
    {
        UIManager uiManager = UIManager.Instance;
        uiManager.CloseGUI();
        ClientNetworkManager manager = FindObjectOfType<ClientNetworkManager>();
        manager.Leave();
    }
}
