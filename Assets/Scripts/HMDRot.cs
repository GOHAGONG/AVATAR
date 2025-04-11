using UnityEngine;

public class HMDRot : MonoBehaviour
{
    [SerializeField] private Transform hmdCamera;
    [SerializeField] private Transform hmdCameraRig;
    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform playerHead;
    [SerializeField, Range(0f, 1f)] private float sensitivity = 0.5f;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 1.7f, 0); // HMD의 몸에 대한 상대 위치

    private float lastYaw;

    void Start()
    {
        float initialYaw = hmdCamera.eulerAngles.y;
        playerBody.rotation = Quaternion.Euler(0, initialYaw, 0);
        lastYaw = initialYaw;

        // 초기 위치 세팅
        hmdCameraRig.position = playerHead.position + cameraOffset;
    }

    void Update()
    {
        float currentYaw = hmdCamera.eulerAngles.y;
        float deltaYaw = Mathf.DeltaAngle(lastYaw, currentYaw);
        float adjustedYaw = deltaYaw * sensitivity;

        // 회전 적용
        playerBody.Rotate(0, adjustedYaw, 0);

        // hmdCamera 위치를 playerBody 기준으로 따라가게 함
        hmdCameraRig.position = playerHead.position + cameraOffset;

        // 보정: 회전 중첩 방지
        hmdCamera.localRotation = Quaternion.Euler(
            hmdCamera.localRotation.eulerAngles.x,
            hmdCamera.localRotation.eulerAngles.y - adjustedYaw,
            hmdCamera.localRotation.eulerAngles.z
        );

        lastYaw = currentYaw;
    }
}
