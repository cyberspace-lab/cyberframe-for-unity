using System.Collections.Generic;
using UnityEngine;

// SOURCE https://sharpcoderblog.com/blog/unity-3d-fps-controller
[RequireComponent(typeof(CharacterController))]

public class BasicFirstPersonController : cyberframe.Player.PlayerController
{
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    protected override void OnUpdate()
    {
        // We are grounded, so recalculate move direction based on axes
        var forward = transform.TransformDirection(Vector3.forward);
        var right = transform.TransformDirection(Vector3.right);
        // Press Left Shift to run
        var isRunning = Input.GetKey(KeyCode.LeftShift);
        var curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        var curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        var movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (!canMove) return;

        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }

    public override void EnableMovement(bool bo = true)
    {
        throw new System.NotImplementedException();
    }

    public override Vector3 Position
    {
        get { return transform.position; }
        set { gameObject.transform.position = value; }
    }
    public override Vector2 PointingDirection { get { return Rotation; } }

    public override void MoveToPosition(Vector2 position)
    {
        throw new System.NotImplementedException();
    }

    public override void MoveToPosition(Vector3 position)
    {
        throw new System.NotImplementedException();
    }

    public override Vector2 Rotation
    {
        //in common practice player rotates in Y and camera on X, but this should be reimplemented in VR
        get { return new Vector2(transform.eulerAngles.y, Camera.main.transform.eulerAngles.x); }
        set
        {
            gameObject.transform.eulerAngles = new Vector3(0, value.x, 0);
            Camera.main.transform.localEulerAngles = new Vector3(value.y, 0, 0);
        }
    }

    public override void EnableRotation(bool bo = true)
    {
        throw new System.NotImplementedException();
    }

    public override void LookAtPosition(Vector2 point)
    {
        throw new System.NotImplementedException();
    }

    public override void LookAtPosition(Vector3 point)
    {
        throw new System.NotImplementedException();
    }

    public override void SetHeight(float height)
    {
        throw new System.NotImplementedException();
    }

    public override void SetSpeed(float speed)
    {
        throw new System.NotImplementedException();
    }

    public override string HeaderLine()
    {
        return "Position; Rotation.X; Rotation.Y;";
    }
    public override List<string> PlayerInformation()
    {
        var strgs = new List<string>
        {
            Position.ToString("F4"),
            Rotation.x.ToString("F4"),
            Rotation.y.ToString("F4")
        };
        return strgs;
    }

    public override Dictionary<string, string> PlayerInformationDictionary()
    {
        var strgs = new Dictionary<string, string>
        {
            {"Position", Position.ToString("F4")},
            {"Rotation.x", Rotation.x.ToString("F4")},
            {"Rotation.y", Rotation.y.ToString("F4")}
        };
        return strgs;
    }
}
