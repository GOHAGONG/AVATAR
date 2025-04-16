using UnityEngine;

public class RotateAvatarWithHead : MonoBehaviour
{
    public Transform headTransform;      // HMD 또는 Head 카메라
    public Transform avatarRoot;         // 아바타 전체 몸체

    void LateUpdate()
    {
        if (headTransform == null || avatarRoot == null)
            return;

        // HMD의 회전에서 Y축(수평 방향)만 추출
        Vector3 headEuler = headTransform.rotation.eulerAngles;
        Quaternion targetRotation = Quaternion.Euler(0f, headEuler.y, 0f);

        // 아바타 몸체의 회전 적용
        avatarRoot.rotation = targetRotation;
    }
}
