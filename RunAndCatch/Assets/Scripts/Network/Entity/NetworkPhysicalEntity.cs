using Photon.Pun;
using UnityEngine;

public class NetworkPhysicalEntity : NetworkEntity
{
    private float m_Fraction;

    private PhysicalEntity GetEntity()
    {
        //cast to the actual type we need
        PhysicalEntity _pEntity = _entity as PhysicalEntity;
        if (_pEntity == null)
        {
            Debug.LogError("Entity is not a PhysicalEntity!");
            return null;
        }
        return _pEntity;
    }

    // override default behavior
    public override void ApplyInterpolation(double interpolationTime)
    {
        //float distance = Vector3.Distance(targetTempTransform.Position, transform.position);
        //Vector3 lerpPos = Vector3.Lerp(transform.position, targetTempTransform.Position, (float) (distance * interpolationTime));
        //GetEntity().transform.position = lerpPos;

        GetEntity().MoveEntity(targetTempTransform.Position);
        //transform.position = targetTempTransform.Position;
        GetEntity().RotateEntity(targetTempTransform.Rotation);
    }

    public override void ApplyExtrapolation(double interpolationTime)
    {
        //Vector3 velocity = targetTempTransform.Velocity;
        //Vector3 velocity = GetEntity().GetVelocity();
        Vector3 velocity = targetTempTransform.Position - m_bufferedStates[1].Position;

        float lag = (float)(PhotonNetwork.Time - targetTempTransform.Timestamp);
        Vector3 networkPosition = targetTempTransform.Position;
        networkPosition += (velocity * lag);

        float t = (lag * 10);
        m_Fraction = m_Fraction + Time.deltaTime * t;
        Vector3 lerpPos = Vector3.Lerp(transform.position, networkPosition, m_Fraction);
        GetEntity().transform.position = lerpPos;

        Quaternion networkRotation = targetTempTransform.Rotation;
        transform.rotation = networkRotation;
    }

    public override void OnWritePacket(PhotonStream stream, PhotonMessageInfo info)
    {
        base.OnWritePacket(stream, info);
        stream.SendNext(GetEntity().GetVelocity());
        stream.SendNext(GetEntity().GetAngularVelocity());
    }

    public override void OnReadPacket(PhotonStream stream, PhotonMessageInfo info)
    {
        base.OnReadPacket(stream, info);
        m_Fraction = 0;
    }

    public override NetworkTransform ReceiveNetworkState(PhotonStream stream, PhotonMessageInfo info, NetworkTransform newState)
    {
        newState = base.ReceiveNetworkState(stream, info, newState);

        Vector3 vel = (Vector3)stream.ReceiveNext();
        Vector3 angVel = (Vector3)stream.ReceiveNext();

        newState.Velocity = vel;
        newState.AngularVelocity = angVel;
        //GetEntity().SetVelocity(vel);

        return newState;
    }
}