using UnityEngine;

public class CameraRot : MonoBehaviour
{
    [SerializeField] private Transform playerBody; // 캐릭터 오브젝트
    [SerializeField] private float mouseSensitivity = 200f;

    private float xRotation = 0f;

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

        // 캐릭터 몸통 회전 (좌우)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}