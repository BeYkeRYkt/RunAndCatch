using Photon.Pun;
using UnityEngine;

public class NetworkEntityPlayer : NetworkNameableEntity
{
    public void Start()
    {
        //Player is local
        if (photonView.IsMine)
        {
            gameObject.tag = "Player";
        } else
        {
            Rigidbody body = GetComponent<Rigidbody>();
            body.isKinematic = true;
        }
    }

    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        base.OnPhotonSerializeView(stream, info);
    }

    public void OnDeathEntity()
    {
        // tell game manager
        if (photonView.IsMine)
        {
            GameObject managerObject = GameObject.Find("GameManager");
            GameManager manager = managerObject.GetComponent<GameManager>();
            manager.onPlayerDeath();
        }
    }
}
