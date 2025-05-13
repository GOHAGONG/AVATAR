using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;           // 따라갈 대상 (플레이어)
    public Vector3 offset = new Vector3(0f, 5f, -7f); // 거리/각도
    public float followSpeed = 5f;     // 따라가는 속도
    public float lookSpeed = 5f;       // 회전 속도

    void LateUpdate()
    {
        if (target == null) return;

        // 부드럽게 위치 이동
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // 부드럽게 회전하여 플레이어 바라보기
        //Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position);
        //transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, lookSpeed * Time.deltaTime);
    }
}
