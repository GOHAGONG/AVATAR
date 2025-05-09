using UnityEngine;

public class UIFollowVRHead : MonoBehaviour
{
    public Transform hmdCamera;   // HMD 카메라 (예: CameraRig > Camera)
    public float distance = 2.0f;
    public Vector3 offset = new Vector3(0, -0.3f, 0); // 약간 아래로

    void LateUpdate()
    {
        if (hmdCamera == null) return;

        // 시야 방향 앞쪽 위치 계산
        Vector3 targetPos = hmdCamera.position + hmdCamera.forward * distance + offset;
        transform.position = targetPos;

        // 카메라 바라보도록 회전
        transform.LookAt(hmdCamera);
        transform.Rotate(0, 180, 0); // 텍스트가 반대 방향으로 나오지 않도록 반전
    }
}
