
using Photon.Pun;
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
    
    public void Leave()
    {
        PhotonNetwork.LeaveRoom();
    }

    // Photon override
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
