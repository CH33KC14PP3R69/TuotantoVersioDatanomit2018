using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerControls controls;
    private Rigidbody rb;
    private Vector2 movementInput;
    private Vector3 desiredDirection;
    private GroundSensor groundSensor;
    [Header("Punch Setting")]
    [SerializeField] float punchStrength = 100f;
    [SerializeField] float rayLength = 10f;
    [SerializeField] Transform rayCastPos;
    [Header("Movement Setting")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float turnSpeed = 5f;
    [SerializeField] private float jumpForce = 500f;
    [SerializeField] private float jumpCd = 2f;

    private Vector3 inputDirection;
    private Vector3 moveVector;
    private Quaternion currentRotation;
    private bool grounded = false;
    

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        controls = new PlayerControls();

        controls.Player.Movement.performed += 
            context => movementInput = context.ReadValue<Vector2>();
        groundSensor = GetComponentInChildren<GroundSensor>();
    }


    private void Update()
    {
        GroundCheck();
        Move(desiredDirection);
        Turn(desiredDirection);

        if (controls.Player.Jump.triggered && grounded)
        {
            JumpInput();
        }

        if (controls.Player.Punch.triggered)
        {
            AttackAction();
        }
    }

    void FixedUpdate()
    {
       GetDirection();
    }

    private void GetDirection()
    {
        float h = movementInput.x;
        float v = movementInput.y;

        Vector3 targetInput = new Vector3(h, 0, v);

        inputDirection = Vector3.Lerp
            (inputDirection, targetInput, Time.deltaTime * 10f);

        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;
        camForward.y = 0f;
        camRight.y = 0f;

         desiredDirection = camForward
            * inputDirection.z + camRight * inputDirection.x;

         
    }

    void Move(Vector3 desiredDirection)
    {
        moveVector.Set(desiredDirection.x, 0f, desiredDirection.z);
        moveVector = moveVector * moveSpeed * Time.deltaTime;
        transform.position += moveVector;
    }

    void Turn(Vector3 desiredDirection)
    {
        if ((desiredDirection.x > 0.1 || desiredDirection.x < -0.1) 
            || (desiredDirection.z > 0.1 || desiredDirection.z < -0.1))
        {
            currentRotation = Quaternion.LookRotation(desiredDirection);
            transform.rotation = Quaternion.Lerp(this.transform.rotation, currentRotation, Time.deltaTime * turnSpeed);
        }
        else
            transform.rotation = currentRotation;
    }
    void GroundCheck()
    {
        if (!grounded && groundSensor.State())
        {
            grounded = true;
        }

        if (grounded && !groundSensor.State())
        {
            grounded = false;
        }
    }
    void AttackAction()
    {

        Ray ray = new Ray(rayCastPos.position, rayCastPos.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, rayLength))
        {
           GameObject objecthit = hit.collider.gameObject;

            if (objecthit.GetComponent<Rigidbody>())
            {
                objecthit.GetComponent<Rigidbody>().AddForceAtPosition(hit.transform.forward * punchStrength, hit.point);
            }
        }
    }

    void JumpInput()
    {
        rb.AddForce(Vector3.up * jumpForce);
    }

    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }

}
