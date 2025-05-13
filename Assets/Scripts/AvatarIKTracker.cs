using UnityEngine;
using System.Collections;
using Valve.VR;

[RequireComponent(typeof(Animator))]
public class AvatarIKTracker : MonoBehaviour
{
    public Transform leftTrackerTransform;
    public Transform rightTrackerTransform;

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
