using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceMobile : MonoBehaviour {
    public Transform pivot; // The transform of the control surface.
    public float amount; // The amount by which they can rotate.
    public Type type; // The type of control surface.
    [SerializeField] public float m_Smoothing = 5f; // The smoothing applied to the movement of control surfaces.
    [HideInInspector] public Quaternion originalLocalRotation; // The rotation of the surface at the start.

    public enum Type // Flaps differ in position and rotation and are represented by different types.
    {
        Aileron, // Horizontal flaps on the wings, rotate on the x axis.
        Elevator, // Horizontal flaps used to adjusting the pitch of a plane, rotate on the x axis.
        Rudder, // Vertical flaps on the tail, rotate on the y axis.
        RuddervatorNegative, // Combination of rudder and elevator.
        RuddervatorPositive, // Combination of rudder and elevator.
        Fuselage,
    }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void RotateSurface(SurfaceMobile surfaceMobile, Quaternion rotation)
    {
        // Create a target which is the surface's original rotation, rotated by the input.
        Quaternion target = surfaceMobile.originalLocalRotation * rotation;

        // Slerp the surface's rotation towards the target rotation.
        surfaceMobile.transform.localRotation = Quaternion.Slerp(surfaceMobile.pivot.localRotation, target,
                                                           m_Smoothing * Time.deltaTime);
    }
}
