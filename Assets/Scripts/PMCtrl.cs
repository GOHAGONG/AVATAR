using UnityEngine;
using System.Collections;
using Valve.VR;

public class PMCtrl : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 0f;
    public float jumpForce = 0f;

    [Header("Components")]
    private CharacterController controller;
    private Vector3 velocity;
    public bool isGrounded;
    public bool isCrouching = false;
    public bool isWalking = false;
    public bool isRunning = false;
    public bool isJumping = false;
    public bool isCrawling = false;
    public bool filpLeft = false;
    private bool leftReleasedFlag = false;
    private bool rightReleasedFlag = false;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.1f;
    public LayerMask groundMask;

    [Header("Animation")]
    public Animator animator;

    [Header("VR Input")]
    public SteamVR_Action_Vector2 trackpadAxisAction;
    public SteamVR_Action_Boolean jumpAction;
    public SteamVR_Action_Boolean crouchAction;
    public SteamVR_Action_Boolean crawlAction;
    public SteamVR_Input_Sources leftInputSource = SteamVR_Input_Sources.LeftHand;
    public SteamVR_Input_Sources rightInputSource = SteamVR_Input_Sources.RightHand;
    private SteamVR_Input_Sources? activeHand = null;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Ground Check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
        

        // Movement input
        Vector2 rightInput = trackpadAxisAction.GetAxis(rightInputSource);
        Vector2 leftInput = trackpadAxisAction.GetAxis(leftInputSource);

        bool leftWalking = leftInput.y > 0.2;
        bool rightWalking = rightInput.y > 0.2;

        if (activeHand == null)
        {
            if (leftWalking)
                activeHand = leftInputSource;
            else if (rightWalking)
                activeHand = rightInputSource;
        }

        if (activeHand == leftInputSource && leftWalking == false)
            activeHand = null;

        if (activeHand == rightInputSource && rightWalking == false)
            activeHand = null;

        Vector2 activeInput = Vector2.zero;
        
        if (activeHand == leftInputSource)
            activeInput = leftInput;
        else if (activeHand == rightInputSource)
            activeInput = rightInput;

        


        if (activeHand != null && !isJumping)
        {
            // 이동벡터 계산
            Vector3 move = transform.right * activeInput.x + transform.forward * activeInput.y;

            // 이동 적용
            controller.Move(move * moveSpeed * Time.deltaTime);
            
            isWalking = (activeInput.y > 0.2f && activeInput.y < 0.8f);
            isRunning = (activeInput.y >= 0.5f && activeInput.y < 0.8f);
        }
        else
        {
            isWalking = false;
            isRunning = false;
        }

        // Animator 연동
        if (animator != null)
        {
            animator.SetBool("isWalking", isWalking);
            animator.SetBool("isCrouching", isCrouching);
            animator.SetBool("isGrounded", isGrounded);
            animator.SetBool("isRunning", isRunning);
            animator.SetBool("isCrawling", isCrawling);
        }

        // Jump
        bool leftPressed = jumpAction.GetStateDown(leftInputSource);
        bool rightPressed = jumpAction.GetStateDown(rightInputSource);

        if (!isJumping && leftPressed && rightPressed && isGrounded)
        {
            isJumping = true;
            animator.SetTrigger("Jump Prepare");
            Debug.Log("Jump preparation started");
            leftReleasedFlag = false;
            rightReleasedFlag = false;
        }

        if (isJumping)
        {
            if (jumpAction.GetStateUp(leftInputSource))
                leftReleasedFlag = true;
            if (jumpAction.GetStateUp(rightInputSource))
                rightReleasedFlag = true;
            
            if (leftReleasedFlag && rightReleasedFlag)
            {
                animator.SetTrigger("Jump Execute");
                velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
                Debug.Log("Jump executed!");
                isJumping = false;
            }
        }

        // Gravity 적용
        //velocity.y += Physics.gravity.y * Time.deltaTime;
        //controller.Move(velocity * Time.deltaTime);

        if (crouchAction.GetStateDown(rightInputSource))
        {
            animator.SetTrigger("Crouch Start");
            isCrouching = true;
        }

        if ((crouchAction.GetStateUp(rightInputSource) && isCrouching)) 
        {
            animator.SetTrigger("Crouch End");
            isCrouching = false;
        }

        if (crouchAction.GetStateDown(leftInputSource))
        {
            
            Vector3 newScale = transform.localScale;
            newScale.x = Mathf.Abs(newScale.x) * -1f; // 왼손 입력이면 반전
            transform.localScale = newScale;
            filpLeft = true;

            animator.SetTrigger("Crouch Start");
            isCrouching = true;

        }

        if ((crouchAction.GetStateUp(leftInputSource) && isCrouching)) 
        {
            animator.SetTrigger("Crouch End");
            isCrouching = false;

            Vector3 newScale = transform.localScale;
            newScale.x = Mathf.Abs(newScale.x) * 1f; // 왼손 입력이면 반전
            transform.localScale = newScale;
            filpLeft = false;
        }

        if (crawlAction.GetStateDown(leftInputSource))
        {
            Debug.Log("Crawl Left Triggered");
            animator.SetTrigger("Crawl Start");
            isCrawling = true;

            if (activeHand == leftInputSource)
                activeHand = rightInputSource;
        }
        if (crawlAction.GetStateDown(rightInputSource))
        {
            Debug.Log("Crawl Right Triggered");
            animator.SetTrigger("Crawl Start");
            isCrawling = true;

            if (activeHand == rightInputSource)
                activeHand = leftInputSource;
        }

        if ((crawlAction.GetStateUp(leftInputSource) && isCrawling) ||
            (crawlAction.GetStateUp(rightInputSource) && isCrawling))
        {
            animator.SetTrigger("Crawl End");
            isCrawling = false;
        }
    }
}