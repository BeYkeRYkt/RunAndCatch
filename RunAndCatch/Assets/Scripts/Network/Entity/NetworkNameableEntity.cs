using Photon.Pun;
using UnityEngine;

public class NetworkNameableEntity : NetworkLivingEntity
{
    private NameableEntity GetEntity()
    {
        //cast to the actual type we need
        NameableEntity _nEntity = _entity as NameableEntity;
        if (_nEntity == null)
        {
            Debug.LogError("Entity is not a NameableEntity!");
            return null;
        }
        return _nEntity;
    }

    public override void OnWritePacket(PhotonStream stream, PhotonMessageInfo info)
    {
        base.OnWritePacket(stream, info);
        stream.SendNext(GetEntity().GetDisplayName());
    }

    public override void OnReadPacket(PhotonStream stream, PhotonMessageInfo info)
    {
        base.OnReadPacket(stream, info);
        GetEntity().SetDisplayName((string)stream.ReceiveNext());
    }
}
