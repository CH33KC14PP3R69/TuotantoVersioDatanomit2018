using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    private PlayerControls controls;
    private Rigidbody rb;
    private Vector2 movementInput;
    //private Vector3 desiredDirection;
    private GroundSensor groundSensor;
    [Header("Punch Setting")]
    [SerializeField] float punchStrength = 250f;
    [SerializeField] float rayLength = 10f;
    [SerializeField] Transform rayCastPos;
    [Header("Movement Setting")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float turnSpeed = 1f;
    [SerializeField] private float jumpForce = 500f;
    //[SerializeField] private float jumpCd = 2f;

    //private Vector3 inputDirection;
    private Vector3 moveVector;
    //private Quaternion currentRotation;
    private bool grounded = false;
    private int Score;
    public Text ScoreText;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        controls = new PlayerControls();
        Score = 0;
        SetScoreText ();

        controls.Player.Movement.performed +=
            context => movementInput = context.ReadValue<Vector2>();
        groundSensor = GetComponentInChildren<GroundSensor>();
    }


    private void Update()
    {
        GroundCheck();
        Move();
        Turn();

        if (controls.Player.Jump.triggered && grounded)
        {
            JumpInput();
        }

        if (controls.Player.Punch.triggered)
        {
            AttackAction();
        }
    }

    void Move()
    {
        moveVector = (transform.forward * movementInput.y) * moveSpeed;
        moveVector.y = rb.velocity.y;
        rb.velocity = moveVector;
    }

    void Turn()
    {
        transform.Rotate(new Vector3(0f, movementInput.x, 0) * turnSpeed);
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
                objecthit.GetComponent<Rigidbody>().AddForceAtPosition(this.transform.forward * punchStrength, hit.point);
                Score = Score + 1;
                SetScoreText();
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
    void SetScoreText()
    {
        ScoreText.text = "Score: " + Score.ToString();
    }
}
