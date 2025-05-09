using UnityEngine;
using System.Collections;
using Valve.VR;

[RequireComponent(typeof(Animator))]
public class AvatarIKController : MonoBehaviour
{
    public Transform leftControllerTransform;
    public Transform rightControllerTransform;
    public Transform leftTrackerTransform;
    public Transform rightTrackerTransform;
    // public Transform leftElbowHintPosition;
    // public Transform rightElbowHintPosition;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {
            // 왼손
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftControllerTransform.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, leftControllerTransform.rotation);

            // 오른손
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightControllerTransform.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, rightControllerTransform.rotation);

            // // 팔꿈치
            // animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 1f);
            // animator.SetIKHintPosition(AvatarIKHint.LeftElbow, leftElbowHintPosition.position);

            // animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 1f);
            // animator.SetIKHintPosition(AvatarIKHint.RightElbow, rightElbowHintPosition.position);

            // 왼발
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
            animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftTrackerTransform.position);
            animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftTrackerTransform.rotation);

            // 오른발
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
            animator.SetIKPosition(AvatarIKGoal.RightFoot, rightTrackerTransform.position);
            animator.SetIKRotation(AvatarIKGoal.RightFoot, rightTrackerTransform.rotation);
        }
    }
}
