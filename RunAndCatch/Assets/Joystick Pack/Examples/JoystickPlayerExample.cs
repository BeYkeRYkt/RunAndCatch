using System;
using UnityEngine;


public class JoystickPlayerExample : MonoBehaviour
{
    public float speed;
    public VariableJoystick variableJoystick;
    public Rigidbody rb;
    private Animator anim;
    private bool grounded = false;
    public int maxSlopeAngle = 10;
    private bool joyMoved = false;

    public bool isGrounded()
    {
        return grounded;
    }

    public bool isJoystickMoved()
    {
        return joyMoved;
    }

    public void Start()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();
        // if player is old model
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
        variableJoystick = FindObjectOfType<VariableJoystick>();
    }

    // BeYkeRYkt add start: fixes ID 004
    void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contactPoint in collision.contacts)
        {
            Debug.DrawRay(contactPoint.point, contactPoint.normal * 10, Color.white); // DEBUG
            if (Vector3.Angle(Vector3.up, contactPoint.normal) < maxSlopeAngle)
            {
                grounded = true;
            }
            else
            {
                rb.AddForce(collision.impulse, ForceMode.Impulse);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (grounded)
        {
            grounded = false;
        }
    }
    // BeYkeRYkt add end: fixes ID 004

    private Vector3 lookDir;
    private Vector3 oldLookDir;
    public float maxVelocitySpeed = 10f;

    public void FixedUpdate()
    {
        if (variableJoystick.Vertical != 0f || variableJoystick.Horizontal != 0f)
        {
            joyMoved = true;
            Vector3 direction = Vector3.forward * variableJoystick.Vertical + Vector3.right * variableJoystick.Horizontal;
            if (grounded)
            {
                if (rb.velocity.magnitude < maxVelocitySpeed)
                {
                    rb.AddForce(direction * speed * Time.fixedDeltaTime, ForceMode.VelocityChange);
                }
            }
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = rotation;
            oldLookDir = direction;
        }
        else
        {
            joyMoved = false;
            transform.rotation = Quaternion.LookRotation(oldLookDir);
        }
        Animating();
    }

    void Animating()
    {
        // Create a boolean that is true if either of the input axes is non-zero.
        float speed = rb.velocity.magnitude;
        bool walking = (speed != 0) && variableJoystick.Horizontal != 0f || variableJoystick.Vertical != 0f;
        float defaultAnimationSpeed = 1.2f;
        if (walking)
        {
            float speedAnim = Math.Abs(defaultAnimationSpeed * variableJoystick.Vertical + defaultAnimationSpeed * variableJoystick.Horizontal) * speed;
            if (speedAnim > defaultAnimationSpeed)
            {
                speedAnim = defaultAnimationSpeed;
            }
            anim.speed = speedAnim;
            //Debug.Log("Speedy racer: " + speedAnim);
        }
        else
        {
            anim.speed = defaultAnimationSpeed;
        }

        // Tell the animator whether or not the player is walking.
        anim.SetBool("IsWalking", walking);
    }
}