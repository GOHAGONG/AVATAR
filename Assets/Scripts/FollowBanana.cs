using UnityEngine;

public class FollowBanana : MonoBehaviour
{
    [SerializeField] private Transform ControllerRig;
    [SerializeField] private Transform playerHip;
    [SerializeField] private Vector3 controllerOffset = new Vector3(0, 0, 0); // HMD의 몸에 대한 상대 위치
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 초기 위치 세팅
        ControllerRig.position = playerHip.position + controllerOffset;
    }

    // Update is called once per frame
    void Update()
    {
        // Controller 위치를 playerBody 기준으로 따라가게 함
        ControllerRig.position = playerHip.position + controllerOffset;
    }
}
