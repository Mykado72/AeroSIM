using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlaneInputController : MonoBehaviour {
    public float RollInput;
    public float PitchInput;
    public float YawInput;
    public float ThrottleInput;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        RollInput = CrossPlatformInputManager.GetAxis("Horizontal")  ;
        PitchInput = CrossPlatformInputManager.GetAxis("Pitch")  ;
        YawInput = CrossPlatformInputManager.GetAxis("Yaw") ;
        ThrottleInput = CrossPlatformInputManager.GetAxis("Vertical")  ;
        ClampInputs();
    }

    private void ClampInputs()
    {
        // clamp the inputs to -1 to 1 range
        RollInput = Mathf.Clamp(RollInput, -1, 1);
        PitchInput = Mathf.Clamp(PitchInput, -1, 1);
        YawInput = Mathf.Clamp(YawInput, -1, 1);
        ThrottleInput = Mathf.Clamp(ThrottleInput, -1, 1);
    }


}
