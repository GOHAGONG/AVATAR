using UnityEngine;

public class CameraRot : MonoBehaviour
{
    [SerializeField] private Transform playerBody; // 캐릭터 오브젝트
    [SerializeField] private float mouseSensitivity = 200f;

    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // 마우스 커서 잠금
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        //float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 상하 회전 제한
        //xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // 카메라 자체 회전 (상하)
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 캐릭터 몸통 회전 (좌우)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}