using Photon.Pun;
using UnityEngine;

// This script moves the character controller forward
// and sideways based on the arrow keys.
// It also jumps when pressing space.
// Make sure to attach a character controller to the same game object.
// It is recommended that you make only one call to Move or SimpleMove per frame.

public class CharacterMovement : MonoBehaviour
{
    //Variables
    public float currentSpeed = 0;
    public float groundSpeed = 12.0F;
    public float airSpeed = 6.0f;
    public float jumpSpeed = 1.0F;

    // mouse variables
    public bool mouse_enable = false;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 60.0f;

    public GameObject cameraObject;
    
    private Vector3 moveDirection = Vector3.zero;
    private Vector2 rotation = Vector2.zero;

    private EntityPlayer entityPlayer;
    private ClientGameManager gameManager;

    // Photon
    private PhotonView photonView;

    private void Start()
    {
        gameManager = ClientGameManager.Instance;
        entityPlayer = GetComponent<EntityPlayer>();
        photonView = GetComponent<PhotonView>();
        rotation.y = transform.eulerAngles.y;
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        // Jumping
        if (Input.GetButton("Jump"))
        {
            if (entityPlayer.IsGrounded())
            {
                entityPlayer.SetVelocity(jumpSpeed * Vector3.up);
            }
        }
    }

    public void FixedUpdate()
    {
        // handle movement
        if (!photonView.IsMine) return;

        Vector3 cameraRight = cameraObject.transform.right;
        Vector3 cameraForward = cameraObject.transform.forward;

        float moveX = gameManager.isPaused ? 0 : Input.GetAxis("Horizontal");
        float moveZ = gameManager.isPaused ? 0 : Input.GetAxis("Vertical");

        Vector3 inputVector = new Vector3(moveX, 0, moveZ);

        //Feed moveDirection with input.
        moveDirection = cameraForward * moveZ + cameraRight * moveX;

        //Multiply it by speed.
        if (moveX != 0 || moveZ != 0)
        {
            moveDirection.y = 0;
            if (entityPlayer.IsGrounded())
            {
                currentSpeed = groundSpeed;
            }
            else
            {
                currentSpeed = airSpeed;
            }
        }
        else
        {
            currentSpeed = 0.0f;
            moveDirection = Vector3.zero;
        }

        transform.LookAt(transform.position + moveDirection);

        Vector3 calcVector = moveDirection.normalized * currentSpeed;

        // Setting movement if was collided in the air or not
        if (!entityPlayer.IsGrounded())
        {
            if (entityPlayer.checkCollide(calcVector))
            {
                calcVector = Vector3.zero;
                Debug.Log("FUU");
            }
        }

        // Making the character move
        entityPlayer.MoveEntityXZ(calcVector);

        // mouse handle
        if (mouse_enable && !gameManager.isPaused)
        {
            rotation.y += Input.GetAxis("Mouse X") * lookSpeed;
            rotation.x += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotation.x = Mathf.Clamp(rotation.x, -lookXLimit, lookXLimit);
            cameraObject.transform.localRotation = Quaternion.Euler(rotation.x, 0, 0);
            transform.eulerAngles = new Vector2(0, rotation.y);
        }
    }
}