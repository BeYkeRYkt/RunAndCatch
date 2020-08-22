
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
            //body.isKinematic = true;
        }
    }

    public void OnEntityDeath()
    {
        // tell game manager
        GameManager manager = GameManager.Instance;
        if (photonView.IsMine)
        {
            manager.OnLocalPlayerDeath();
        } else
        {
            manager.OnRemotePlayerDeath();
        }
    }
}
