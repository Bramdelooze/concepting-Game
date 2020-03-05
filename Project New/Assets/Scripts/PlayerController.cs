using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Handle Input Via The New Input Script?!!
public class PlayerController : MonoBehaviour {

    public Transform GroundCheck;
    public LayerMask groundLayer;
    private bool isGrounded = false;

    [Header("Keyboard controls")]
    [SerializeField] private KeyCode left;
    [SerializeField] private KeyCode right;
    [SerializeField] private KeyCode jump;

    [SerializeField] private string horizontal;
    [SerializeField] private string vertical;

    [Header("Controller Controls")]
    [SerializeField] private KeyCode joystickJump;

    private Rigidbody2D rb;
    [Header("Prefabs")]

    public LevelManager levelManager;
    //Until here
    private enum HorizontalMovementStates { STATIC, LEFT, RIGHT };
    private HorizontalMovementStates currentHorizontalMovementState;

    private PlayerClass currentClass;

    private bool isJumping = false;
    private float jumpTimeCounter;

    private void Awake()
    {
        currentClass = new Character_3Class();

        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapBox(GroundCheck.position, new Vector2(2.7f, 0f), 0, groundLayer); //(1.2f)

        rb.velocity = Vector2.up * rb.velocity.y;

        HandleInput();
        MovementUpdate();
    }

    private void Update()
    {
        transform.position = new Vector2(Mathf.Round(transform.position.x * 100) / 100, Mathf.Round(transform.position.y * 100) / 100);

        if (GetComponent<Animator>() != null)
            GetComponent<Animator>().SetFloat("Speed", Mathf.Abs(rb.velocity.x)); 

        if ((Input.GetKeyDown(jump) || Input.GetKeyDown(joystickJump)) && isGrounded)
        {
            isJumping = true;

            jumpTimeCounter = currentClass.MaxJumpTime;

            //OnJump();
        }

        if ((Input.GetKey(jump) || Input.GetKey(joystickJump)) && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                //OnJump();
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        if (Input.GetKeyUp(jump) || Input.GetKeyUp(joystickJump))
        {
            isJumping = false;
        }
    }

    private void HandleInput()
    {
        currentHorizontalMovementState = HorizontalMovementStates.STATIC;

        if (Input.GetKey(left) || Input.GetAxis(horizontal) <= -.8)
        {
            currentHorizontalMovementState = HorizontalMovementStates.LEFT;
        }
        if(Input.GetKey(right) || Input.GetAxis(horizontal) >= .8)
        {
            currentHorizontalMovementState = HorizontalMovementStates.RIGHT;
        }
    }

    private void MovementUpdate()
    {
        if(currentHorizontalMovementState == HorizontalMovementStates.STATIC)
        {
            return;
        } else if(currentHorizontalMovementState == HorizontalMovementStates.LEFT)
        {
            transform.rotation = Quaternion.identity;

            rb.velocity = new Vector2(-currentClass.MoveSpeed, rb.velocity.y);
        } else if(currentHorizontalMovementState == HorizontalMovementStates.RIGHT)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);

            rb.velocity = new Vector2(currentClass.MoveSpeed, rb.velocity.y);
        }
        else
        {
            Debug.LogError("Unrecognised state " + currentHorizontalMovementState);
        }
    }

    public void OnJump()
    {
        rb.velocity = Vector2.up * currentClass.JumpHeight;
    }
}