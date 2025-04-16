using UnityEngine;
using System.Collections;
using Valve.VR;

public class HalfBodyCtrl : MonoBehaviour
{
    public enum currentTest { Walk_Run, Jump, Crouch, Crawl };
    [Header("SelectTestName")]
    public currentTest currentTestName;

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
    public float crouchThreshold;
    private bool crouchTriggered = false;
    public float jumpThreshold;

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


    public SteamVR_Behaviour_Pose leftHandPose;
    public SteamVR_Behaviour_Pose rightHandPose;
    private Vector3 prevLeftPos;
    private Vector3 prevRightPos;
    private float leftSwingAmount;
    private float rightSwingAmount;

    [Header("Moving_velocity")]
    private float walkTimer = 0f;
    private float runTimer = 0f;
    private float walkHoldTime = 0.9f; // 얼마 동안 유지할지
    private float runHoldTime = 1.0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        prevLeftPos = leftHandPose.transform.position;
        prevRightPos = rightHandPose.transform.position;
        
    }

    void Update()
    {
        // Ground Check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
        
        // 속도 계산
        Vector3 leftVel = leftHandPose.GetVelocity();
        Vector3 rightVel = rightHandPose.GetVelocity();

        float leftForward = Vector3.Dot(transform.forward, leftVel);
        float rightForward = Vector3.Dot(transform.forward, rightVel);

        float avgSpeed = (Mathf.Abs(leftForward) + Mathf.Abs(rightForward)) / 2f;

        // 손 교차 판단
        bool handsAreCrossing = (leftForward > 0.05f && rightForward < -0.05f) ||
                                (leftForward < -0.05f && rightForward > 0.05f);
        
        // 높이 계산
        float leftY = leftHandPose.transform.position.y;
        float rightY = rightHandPose.transform.position.y;

        if (currentTestName == currentTest.Walk_Run)
        {
            // // 거리 기반
            // // 팔의 이동량 계산
            // Vector3 leftDelta = leftHandPose.transform.position - prevLeftPos;
            // Debug.Log("left :" + leftHandPose.transform.position);
            // Vector3 rightDelta = rightHandPose.transform.position - prevRightPos;

            // leftSwingAmount = Vector3.Dot(transform.forward, leftDelta);
            // Debug.Log("left threshold :" + leftSwingAmount);
            // float leftSwingAmount_abs = Mathf.Abs(leftSwingAmount);
            // rightSwingAmount = Vector3.Dot(transform.forward, rightDelta);
            // float rightSwingAmount_abs = Mathf.Abs(rightSwingAmount);

            // prevLeftPos = leftHandPose.transform.position;
            // prevRightPos = rightHandPose.transform.position;

            // // 걷기/달리기 판단
            // float walkThreshold = 0.0002f;
            // float runThreshold = 0.01f;

            // bool isArmMoving = leftSwingAmount * rightSwingAmount < -0.0000004;
            // if(isArmMoving)
            //     Debug.Log("isArmMoving!");
            // isWalking = isArmMoving && (leftSwingAmount_abs < runThreshold && rightSwingAmount_abs < runThreshold);
            // isRunning = isArmMoving && (leftSwingAmount_abs >= runThreshold && rightSwingAmount_abs >= runThreshold);

            // // 이동 방향 = 양손의 이동방향 평균
            // Vector3 moveDirection = ((leftDelta + rightDelta) / 2f);
            // moveDirection.y = 0f;

            // if (isArmMoving)
            // {
            //     float speed = isRunning ? runSpeed : walkSpeed;
            //     controller.Move(moveDirection.normalized * speed * Time.deltaTime);
            // }

            // (기존의 Jump, Crouch 등은 그대로 유지)

            // void OnGUI()
            // {
            //     GUI.Label(new Rect(10, 10, 400, 30), $"Left Swing: {leftSwingAmount:F2} | Right Swing: {rightSwingAmount:F2}");
            //     GUI.Label(new Rect(10, 30, 400, 30), $"Walking: {isWalking} | Running: {isRunning}");
            // }

            // 속도 기반

            // 타이머 업데이트
            if (handsAreCrossing && avgSpeed > 0.05f)
            {
                // 조건 만족했을 때만 타이머 리셋
                if (avgSpeed >= 0.4f)
                {
                    runTimer = runHoldTime;
                }
                else
                {
                    walkTimer = walkHoldTime;
                }
            }

            // 타이머 감소
            if(walkTimer > 0)
                walkTimer -= Time.deltaTime;
            if(runTimer > 0)
                runTimer -= Time.deltaTime;

            // 상태 판단
            isRunning = runTimer > 0f;
            isWalking = walkTimer > 0f;

            // 디버깅 (선택사항)
            Debug.Log($"[Move] Cross: {handsAreCrossing}, Speed: {avgSpeed:F2}, WalkT: {walkTimer:F2}, RunT: {runTimer:F2}");
        }

        else if (currentTestName == currentTest.Jump)
        {
            Debug.Log("Height :"+leftY);
            Debug.Log("jumpThreshold: " + jumpThreshold);
            // Jump
            if (!isJumping && leftY >= jumpThreshold && rightY >= jumpThreshold)
            {
                isJumping = true;
                animator.SetTrigger("Jump Prepare");
                Debug.Log("Jump preparation started");
            }

            if(isJumping && leftY < jumpThreshold && rightY < jumpThreshold){
                animator.SetTrigger("Jump Execute");
                velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
                Debug.Log("Jump executed!");
                isJumping = false;
            }
        }

        // if ((jumpAction.GetStateUp(leftInputSource) && isJumping) ||
        //     (jumpAction.GetStateUp(rightInputSource) && isJumping))
        // {
        //     animator.SetTrigger("Jump Cancel");
        //     isJumping = false;
        // }

        // Gravity 적용
        //velocity.y += Physics.gravity.y * Time.deltaTime;
        //controller.Move(velocity * Time.deltaTime);

        else if (currentTestName == currentTest.Crouch)
        {
            float yDiff = Mathf.Abs(leftY - rightY);
            // bool leftLower = leftY < rightY;

            if (!isCrouching && yDiff >= crouchThreshold && !crouchTriggered)
            {
                animator.SetTrigger("Crouch Start");
                isCrouching = true;
                crouchTriggered = true;
                Debug.Log("Crouch Start");
            }

            if (isCrouching && yDiff < crouchThreshold * 0.5f && crouchTriggered) 
            {
                animator.SetTrigger("Crouch End");
                isCrouching = false;
                crouchTriggered = false;
                Debug.Log("Crouch End");
            }
        }

        else if (currentTestName == currentTest.Crawl)
        {
            Debug.Log("Height :"+leftY);
            if (!isCrawling && leftY > 1.0f && leftY < 1.5f && rightY > 1.4f && rightY < 1.5f)
            {
                Debug.Log("Crawl Triggered");
                animator.SetTrigger("Crawl Start");
                isCrawling = true;
            }

            // 타이머 업데이트
            if (handsAreCrossing && avgSpeed > 0.05f)
            {
                // // 조건 만족했을 때만 타이머 리셋
                // if (avgSpeed >= 0.4f)
                // {
                    // runTimer = runHoldTime;
                // }
                // else
                // {
                    walkTimer = walkHoldTime;
                // }
            }

            // 타이머 감소
            if(walkTimer > 0)
                walkTimer -= Time.deltaTime;
            // if(runTimer > 0)
                // runTimer -= Time.deltaTime;

            // 상태 판단
            // isRunning = runTimer > 0f;
            isWalking = (isCrawling && walkTimer > 0f);

            if (isCrawling && (leftY < 0.5f && rightY < 0.5f) )
            {
                animator.SetTrigger("Crawl End");
                isCrawling = false;
            }

        }
        // if (!isCrawling &&crouchAction.GetStateDown(leftInputSource))
        // {
            
        //     Vector3 newScale = transform.localScale;
        //     newScale.x = Mathf.Abs(newScale.x) * -1f; // 왼손 입력이면 반전
        //     transform.localScale = newScale;
        //     filpLeft = true;

        //     animator.SetTrigger("Crouch Start");
        //     isCrouching = true;

        // }

        // if ((crouchAction.GetStateUp(leftInputSource) && isCrouching)) 
        // {
        //     animator.SetTrigger("Crouch End");
        //     isCrouching = false;

        //     Vector3 newScale = transform.localScale;
        //     newScale.x = Mathf.Abs(newScale.x) * 1f; // 왼손 입력이면 반전
        //     transform.localScale = newScale;
        //     filpLeft = false;
        // }


        // Animator 연동
        if (animator != null)
        {
            animator.SetBool("isWalking", isWalking);
            animator.SetBool("isCrouching", isCrouching);
            animator.SetBool("isGrounded", isGrounded);
            animator.SetBool("isRunning", isRunning);
            animator.SetBool("isCrawling", isCrawling);
        }
    }
}