using UnityEngine;
using System.Collections;
using Valve.VR;

[RequireComponent(typeof(Animator))]
public class AvatarIKController : MonoBehaviour
{
    public Transform leftControllerTransform;
    public Transform rightControllerTransform;

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

        }
    }
}
