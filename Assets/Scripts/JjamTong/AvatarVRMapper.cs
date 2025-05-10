using UnityEngine;
using System.Collections;
using Valve.VR;

public class AvatarVRMapper : MonoBehaviour
{
    [Header("VR Components")]
    public Transform hmdTransform;           // CameraRig의 Camera
    public Transform leftControllerTransform; // CameraRig의 왼손 컨트롤러
    public Transform rightControllerTransform;// CameraRig의 오른손 컨트롤러

    [Header("Avatar Bones")]
    public Transform headBone;
    public Transform leftHandBone;
    public Transform rightHandBone;

    [Header("CameraRig Movement")]
    public Transform cameraRigRoot;          // 이동시킬 CameraRig의 루트

    void Update()
    {
        // 1. HMD 위치를 아바타 머리에 동기화
        headBone.position = hmdTransform.position;
        headBone.rotation = hmdTransform.rotation;

        // 2. 손 위치를 컨트롤러에 동기화
        leftHandBone.position = leftControllerTransform.position;
        leftHandBone.rotation = leftControllerTransform.rotation;

        rightHandBone.position = rightControllerTransform.position;
        rightHandBone.rotation = rightControllerTransform.rotation;

        // 3. 이동 구현 예시 (WASD 입력 기준, 실제 VR에서는 trackpad 등 입력에 따라)
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(moveX, 0, moveZ);

        cameraRigRoot.position += moveDirection * Time.deltaTime * 1.5f;
    }
}

