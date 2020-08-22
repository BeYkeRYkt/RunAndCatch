using UnityEngine;

// This class holds transform values and can load and send the data to server
public class NetworkTransform
{
    public double Timestamp;
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Velocity;
    public Vector3 AngularVelocity;

    public NetworkTransform() { }

    public static NetworkTransform Lerp(NetworkTransform targetTempTransform, NetworkTransform startPoint, NetworkTransform endPoint, float t)
    {
        targetTempTransform.Position = Vector3.Lerp(startPoint.Position, endPoint.Position, t);
        targetTempTransform.Rotation = Quaternion.Slerp(startPoint.Rotation, endPoint.Rotation, t);
        targetTempTransform.Velocity = Vector3.Lerp(startPoint.Velocity, endPoint.Velocity, t);
        targetTempTransform.AngularVelocity = Vector3.Lerp(startPoint.AngularVelocity, endPoint.AngularVelocity, t);

        targetTempTransform.Timestamp = Mathf.Lerp((float)startPoint.Timestamp, (float)endPoint.Timestamp, t);

        return targetTempTransform;
    }

    // Copies another NetworkTransform to itself
    public void Load(NetworkTransform ntransform)
    {
        Position = ntransform.Position;
        Rotation = ntransform.Rotation;
        Velocity = ntransform.Velocity;
        AngularVelocity = ntransform.AngularVelocity;
        Timestamp = ntransform.Timestamp;
    }

    public void ResetValues()
    {
        Timestamp = 0;
        Position = Vector3.zero;
        Rotation = Quaternion.identity;
        Velocity = Vector3.zero;
        AngularVelocity = Vector3.zero;
    }

    // Clone itself
    public static NetworkTransform Clone(NetworkTransform ntransform)
    {
        NetworkTransform trans = new NetworkTransform();
        trans.Load(ntransform);
        return trans;
    }
}
