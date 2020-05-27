using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class NetworkEntity_old : MonoBehaviourPunCallbacks, IPunObservable
{
    // settings
    public bool syncPosition = false;
    public bool syncRotation = false;
    public bool syncVelocity = false;
    public bool syncAngularVelocity = false;

    public Vector3 _realPosition;
    public Vector3 _positionAtLastPacket;
    public Quaternion _realRotation;

    public Vector3 _realVelocity;
    public Vector3 _realAngularVelocity;

    public double currentTime = 0.0;
    public double currentPacketTime = 0.0;
    public double lastPacketTime = 0.0;
    public double timeToReachGoal = 0.0;
    public Entity _entity;

    void Update()
    {
        if (!photonView.IsMine)
        {
            timeToReachGoal = currentPacketTime - lastPacketTime;
            currentTime += Time.deltaTime;

            //Update Entity position and Rigidbody parameters
            //transform.position = Vector3.Lerp(_positionAtLastPacket, _realPosition, (float)(currentTime / timeToReachGoal));
            if (syncPosition)
            {
                _entity.SetPosition(Vector3.Lerp(_positionAtLastPacket, _realPosition, (float)(currentTime / timeToReachGoal)));
            }
            //transform.rotation = Quaternion.RotateTowards(_entity.GetRotation(), _realRotation, (float)(currentTime / timeToReachGoal));
            if (syncRotation)
            {
                _entity.SetRotation(Quaternion.Lerp(_entity.GetRotation(), _realRotation, (float)(currentTime / timeToReachGoal)));
            }
            if(syncVelocity)
            {
                //_entity.SetVelocity(_realVelocity);
            }
            if(syncAngularVelocity)
            {
                //_entity.SetAngularVelocity(_realAngularVelocity);
            }
        }
    }

    // Photon
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //We own this entity: send the others our data
            if (syncPosition)
            {
                stream.SendNext(_entity.GetPosition());
            }

            if (syncRotation)
            {
                stream.SendNext(_entity.GetRotation());
            }

            if (syncVelocity)
            {
                //stream.SendNext(_entity.GetVelocity());
            }

            if(syncAngularVelocity)
            {
                //stream.SendNext(_entity.GetAngularVelocity());
            }
        }
        else
        {
            currentTime = 0.0f;
            _positionAtLastPacket = _entity.GetPosition();
            if (syncPosition)
            {
                _realPosition = (Vector3)stream.ReceiveNext();
            }

            if (syncRotation)
            {
                _realRotation = (Quaternion)stream.ReceiveNext();
            }

            if (syncVelocity)
            {
                _realVelocity = (Vector3)stream.ReceiveNext();
            }

            if(syncAngularVelocity)
            {
                _realAngularVelocity = (Vector3)stream.ReceiveNext();
            }

            lastPacketTime = currentPacketTime;
            currentPacketTime = info.SentServerTime;
        }
    }

    void OnCollisionEnter(Collision contact)
    {
        if (!photonView.IsMine)
        {
            Transform collisionObjectRoot = contact.transform.root;
            if (collisionObjectRoot.CompareTag("Player"))
            {
                //Transfer PhotonView of Rigidbody to our local player
                photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            }
        }
    }
}