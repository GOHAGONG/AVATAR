using UnityEngine;

public class LockLocalRotation : MonoBehaviour
{
    private Quaternion initialLocalRotation;

    void Start()
    {
        // 현재 로컬 회전을 기억
        initialLocalRotation = transform.localRotation;
    }

    void LateUpdate()
    {
        // 부모의 회전은 자연스럽게 적용되므로,
        // 자식으로서의 로컬 회전을 매 프레임 고정시킴
        transform.localRotation = initialLocalRotation;
    }
}