using Photon.Pun;

public class NetworkEntity : NetworkObject
{
    public Entity _entity;

    public void KillEntity()
    {
        photonView.RPC("KillEntityRPC", RpcTarget.Others);
        if(photonView.InstantiationId >= 1000)
        {
            // Destroy non-scene entity
            PhotonNetwork.Destroy(gameObject);
        }
    }

    [PunRPC]
    public void KillEntityRPC()
    {
        _entity.KillEntityRPC();
    }
}
