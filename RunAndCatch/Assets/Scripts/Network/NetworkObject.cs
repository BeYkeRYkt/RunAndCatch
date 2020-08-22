using Photon.Pun;
using UnityEngine;

public class NetworkObject : MonoBehaviourPun, IPunObservable
{
    public enum InterpolationMode
    {
        INTERPOLATION,
        EXTRAPOLATION
    }
    public InterpolationMode mode = InterpolationMode.EXTRAPOLATION;

    protected float InterpolationBackTime = 0.12f; //Default 0.1, one tenth of a second
    protected float maxInterpolationBackTime = 0.7f;

    protected float ExtrapolationLimit = 0.5f;

    // uses for lerp
    protected NetworkTransform targetTempTransform;

    // We store twenty states with "playback" information
    protected NetworkTransform[] m_bufferedStates;
    // Keep track of what slots are used
    private int m_stateCount = 0;

    private Vector3 currentPos = Vector3.zero;

    private void Awake()
    {
        targetTempTransform = new NetworkTransform();
        int calculatedStateBufferSize = ((int)(PhotonNetwork.SerializationRate * InterpolationBackTime) + 1) * 2;
        m_bufferedStates = new NetworkTransform[calculatedStateBufferSize];
        //InterpolationBackTime = (1.0f / PhotonNetwork.SerializationRate); // 100 ms
    }

    public virtual void Update()
    {
        if (photonView.IsMine || m_stateCount == 0) return;

        double currentTime = PhotonNetwork.Time;
        double packetDelay = (float)(currentTime - m_bufferedStates[0].Timestamp);
        //Debug.Log("InterpolationBackTime: " + InterpolationBackTime);
        //Debug.Log("PacketDelay: " + packetDelay);
        double interpolationTime = currentTime - InterpolationBackTime;

        if (mode == InterpolationMode.INTERPOLATION && m_bufferedStates[0].Timestamp > interpolationTime)
        {
            if (Interpolation(interpolationTime))
            {
                //Debug.Log("ApplyInterpolation");
                ApplyInterpolation(interpolationTime);
            }
        }
        else
        {
            if (Extrapolation(interpolationTime))
            {
                //Debug.Log("ApplyExtrapolation");
                ApplyExtrapolation(interpolationTime);
            }
            else
            {
                //Debug.Log("Teleporting!");
                transform.position = m_bufferedStates[0].Position;
                //transform.position = Vector3.Lerp(transform.localPosition, m_bufferedStates[0].Position, Time.deltaTime * 20);
                transform.rotation = m_bufferedStates[0].Rotation;
            }
        }
    }

    private bool Interpolation(double interpolationTime)
    {
        for (var i = 0; i < m_stateCount; i++)
        {
            //closest state that matches network Time or use oldest state
            if (m_bufferedStates[i].Timestamp <= interpolationTime || i == m_stateCount - 1)
            {
                //closest to Network
                NetworkTransform startPoint = m_bufferedStates[i];
                //one newer
                NetworkTransform endPoint = m_bufferedStates[Mathf.Max(i - 1, 0)];
                //time between
                double length = endPoint.Timestamp - startPoint.Timestamp;

                var t = 0.0f;
                if (length > 0.0001f)
                {
                    t = (float)((interpolationTime - startPoint.Timestamp) / length);
                }
                targetTempTransform = NetworkTransform.Lerp(targetTempTransform, startPoint, endPoint, t);
                return true;
            }
        }
        return false;
    }

    public virtual void ApplyInterpolation(double interpolationTime)
    {
        transform.position = targetTempTransform.Position;
        transform.rotation = targetTempTransform.Rotation;
    }

    private bool Extrapolation(double interpolationTime)
    {
        targetTempTransform = m_bufferedStates[0];
        double extrapolationLength = interpolationTime - targetTempTransform.Timestamp;
        if (extrapolationLength < ExtrapolationLimit & m_stateCount > 1)
        {
            return true;
        }
        return false;
    }

    public virtual void ApplyExtrapolation(double interpolationTime)
    {
        transform.position = targetTempTransform.Position;
        transform.rotation = targetTempTransform.Rotation;
    }

    public virtual void OnWritePacket(PhotonStream stream, PhotonMessageInfo info) { }

    public virtual void OnReadPacket(PhotonStream stream, PhotonMessageInfo info) { }

    // Photon
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //We own this object: send the others our data
            Vector3 pos = transform.position;
            Quaternion rot = transform.rotation;
            stream.SendNext(pos);
            stream.SendNext(rot);
            OnWritePacket(stream, info);
        }
        else
        {
            // Receive data
            NetworkTransform newTransform = new NetworkTransform();
            newTransform = ReceiveNetworkState(stream, info, newTransform);
            AddState(newTransform);

            // Check integrity, lowest numbered state in the buffer is newest and so on
            for (int i = 0; i < m_stateCount - 1; i++)
            {
                if (m_bufferedStates[i].Timestamp < m_bufferedStates[i + 1].Timestamp)
                {
                    Debug.Log("State inconsistent. fixing...");

                    var temp = m_bufferedStates[i + 1];
                    m_bufferedStates[i + 1] = m_bufferedStates[i];
                    m_bufferedStates[i] = temp;
                }
            }

            OnReadPacket(stream, info);
        }
    }

    public virtual NetworkTransform ReceiveNetworkState(PhotonStream stream, PhotonMessageInfo info, NetworkTransform newTransform)
    {
        double timeStamp = info.SentServerTime;
        Vector3 pos = (Vector3) stream.ReceiveNext();
        Quaternion rot = (Quaternion) stream.ReceiveNext();

        newTransform.Position = pos;
        newTransform.Rotation = rot;
        newTransform.Velocity = Vector3.zero;
        newTransform.AngularVelocity = Vector3.zero;
        newTransform.Timestamp = timeStamp;

        return newTransform;
    }

    private void AddState(NetworkTransform newTransform)
    {
        // Discard if tranform is out of sequence (Possible with UDP)
        if (m_stateCount > 1 && newTransform.Timestamp <= m_bufferedStates[0].Timestamp)
        {
            Debug.Log("Received network transform out of order for: " + gameObject.name);
            return;
        }

        // Shift buffer contents, oldest data erased, 18 becomes 19, ... , 0 becomes 1
        for (int i = m_bufferedStates.Length - 1; i >= 1; i--)
        {
            m_bufferedStates[i] = m_bufferedStates[i - 1];
        }

        //Newest State
        m_bufferedStates[0] = newTransform;

        m_stateCount = Mathf.Min(m_stateCount + 1, m_bufferedStates.Length);
    }
}