
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClientNetworkManager : MonoBehaviourPunCallbacks
{
    private EntityManager entityManager;
    public Text LogText;

    // Start is called before the first frame update
    void Start()
    {
        // Get EntityManager
        entityManager = GetComponent<EntityManager>();
    }
    
    private void Log(string msg)
    {
        string debugId = MobilePauseMenuScreen.ID;
        UIScreen screen = UIManager.Instance.GetScreenById(debugId);
        if (screen != null)
        {
            MobilePauseMenuScreen pauseScreen = (MobilePauseMenuScreen) screen;
            pauseScreen.DebugLog(msg);
        }
    }

    public void Leave()
    {
        PhotonNetwork.LeaveRoom();
    }

    // Photon override
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Log(newPlayer.NickName + " has joined in game!");
        Log("Players: " + PhotonNetwork.CurrentRoom.PlayerCount);
        Log("Entity Players: " + entityManager.getPlayerEntities().Count);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Log(otherPlayer.NickName + " has leaved from game!");
        Log("Players: " + PhotonNetwork.CurrentRoom.PlayerCount);
        Log("Entity Players: " + entityManager.getPlayerEntities().Count);
    }
}
