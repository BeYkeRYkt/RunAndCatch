
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class ClientLobbyManager : MonoBehaviourPunCallbacks
{
    public Text LogText;
    private bool isConnected = false;
    public string levelName;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.NickName = "default" + Random.Range(0, 100);
        Log("My name is " + PhotonNetwork.NickName);

        PhotonNetwork.GameVersion = "1";
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Log("Connected to master!");
        isConnected = true;
    }

    public void CreateRoom()
    {
        if (isConnected)
        {
            PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 5 });
        }
    }

    public void JoinRoom()
    {
        if (isConnected)
        {
            Hashtable table = new Hashtable();
            table.Add("skin", "0");
            PhotonNetwork.LocalPlayer.SetCustomProperties(table);
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnJoinedRoom()
    {
        Log("Joined the room");
        PhotonNetwork.LoadLevel(levelName);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Log("Error code: " + returnCode + " msg: " + message);
        CreateRoom();
    }

    private void Log(string msg)
    {
        Debug.Log(msg);
        LogText.text += "\n";
        LogText.text += msg;
    }
}
