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
            //if (PhotonNetwork.IsMasterClient)
            //{
                if(photonView.IsMine)
                {
                    PhotonNetwork.Destroy(gameObject);
                }
            //}
        }
    }

    [PunRPC]
    public void KillEntityRPC()
    {
        _entity.KillEntityRPC();
        //if (PhotonNetwork.IsMasterClient)
        //{
            // Destroy non-scene entity
            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        //}
    }
}
