using Photon.Pun;
using UnityEngine;

public class NetworkObject : MonoBehaviourPun, IPunObservable
{
    // settings
    public bool syncPosition = false;
    public bool syncRotation = false;

    public Vector3 _realPosition;
    public Vector3 _positionAtLastPacket;
    public Quaternion _realRotation;

    public double currentTime = 0.0;
    public double currentPacketTime = 0.0;
    public double lastPacketTime = 0.0;
    public double timeToReachGoal = 0.0;

    public virtual void Update()
    {
        if (!photonView.IsMine)
        {
            // Object is remote
            timeToReachGoal = currentPacketTime - lastPacketTime;
            currentTime += Time.deltaTime;

            //Update Object position and Rigidbody parameters
            if (syncPosition)
            {
                gameObject.transform.position = Vector3.Lerp(_positionAtLastPacket, _realPosition, (float)(currentTime / timeToReachGoal));
            }
            if (syncRotation)
            {
                gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, _realRotation, (float)(currentTime / timeToReachGoal));
            }
        }
    }

    // Photon
    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //We own this object: send the others our data
            if (syncPosition)
            {
                stream.SendNext(gameObject.transform.position);
            }
            if (syncRotation)
            {
                stream.SendNext(gameObject.transform.rotation);
            }
        }
        else
        {
            currentTime = 0.0f;
            _positionAtLastPacket = gameObject.transform.position;
            if (syncPosition)
            {
                _realPosition = (Vector3)stream.ReceiveNext();
            }
            if (syncRotation)
            {
                _realRotation = (Quaternion)stream.ReceiveNext();
            }
            lastPacketTime = currentPacketTime;
            currentPacketTime = info.SentServerTime;
        }
    }
}
