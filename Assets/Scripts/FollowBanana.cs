using UnityEngine;

public class FollowBanana : MonoBehaviour
{
    [SerializeField] private Transform ControllerRigLeft;
    [SerializeField] private Transform ControllerRigRight;
    [SerializeField] private Transform playerHip;
    [SerializeField] private Vector3 controllerOffsetLeft = new Vector3(0, 0, 0); // HMD의 몸에 대한 상대 위치
    [SerializeField] private Vector3 controllerOffsetRight = new Vector3(0, 0, 0);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 초기 위치 세팅
        ControllerRigLeft.position = playerHip.position + controllerOffsetLeft;
        ControllerRigRight.position = playerHip.position + controllerOffsetRight;
    }

    // Update is called once per frame
    void Update()
    {
        // Controller 위치를 playerBody 기준으로 따라가게 함
        ControllerRigLeft.position = playerHip.position + controllerOffsetLeft;
        ControllerRigRight.position = playerHip.position + controllerOffsetRight;
    }
}
