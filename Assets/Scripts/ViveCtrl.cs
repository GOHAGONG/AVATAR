using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ViveCtrl : MonoBehaviour
{
    public SteamVR_Action_Boolean Grab;

    void ClickGrab(){
        if (Grab.GetStateDown(SteamVR_Input_Sources.Any))
        {
            Debug.Log("ok");
        }
    }
    private void FixedUpdate(){
        ClickGrab();
    }
}
