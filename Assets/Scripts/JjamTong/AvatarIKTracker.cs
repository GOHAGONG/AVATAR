using UnityEngine;
using System.Collections;
using Valve.VR;

[RequireComponent(typeof(Animator))]
public class AvatarIKTracker : MonoBehaviour
{
    public Transform leftFootTrackerTransform;
    public Transform rightFootTrackerTransform;
    // public Transform leftKneeTrackerTransform;
    // public Transform rightKneeTrackerTransform;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {
            // 왼발
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
            animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootTrackerTransform.position);
            animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootTrackerTransform.rotation);

            // 오른발
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
            animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootTrackerTransform.position);
            animator.SetIKRotation(AvatarIKGoal.RightFoot, rightFootTrackerTransform.rotation);

            // 무릎
            // animator.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, 1f);
            // animator.SetIKHintPositionWeight(AvatarIKHint.RightKnee, 1f);
            // animator.SetIKHintPosition(AvatarIKHint.LeftKnee, leftKneeTrackerTransform.position);
            // animator.SetIKHintPosition(AvatarIKHint.RightKnee, rightKneeTrackerTransform.position);
        }
    }
}
