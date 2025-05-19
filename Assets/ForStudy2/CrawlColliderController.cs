using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CrawlColliderController : MonoBehaviour
{
    private CharacterController controller;
    private float originalHeight;
    private Vector3 originalCenter;

    [Header("Crawl Settings")]
    public float crawlHeight = 1.0f;
    public Vector3 crawlCenter = new Vector3(0f, 0.5f, 0f);

    public bool isCrawling = false;
    public float checkRadius = 1.2f;
    public Transform headCheck;
    public LayerMask ceilingMask;
    public bool canStand = true;

    [Header("Animation (Optional)")]
    public Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        originalHeight = controller.height;
        originalCenter = controller.center;
    }

    void Update()
    {
        canStand = Physics.CheckSphere(headCheck.position, checkRadius, ceilingMask);

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isCrawling)
        {
            StartCrawl();
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) && isCrawling)
        {
            if (CanStandUp())
            {
                StopCrawl();
            }
            else
            {
                isCrawling = true;
                animator.SetBool("isCrawling", true);
                /*Debug.Log("Cannot stand up: ceiling too low. Staying in crawl.");*/
            }
        }
    }

    void StartCrawl()
    {
        isCrawling = true;
        controller.height = crawlHeight;
        controller.center = crawlCenter;

        if (animator != null)
        {
            animator.SetTrigger("Crawl Start");
        }
    }

    void StopCrawl()
    {
        isCrawling = false;
        controller.height = originalHeight;
        controller.center = originalCenter;

        if (animator != null)
        {
            animator.SetTrigger("Crawl End");
        }
    }

    bool CanStandUp()
    {
        return !Physics.CheckSphere(headCheck.position, checkRadius, ceilingMask);
    }

    /// <summary>
    /// CheckCapsule 디버깅용 sphere 그리기
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (headCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(headCheck.position, checkRadius);
        }
    }
}
