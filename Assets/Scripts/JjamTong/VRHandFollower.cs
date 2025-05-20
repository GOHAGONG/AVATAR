using UnityEngine;

[RequireComponent(typeof(Animator))]
public class VRHandFollower : MonoBehaviour
{
    public bool ikActive = true;
    public Transform leftHandTarget;
    public Transform rightHandTarget;

    private Animator animator;

    [Tooltip("애니메이터 상태 이름 (예: Idle)")]
    public string ikActiveStateName = "Breathing Idle";

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (animator == null || !ikActive) return;

        // 현재 상태 이름 가져오기
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(layerIndex);

        bool isInIKState = currentState.IsName(ikActiveStateName);

        if (isInIKState)
        {
            if (leftHandTarget != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);
            }

            if (rightHandTarget != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
                animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
                animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);
            }
        }
        else
        {
            // IK 끄기
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0f);
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0f);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0f);
        }
    }
}
