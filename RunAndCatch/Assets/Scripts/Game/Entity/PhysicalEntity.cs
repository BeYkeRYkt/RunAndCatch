using UnityEngine;

public class PhysicalEntity : Entity
{
    // Physics
    public Rigidbody rigidBody;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public bool checkCollide(Vector3 direction)
    {
        RaycastHit hit;
        float distance = 0.1f;
        if (rigidBody.SweepTest(direction, out hit, distance))
        {
            Debug.DrawLine(rigidBody.position, hit.point, Color.white, 2.5f);
            return true;
        }
        return false;
    }

    public void ApplyVelocity(Vector3 velocity)
    {
        rigidBody.velocity += velocity;
    }

    public void SetVelocity(Vector3 velocity)
    {
        rigidBody.velocity = velocity;
    }

    public Vector3 GetVelocity()
    {
        return rigidBody.velocity;
    }

    public void SetAngularVelocity(Vector3 velocity)
    {
        rigidBody.angularVelocity = velocity;
    }

    public Vector3 GetAngularVelocity()
    {
        return rigidBody.angularVelocity;
    }

    /**
    * Only XZ
    */
    public void MoveEntityXZ(Vector3 direction)
    {
        // do not apply new y
        Vector3 newVector = new Vector3(direction.x, rigidBody.velocity.y, direction.z);
        SetVelocity(newVector);
    }

    public void MoveEntity(Vector3 direction)
    {
        rigidBody.MovePosition(direction);
    }

    public void RotateEntity(Quaternion direction)
    {
        rigidBody.MoveRotation(direction.normalized);
    }
}
