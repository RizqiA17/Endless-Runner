using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Utilities;

public class PlayerMovement : MonoBehaviour
{
    [Header("State")]
    public bool isSliding;
    public bool isJump;
    private bool isDead;
    private bool isStart;
    private bool isGrounded;
    private bool isMove;
    private bool isMouseHidden;
    private bool isFall;
    private bool isChangeHeight;
    private int pos = 1;

    [Header("Universal")]
    [SerializeField] private float checkRadius;
    private ClassCaller callClass;
    private CharacterController charController;
    private float normalHeight;
    private Vector3 velocity;

    [Header("Movement")]
    [SerializeField] private float PlayerSpeed;
    [SerializeField] private float rollingSpeed;
    [SerializeField] private float mouseCamRotationSpeed;
    [SerializeField] private float rotationSmoothTime;
    [SerializeField] private float jumpForce;
    [SerializeField] private float smoothJump;
    [SerializeField] private float rollHeight;
    [SerializeField] private float newHeight;
    [SerializeField] private Transform camTransform;
    [SerializeField] private Transform[] path;
    [SerializeField] private Transform obstacleGameOverChecker;
    private float currentVelocity;
    private float gravity;
    private float heightBefore;

    [Header("Gravity")]
    [SerializeField] private float gravityForce;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    private int start;
    private int dead;
    private int jump;
    private int slide;

    // Start is called before the first frame update
    void Start()
    {
        callClass = GameObject.FindGameObjectWithTag("GameController").GetComponent<ClassCaller>();
        charController = GetComponent<CharacterController>();
        camTransform = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();
        normalHeight = charController.height;
        gravity = gravityForce;
        start = Animator.StringToHash("Start");
        dead = Animator.StringToHash("Dead");
        jump = Animator.StringToHash("Jump");
        slide = Animator.StringToHash("Slide");
    }

    // Update is called once per frame
    void Update()
    {
        // Hide and Unhide Cursor
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (isMouseHidden)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                isMouseHidden = false;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                isMouseHidden = true;
            }
        }

        if (!isDead) Move();

        if (isChangeHeight) ChangeHeight(charController.height, newHeight);
    }

    // Script for Movement Character and Call Gravity Effect 
    private void Move()
    {
        if (!callClass.GameManager.isStart) return;

        if (!isStart)
        {
            isStart = true;
            animator.SetTrigger(start);
        }

        // Input Movement
        bool leftInput = (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && pos != 0;
        bool rightInput = (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && pos != 2;
        bool jumpInput = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
        bool slideInput = Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);

        velocity = new Vector3(PlayerSpeed, velocity.y, 0);

        if (IsGrounded())
        {
            Movement(velocity);

            if (jumpInput) Jump();
            if (slideInput) Sliding();

            gravity = gravityForce;
            //if (!isSliding) obstacleGameOverChecker.transform.localScale = new Vector3(1, normalHeight, 1);
        }
        else
        {
            Gravity();
        }

        if (leftInput) pos--;
        else if (rightInput) pos++;

        if (!isSliding && !isJump && IsGrounded()) animator.speed = callClass.GameManager.levelSpeed * 5;

        if (isSliding && jumpInput) Jump();

        if (isJump && !IsGrounded() && slideInput)
        {
            gravity *= 2;
            Sliding();
        }
        transform.position = new Vector3(transform.position.x, transform.position.y, Utility.SmoothTransitionFloat(transform.position.z, path[pos].position.z, 0.25f));

        //animator.SetFloat(animVelocityY, velocity.y);
        //animator.SetBool(animGrounded, CanSlide() || isGrounded);
        //animator.SetBool(animIsRolling, isRolling);
    }

    public void Jump()
    {
        isJump = true;
        if (isSliding) velocity.y = jumpForce * 1.4f;
        else velocity.y = jumpForce;
        animator.speed = 1;
        charController.height = normalHeight;
        isChangeHeight = false;
        isSliding = false;
        animator.SetTrigger(jump);
    }

    private void Sliding()
    {
        gravity = gravityForce;
        animator.speed = 1;
        animator.SetTrigger(slide);
        isSliding = true;

        //animator.CrossFade(animRolling, .1f);
        heightBefore = normalHeight;
        newHeight = rollHeight;
        isChangeHeight = true;

        //obstacleGameOverChecker.transform.localScale = new Vector3(1, rollHeight, 1);
    }

    public void Dead()
    {
        isDead = true;
        callClass.GameManager.levelSpeed = 0;
        animator.speed = 1;
        animator.SetTrigger(dead);
        callClass.GameManager.GameOver();
    }

    public void DefaultHeight()
    {
        heightBefore = charController.height;
        newHeight = normalHeight;
        isChangeHeight = true;
    }

    private void ChangeHeight(float heightBase, float height)
    {
        charController.height = Utility.SmoothTransitionFloat(heightBase, height, .5f);

        if (charController.height == height)
        {
            isChangeHeight = false;

        }
    }

    public void FindSlider()
    {
        //slider = GameObject.FindGameObjectsWithTag("Slider");
    }

    private void Gravity()
    {
        //if (velocity.y < -gravity) velocity.y = -gravity; // Set Max Velocity y
        //else 
        //velocity.y -= Time.deltaTime * gravityForce * 0.1f * smoothJump; // Membuat Effect Percepatan Smooth saat Lompat atau Terjatuh
        //Mathf.Lerp(velocity.y, velocity.y - gravity, Time.deltaTime);
        velocity.y = Utility.SmoothTransitionFloat(velocity.y, velocity.y - gravity);

        //velocity = Mathf.Lerp(velocity, Vector3.zero, Time.deltaTime);
        Movement(GravityMovement(velocity));
    }

    // Movement Ketika Character tidak Bergerak agar Hanya Terkena efek Gravitasi
    Vector3 GravityMovement(Vector3 move)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.y = move.y;
        return moveDirection;
    }

    private void Movement(Vector3 velocity)
    {
        charController.Move(new Vector3(velocity.x * PlayerSpeed, velocity.y, velocity.z * PlayerSpeed) * Time.deltaTime);
    }

    private bool GameOverChecker()
    {
        return Physics.CheckSphere(obstacleGameOverChecker.position, checkRadius, groundLayer);
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, checkRadius, groundLayer);
    }


    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (!collision.gameObject.CompareTag("Slider")) return;

    //    foreach (var item in slider)
    //    {
    //        if (collision.gameObject == item)
    //        {
    //            isSliding = true;
    //        }
    //    }
    //}
    //private void OnCollisionStay(Collision collision)
    //{
    //    print("Tes");
    //    if (!collision.gameObject.CompareTag("Slider")) return;

    //    foreach (var item in slider)
    //    {
    //        if (collision.gameObject == item)
    //        {
    //            PlayerRotation(collision.gameObject.transform.rotation * Vector3.forward);
    //            print(collision.gameObject.transform.rotation * Vector3.forward);
    //        }
    //    }
    //}
    //private void OnCollisionExit(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Slider")) isSliding = false;
    //}
}
