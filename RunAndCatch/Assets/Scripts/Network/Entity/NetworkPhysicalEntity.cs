using Photon.Pun;
using UnityEngine;

public class NetworkPhysicalEntity : NetworkEntity
{
    // settings
    public bool syncVelocity = false;
    public bool syncAngularVelocity = false;

    public Vector3 _realVelocity;
    public Vector3 _realAngularVelocity;

    public override void Update()
    {
        base.Update();

        if (!photonView.IsMine)
        {
            //cast to the actual type we need
            PhysicalEntity _pEntity = _entity as PhysicalEntity;
            if (_pEntity == null)
            {
                Debug.LogError("Entity is not a PhysicalEntity!");
                return;
            }

            if (syncVelocity)
            {
                _pEntity.SetVelocity(_realVelocity);
            }
            if (syncAngularVelocity)
            {
                _pEntity.SetAngularVelocity(_realAngularVelocity);
            }
        }
    }

    // Photon
    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        base.OnPhotonSerializeView(stream, info);

        if (stream.IsWriting)
        {
            //cast to the actual type we need
            PhysicalEntity _pEntity = _entity as PhysicalEntity;
            if (_pEntity == null)
            {
                Debug.LogError("Entity is not a PhysicalEntity!");
                return;
            }

            if (syncVelocity)
            {
                stream.SendNext(_pEntity.GetVelocity());
            }

            if (syncAngularVelocity)
            {
                stream.SendNext(_pEntity.GetAngularVelocity());
            }
        }
        else
        {
            if (syncVelocity)
            {
                _realVelocity = (Vector3)stream.ReceiveNext();
            }

            if (syncAngularVelocity)
            {
                _realAngularVelocity = (Vector3)stream.ReceiveNext();
            }
        }
    }
}
