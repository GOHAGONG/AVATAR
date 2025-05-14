using UnityEngine;
using System.Collections;
using Valve.VR;

public class FullBodyCtrl : MonoBehaviour
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
    public float crouchThreshold;
    private bool crouchTriggered = false;
    public float jumpThreshold;
    public float jumpExitThreshold;
    public float crawlThreshold1;
    public float crawlThreshold2;
    public float walkThreshold;
    public bool isWalkThreshold;
    public float runThreshold;
    // public float crawlThreshold3;
    // public float crawlThreshold4;

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
    public SteamVR_Input_Sources leftHandInputSource = SteamVR_Input_Sources.LeftHand;
    public SteamVR_Input_Sources rightHandInputSource = SteamVR_Input_Sources.RightHand;
    public SteamVR_Input_Sources leftFootInputSource = SteamVR_Input_Sources.LeftFoot;
    public SteamVR_Input_Sources rightFootInputSource = SteamVR_Input_Sources.RightFoot;
    private SteamVR_Input_Sources? activeHand = null;


    public SteamVR_Behaviour_Pose leftHandPose;
    public SteamVR_Behaviour_Pose rightHandPose;
    public SteamVR_Behaviour_Pose leftFootPose;
    public SteamVR_Behaviour_Pose rightFootPose;
    private Vector3 prevLeftHandPos;
    private Vector3 prevRightHandPos;
    private Vector3 prevLeftFootPos;
    private Vector3 prevRightFootPos;
    private float leftHandSwingAmount;
    private float rightHandSwingAmount;
    private float leftFootSwingAmount;
    private float rightFootSwingAmount;

    [Header("Moving_velocity")]
    private float walkTimer = 0f;
    private float runTimer = 0f;
    private float walkHoldTime = 0.9f; // 얼마 동안 유지할지
    private float runHoldTime = 1.0f;
    private float crawlConditionTimer = 0f; // 크롤 조건 유지 시간 추적
    public float crawlHoldTimeRequired = 0.5f; // 0.5초 이상 유지해야 발동
    private bool crawlConditionsHeld = false; // 조건 범위 안에 있는지
    private float crawlStartLeftY;
    private float crawlStartRightY;
    public float crawlExitDelta = 1.3f; // 탈출 기준 변화량 (0.3 이상이면 종료)
    private bool isCrawlingProtected = false; // 크롤 시작 보호 플래그


    void Start()
    {
        controller = GetComponent<CharacterController>();

        prevLeftHandPos = leftHandPose.transform.position;
        prevRightHandPos = rightHandPose.transform.position;
        prevLeftFootPos = leftFootPose.transform.position;
        prevRightFootPos = rightFootPose.transform.position;
        
    }

    void Update()
    {
        // Ground Check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
        
        // 속도 계산
        Vector3 leftHandVel = leftHandPose.GetVelocity();
        Vector3 rightHandVel = rightHandPose.GetVelocity();
        Vector3 leftFootVel = leftFootPose.GetVelocity();
        Vector3 rightFootVel = rightFootPose.GetVelocity();

        float leftHandForward = Vector3.Dot(transform.forward, leftHandVel);
        float rightHandForward = Vector3.Dot(transform.forward, rightHandVel);
        float leftFootForward = Vector3.Dot(transform.forward, leftFootVel);
        float rightFootForward = Vector3.Dot(transform.forward, rightFootVel);

        float avgHandSpeed = (Mathf.Abs(leftHandForward) + Mathf.Abs(rightHandForward)) / 2f;
        float avgFootSpeed = (Mathf.Abs(leftFootForward) + Mathf.Abs(rightFootForward)) / 2f;

        // 손 교차 판단
        bool handsAreCrossing = (leftHandForward > 0.05f && rightHandForward < -0.05f) ||
                                (leftHandForward < -0.05f && rightHandForward > 0.05f);
                                // 손 교차 판단
        bool feetAreCrossing = (leftFootForward > 0.05f && rightFootForward < -0.05f) ||
                                (leftFootForward < -0.05f && rightFootForward > 0.05f);
        
        // 높이 계산
        float leftY = leftHandPose.transform.position.y;
        float rightY = rightHandPose.transform.position.y;
        float yDiff = Mathf.Abs(leftY - rightY);
        // bool leftLower = leftY < rightY;

        bool idlecheck = !isJumping && !isCrawling && !isCrouching && !isWalking && !isWalking;

        if(idlecheck && leftY >= jumpThreshold && rightY >= jumpThreshold){
            isJumping = true;
            animator.SetTrigger("Jump Prepare");
        }
        if(isJumping && leftY < jumpExitThreshold && rightY < jumpExitThreshold){
            animator.SetTrigger("Jump Execute");
            velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
            Debug.Log("Jump executed!");
            isJumping = false;
        }

        // 크롤 진입 조건 범위 안에 있는지 판단
        bool inCrawlRange = leftY > crawlThreshold1 && leftY < crawlThreshold2 &&
                            rightY > crawlThreshold1 && rightY < crawlThreshold2;

        if (idlecheck && inCrawlRange)
        {
            crawlConditionTimer += Time.deltaTime; // 조건 만족 중 → 타이머 증가

            if (crawlConditionTimer >= crawlHoldTimeRequired && !isCrawling)
                {
                    //Debug.Log("Crawl Triggered after Hold");
                    animator.SetTrigger("Crawl Start");
                    isCrawling = true;

                    // 보호 타이머 시작 
                    // 애니메이션 재생동안 Crawl Exit Delta 계산하지 않도록
                    StartCoroutine(CrawlProtectionDelay(2.0f));

                    crawlConditionTimer = 0f; // 초기화
                }
        }
        else
        {
            crawlConditionTimer = 0f; // 범위 벗어나면 타이머 리셋
        }

        if(isCrawling){
            // 타이머 업데이트
            if (handsAreCrossing && avgHandSpeed > 0.05f)
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
        }

        // if (isCrawling && ((leftY < crawlThreshold3 && rightY < crawlThreshold3) || (leftY > crawlThreshold4 && rightY > crawlThreshold4)) )
        // {
        //     animator.SetTrigger("Crawl End");
        //     isCrawling = false;
        // }

        if (isCrawling && !isCrawlingProtected)
        {
            float leftDelta = Mathf.Abs(leftY - crawlStartLeftY);
            float rightDelta = Mathf.Abs(rightY - crawlStartRightY);

            Debug.Log("left Delta: " + leftDelta);

            if (leftDelta > crawlExitDelta && rightDelta > crawlExitDelta)
            {
                //Debug.Log("Crawl End triggered by movement delta");
                animator.SetTrigger("Crawl End");
                isCrawling = false;
            }
        }


        if (idlecheck && yDiff >= crouchThreshold && !crouchTriggered)
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

        if (leftY < walkThreshold && rightY < walkThreshold)
            isWalkThreshold = true;
        else isWalkThreshold = false;
        
        if(!isCrawling && !isCrouching && !isJumping){
            if(isWalkThreshold)
            {
                // 타이머 업데이트
                if (handsAreCrossing && avgHandSpeed > 0.05f && feetAreCrossing && avgFootSpeed > 0.05f)
                {
                    Debug.Log("Foot Speed: " + avgFootSpeed);
                    // 조건 만족했을 때만 타이머 리셋
                    if (avgFootSpeed >= 0.9f)
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
                //Debug.Log($"[Move] Cross: {handsAreCrossing}, Speed: {avgSpeed:F2}, WalkT: {walkTimer:F2}, RunT: {runTimer:F2}");
            }
            else{
                isRunning = false;
                isWalking = false;
            }
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
    }
    private IEnumerator CrawlProtectionDelay(float delay)
    {
        isCrawlingProtected = true;
        yield return new WaitForSeconds(delay);
        isCrawlingProtected = false;

        // 엎드렸을 때 손 위치 초기화
        crawlStartLeftY = leftHandPose.transform.position.y; // leftY
        crawlStartRightY = rightHandPose.transform.position.y; // rightY
        Debug.Log("Crawl Start");
    }
}