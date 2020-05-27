
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ClientNetworkManager : MonoBehaviourPunCallbacks
{
    private ClientGameManager gameManager;
    private EntityManager entityManager;
    //public GameObject playerPrefab;
    public List<GameObject> playerPrefabs = new List<GameObject>();
    public Text LogText;

    // Start is called before the first frame update
    void Start()
    {
        // Get GameManager
        gameManager = GetComponent<ClientGameManager>();
        entityManager = GetComponent<EntityManager>();
    }

    public void SpawnEntityPlayer()
    {
        int index = Random.Range(0, playerPrefabs.Count);
        GameObject prefab = playerPrefabs[index];
        SpawnEntityPlayer(prefab);
    }

    private void SpawnEntityPlayer(GameObject prefab)
    {
        // kill another player
        if(gameManager.playerObject != null)
        {
            GameObject playerObject = gameManager.playerObject;
            EntityPlayer ep = playerObject.GetComponent<EntityPlayer>();
            ep.KillEntity();
        }

        // spawn player
        Vector3 pos = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
        gameManager.playerObject = PhotonNetwork.Instantiate(prefab.name, pos, Quaternion.identity);
        Log("Created new prefab for " + PhotonNetwork.NickName);

        // set nickname
        EntityPlayer player = gameManager.playerObject.GetComponent<EntityPlayer>();
        player.SetDisplayName(PhotonNetwork.NickName);

        // set camera for prefab
        CharacterMovement mvnt = gameManager.playerObject.GetComponent<CharacterMovement>();
        mvnt.cameraObject = gameManager.cameraObject.transform.parent.gameObject;
        gameManager.cameraObject.transform.parent.parent = gameManager.playerObject.transform;
        gameManager.cameraObject.transform.parent.transform.localPosition = new Vector3(0f, 5, -4);

        // talk to camera follow game object
        //CameraFollower flwr = gameManager.cameraObject.GetComponent<CameraFollower>();
        //flwr.target = gameManager.playerObject;

        // disable canvas
        gameManager.UnpauseGame();
    }

    private void Log(string msg)
    {
        string debugId = PCPauseMenuScreen.ID;
        UIScreen screen = UIManager.Instance.GetScreenById(debugId);
        if (screen != null)
        {
            PCPauseMenuScreen pauseScreen = (PCPauseMenuScreen) screen;
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
