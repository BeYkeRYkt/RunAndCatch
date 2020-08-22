using Photon.Pun;
using UnityEngine;

public class NetworkLivingEntity : NetworkPhysicalEntity
{
    private LivingEntity GetEntity()
    {
        //cast to the actual type we need
        LivingEntity _lEntity = _entity as LivingEntity;
        if (_lEntity == null)
        {
            Debug.LogError("Entity is not a LivingEntity!");
            return null;
        }
        return _lEntity;
    }

    public override void OnWritePacket(PhotonStream stream, PhotonMessageInfo info)
    {
        base.OnWritePacket(stream, info);
        stream.SendNext(GetEntity().GetMaxHealth());
        stream.SendNext(GetEntity().GetHealth());
    }

    public override void OnReadPacket(PhotonStream stream, PhotonMessageInfo info)
    {
        base.OnReadPacket(stream, info);
        GetEntity().SetMaxHealth((int)stream.ReceiveNext());
        GetEntity().SetHealth((int)stream.ReceiveNext());
    }
}
