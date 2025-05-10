using UnityEngine;
using System.Collections.Generic;
using Valve.VR;

public class ExhaustCheck : MonoBehaviour
{
    [Header("Exhaust Count")]
    public int exhaust_count = 0;
    public List<int> exhaustHistory = new List<int>();

    [Header("Components")]
    public bool isExhausted = false;

    [Header("VR Input")]
    public SteamVR_Action_Boolean countAction;
    public SteamVR_Input_Sources leftInputSource = SteamVR_Input_Sources.LeftHand;

    void OnEnable()
    {
        // 이벤트 등록
        if (countAction != null)
        {
            countAction.AddOnStateUpListener(OnTriggerReleased, leftInputSource);
        }
    }

    void OnDisable()
    {
        // 이벤트 해제
        if (countAction != null)
        {
            countAction.RemoveOnStateUpListener(OnTriggerReleased, leftInputSource);
        }
    }

    private void OnTriggerReleased(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        exhaust_count++;
        Debug.Log("Trigger released. Exhaust count: " + exhaust_count);
    }

    public void ResetExhaustCount()
    {
        exhaustHistory.Add(exhaust_count);
        exhaust_count = 0;
        Debug.Log("Exhaust count reset.");
    }
}
