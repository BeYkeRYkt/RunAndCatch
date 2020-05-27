using UnityEngine;

public class move : MonoBehaviour
{
    public float speed = 12f;

    Vector3 movement;
    public Collider col;
    Collider playerCollider;
    Rigidbody playerRigidbody;
    int floorMask;
    public float friction = 0.95f;



    public VariableJoystick variableJoystick;



    //public float slideSpeed  = 5; // slide speed
    // public bool isSliding  = false;
    // Vector3 slideForward ; // direction of slide
    //public  float sliderTimer= 0.0f;
    //public float slideTimerMax  = 2.5f; // time while sliding

    void Awake()
    {
        // Create a layer mask for the floor layer.
        floorMask = LayerMask.GetMask("ICE");
        playerRigidbody = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();
    }


    void FixedUpdate()
    {

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        slide(h, v, playerCollider);

    }

    void slide(float h, float v, Collider other)

    {
        Vector3 direction = Vector3.forward * variableJoystick.Vertical + Vector3.right * variableJoystick.Horizontal;
        // Set the movement vector based on the axis input.
        movement.Set(h, 0f, v);

        // Normalise the movement vector and make it proportional to the speed per second.

        // movement = movement.normalized * speed * Time.deltaTime;

        // Move the player to it's current position plus the movement.
        if (Input.GetKey("w") || Input.GetKey("s") || Input.GetKey("a") || Input.GetKey("d"))
        {
            speed = 6f;
            movement = movement.normalized * speed * Time.deltaTime;
            playerRigidbody.MovePosition(transform.position + movement);
            return;
        }

        while (speed > 1f)
        {
            speed *= friction;
            if (other.attachedRigidbody)

                other.attachedRigidbody.AddForce(Vector3.forward * speed);
        }
    }
    void Move(float h, float v)
    {
        // Set the movement vector based on the axis input.
        movement.Set(h, 0f, v);

        // Normalise the movement vector and make it proportional to the speed per second.

        movement = movement.normalized * speed * Time.deltaTime;

        // Move the player to it's current position plus the movement.

        playerRigidbody.MovePosition(transform.position + movement);

    }
}