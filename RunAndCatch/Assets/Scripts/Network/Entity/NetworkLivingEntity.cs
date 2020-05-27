using Photon.Pun;
using UnityEngine;

public class NetworkLivingEntity : NetworkPhysicalEntity
{
    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        base.OnPhotonSerializeView(stream, info);

        //cast to the actual type we need
        LivingEntity _lEntity = _entity as LivingEntity;
        if (_lEntity == null)
        {
            Debug.LogError("Entity is not a LivingEntity!");
            return;
        }

        if (stream.IsWriting)
        {
            stream.SendNext(_lEntity.GetMaxHealth());
            stream.SendNext(_lEntity.GetHealth());
        }
        else
        {
            _lEntity.SetMaxHealth((int)stream.ReceiveNext());
            _lEntity.SetHealth((int)stream.ReceiveNext());
        }
    }
}
