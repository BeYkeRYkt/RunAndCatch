using Photon.Pun;
using UnityEngine.UI;
using UnityEngine;

public class PCPauseMenuScreen : UIScreen
{
    public static string ID = "ui_pc_pause_menu";

    public Text pingText;
    public Text LogText;

    void Start()
    {
        mId = "ui_pc_pause_menu";
    }

    // Update is called once per frame
    void Update()
    {
        pingText.text = "Ping: " + PhotonNetwork.GetPing();
    }

    public void DebugLog(string msg)
    {
        Debug.Log(msg);
        LogText.text += "\n";
        LogText.text += msg;
    }

    public void OnSpawnButtonPressed()
    {
        Hide();
        ClientNetworkManager manager = FindObjectOfType<ClientNetworkManager>();
        manager.SpawnEntityPlayer();
    }

    public void OnLeaveButtonPressed()
    {
        Hide();
        ClientNetworkManager manager = FindObjectOfType<ClientNetworkManager>();
        manager.Leave();
    }
}
