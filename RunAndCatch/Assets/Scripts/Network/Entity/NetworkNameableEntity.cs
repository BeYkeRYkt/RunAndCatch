using Photon.Pun;
using UnityEngine;

public class NetworkNameableEntity : NetworkLivingEntity
{
    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        base.OnPhotonSerializeView(stream, info);

        //cast to the actual type we need
        NameableEntity _nEntity = _entity as NameableEntity;
        if (_nEntity == null)
        {
            Debug.LogError("Entity is not a NameableEntity!");
            return;
        }

        if (stream.IsWriting)
        {
            stream.SendNext(_nEntity.GetDisplayName());
        }
        else
        {
            _nEntity.SetDisplayName((string)stream.ReceiveNext());
        }
    }
}
