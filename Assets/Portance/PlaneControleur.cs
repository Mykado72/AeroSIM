using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneControleur : MonoBehaviour
{

    [SerializeField] public float m_MaxEnginePower = 40f;        // The maximum output of the engine.
    [SerializeField] private float m_ThrottleChangeSpeed = 0.3f;  // The speed with which the throttle changes.
    [SerializeField] private float m_DragIncreaseFactor = 0.004f; // how much drag should increase with speed.
    [SerializeField] public Rigidbody rg;
    [SerializeField] public float forwardSpeed;
    [SerializeField] private float edgeSpeed;
    [SerializeField] private float ascensionalSpeed;
    [SerializeField] private SurfacePortante[] surfacesPortantes;
    [SerializeField] private VitesseRelative vent;
    [SerializeField] public PlaneInputController PlaneInputs;
    [SerializeField] private SurfaceMobile[] surfacesMobiles; // Collection of control surfaces.
    [SerializeField] private float m_RollEffect = 1f;             // The strength of effect for roll input.
    [SerializeField] private float m_PitchEffect = 1f;            // The strength of effect for pitch input.
    [SerializeField] private float m_YawEffect = 0.2f;            // The strength of effect for yaw input.

    public float Throttle { get; private set; }
    public float EnginePower;
    private float m_OriginalDrag;         // The drag when the scene starts.
    private float m_OriginalAngularDrag;  // The angular drag when the scene starts.
    private float m_AeroFactor;
    public Vector3 localVelocity;
    public Transform CG;
    private float rollTorque;
    private float pitchTorque;
    private float yawTorque;
    // Use this for initialization
    void Start()
    {
        rg.centerOfMass = CG.localPosition;
        // CG.localPosition = rg.centerOfMass;
        m_OriginalAngularDrag = rg.angularDrag;
        foreach (SurfacePortante surfacePortante in surfacesPortantes)
        {
            surfacePortante.rg = rg;
        }
        // Store the original local rotation of each surface, so we can rotate relative to this
        foreach (SurfaceMobile surfaceMobile in surfacesMobiles)
        {
            surfaceMobile.originalLocalRotation = surfaceMobile.pivot.localRotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateSpeeds();
        CalculateDrag();
        ControlThrottle();
        CalculateLinearForces();
        CalculateTorque();
        foreach (SurfacePortante surfacePortante in surfacesPortantes)
        {
            surfacePortante.vitesseRelativeFrontale = forwardSpeed;
            surfacePortante.vitesseRelativeLaterale = edgeSpeed;
            surfacePortante.vitesseAscensionelle= ascensionalSpeed;
            switch (surfacePortante.type)
            {
                case SurfacePortante.Type.Aileron:
                    {
                        // Ailerons rotate around the x axis, according to the plane's roll input
                        if (Mathf.Sign(rg.transform.up.y) > 0)
                            surfacePortante.surface = surfacePortante.surface_initiale + (PlaneInputs.RollInput * surfacePortante.amount);
                        else
                            surfacePortante.surface = surfacePortante.surface_initiale - (PlaneInputs.RollInput * surfacePortante.amount);
                        break;
                    }
                case SurfacePortante.Type.Elevator:
                    {
                        // Elevators rotate negatively around the x axis, according to the plane's pitch input
                        if (Mathf.Sign(rg.transform.up.y) > 0)
                            surfacePortante.surface = surfacePortante.surface_initiale + (PlaneInputs.PitchInput * surfacePortante.amount);
                        else
                            surfacePortante.surface = surfacePortante.surface_initiale - (PlaneInputs.PitchInput * surfacePortante.amount);
                        break;
                    }
                case SurfacePortante.Type.Rudder:
                    {
                        // Rudders rotate around their y axis, according to the plane's yaw input
                        surfacePortante.surface = PlaneInputs.YawInput*surfacePortante.amount;
                        break;
                    }
            }
        }

        foreach (SurfaceMobile surfaceMobile in surfacesMobiles)
        {
            switch (surfaceMobile.type)
            {
                case SurfaceMobile.Type.Aileron:
                    {
                        // Ailerons rotate around the x axis, according to the plane's roll input
                        Quaternion rotation = Quaternion.Euler(surfaceMobile.amount * PlaneInputs.RollInput, 0f, 0f);
                        RotateSurface(surfaceMobile, rotation);
                        break;
                    }
                case SurfaceMobile.Type.Elevator:
                    {
                        // Elevators rotate negatively around the x axis, according to the plane's pitch input
                        Quaternion rotation = Quaternion.Euler(surfaceMobile.amount * -PlaneInputs.PitchInput, 0f, 0f);
                        RotateSurface(surfaceMobile, rotation);
                        break;
                    }
                case SurfaceMobile.Type.Rudder:
                    {
                        // Rudders rotate around their y axis, according to the plane's yaw input
                        Quaternion rotation = Quaternion.Euler(0f, surfaceMobile.amount * PlaneInputs.YawInput, 0f);
                        RotateSurface(surfaceMobile, rotation);
                        break;
                    }
                case SurfaceMobile.Type.RuddervatorPositive:
                    {
                        // Ruddervators are a combination of rudder and elevator, and rotate
                        // around their z axis by a combination of the yaw and pitch input
                        float r = PlaneInputs.YawInput + PlaneInputs.PitchInput;
                        Quaternion rotation = Quaternion.Euler(0f, 0f, surfaceMobile.amount * r);
                        RotateSurface(surfaceMobile, rotation);
                        break;
                    }
                case SurfaceMobile.Type.RuddervatorNegative:
                    {
                        // ... and because ruddervators are "special", we need a negative version too. >_<
                        float r = PlaneInputs.YawInput - PlaneInputs.PitchInput;
                        Quaternion rotation = Quaternion.Euler(0f, 0f, surfaceMobile.amount * r);
                        RotateSurface(surfaceMobile, rotation);
                        break;
                    }
            }
        }
    }

    private void RotateSurface(SurfaceMobile surfaceMobile, Quaternion rotation)
    {
        // Create a target which is the surface's original rotation, rotated by the input.
        Quaternion target = surfaceMobile.originalLocalRotation * rotation;

        // Slerp the surface's rotation towards the target rotation.
        surfaceMobile.pivot.localRotation = Quaternion.Slerp(surfaceMobile.pivot.localRotation, target,
                                                           surfaceMobile.m_Smoothing * Time.deltaTime);
    }

    private void CalculateSpeeds()
    {
        // Forward speed is the speed in the planes's forward direction (not the same as its velocity, eg if falling in a stall)
        localVelocity = transform.InverseTransformDirection(rg.velocity);
        
        forwardSpeed = localVelocity.z;
        edgeSpeed = localVelocity.x;
        ascensionalSpeed= localVelocity.y;
    }

    private void ControlThrottle()
    {
        // override throttle if immobilized
        /* if (m_Immobilized)
        {
            ThrottleInput = -0.5f;
        }
        */
        // Adjust throttle based on throttle input (or immobilized state)
        Throttle = Mathf.Clamp01(Throttle + PlaneInputs.ThrottleInput * Time.deltaTime * m_ThrottleChangeSpeed);

        // current engine power is just:
        EnginePower = Throttle * m_MaxEnginePower;
    }

    private void CalculateLinearForces()
    {
        // Now calculate forces acting on the aeroplane:
        // we accumulate forces into this variable:
        rollTorque = PlaneInputs.RollInput;
        yawTorque = PlaneInputs.YawInput;
        pitchTorque = PlaneInputs.PitchInput;
        var forces = Vector3.zero;
        // Add the engine power in the forward direction
        forces += EnginePower * transform.forward;
        // var liftDirection = Vector3.Cross(rg.velocity, transform.right).normalized;
        // forces += liftDirection;
        Debug.DrawRay(transform.position, forces/20);
        // Apply the calculated forces to the the Rigidbody
        rg.AddForce(forces, ForceMode.Acceleration);
    }
       
    private void CalculateTorque()
    {
        // We accumulate torque forces into this variable:
        var torque = Vector3.zero;
        // Add torque for the pitch based on the pitch input.
        torque += pitchTorque * m_PitchEffect * transform.right;
        // Add torque for the yaw based on the yaw input.
        torque += yawTorque * m_YawEffect * transform.up;
        // Add torque for the roll based on the roll input.
        torque += -rollTorque * m_RollEffect * transform.forward;
        // Add torque for banked turning.
        // torque += m_BankedTurnAmount * m_BankedTurnEffect * transform.up;
        // The total torque is multiplied by the forward speed, so the controls have more effect at high speed,
        // and little effect at low speed, or when not moving in the direction of the nose of the plane
        // (i.e. falling while stalled)
        Debug.Log(torque);
        rg.AddTorque(torque*1000); // * ForwardSpeed * m_AeroFactor);
    }

    private void CalculateDrag()
    {
        // Air brakes work by directly modifying drag. This part is actually pretty realistic!
        rg.drag = m_OriginalDrag + rg.velocity.magnitude * m_DragIncreaseFactor * 2;
        // Forward speed affects angular drag - at high forward speed, it's much harder for the plane to spin
        rg.angularDrag = m_OriginalAngularDrag+ m_DragIncreaseFactor * forwardSpeed;
    }

}