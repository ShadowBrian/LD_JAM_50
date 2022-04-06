using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class SetXROrigin : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InputDevices.GetDeviceAtXRNode(XRNode.Head).subsystem.TrySetTrackingOriginMode(TrackingOriginModeFlags.Floor);
        //SubsystemManager.GetInstances<XRInputSubsystem>(s_InputSubsystems);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
