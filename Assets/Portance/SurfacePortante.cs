using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfacePortante : MonoBehaviour {

  //  La portance verticale
  //  en newtons(N) d'une aile vaut :
    [SerializeField] public float portance_verticale; // en Newtons
    [SerializeField] private Vector3 vector_portance;
    [SerializeField] private float masse_volumique_air= 1.25f ; // masse volumique du fluide traversé en kg/m3
                                                             // air à 0°C = 1,293 à 20 °C = 1,204        
    [SerializeField] private float vitesse_relative; // vitesse verticale en m/s
    [SerializeField] public float surface; // surface en m^2
    [SerializeField] public float surface_initiale; // surface en m^2
    [SerializeField] private float coef_portance; //  coefficient de portance
    [SerializeField] private float pression_dynamique; // = (masse_volumique_air*vitesse_vericale^2)/2

    [SerializeField] public Rigidbody rg;
    [SerializeField] private Transform pointApplicationForce;
    [SerializeField] public float vitesseRelativeFrontale;
    [SerializeField] public float vitesseRelativeLaterale;
    [SerializeField] public float vitesseAscensionelle;
    [SerializeField] public float vitesseChutte;
    public float incidence;
    public Type type; // The type of control surface.
    public float amount; // The amount by which they can rotate.
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
    void Start()
    {
        switch (type)
        {
            case SurfacePortante.Type.Aileron:
                // pointApplicationForce.localPosition= new Vector3(pointApplicationForce.localPosition.x, pointApplicationForce.localPosition.y, pointApplicationForce.localPosition.z);
                pointApplicationForce.localPosition = new Vector3(pointApplicationForce.localPosition.x, rg.centerOfMass.y, rg.centerOfMass.z);
                break;
            case SurfacePortante.Type.Elevator:
                pointApplicationForce.localPosition = new Vector3(rg.centerOfMass.x, rg.centerOfMass.y, rg.centerOfMass.z);
                break;
        }
    }
	
	// Update is called once per frame
	void Update () {
        /*
        vitesse_verticale = vitesseRelative;
        pression_dynamique = (masse_volumique_air * vitesse_verticale * vitesse_verticale) / 2;
        portance_verticale = pression_dynamique * surface * coef_portance;
        */    
    }

    private void FixedUpdate()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(rg.velocity);
        float altitude = rg.transform.position.y;
        masse_volumique_air = 1.25f * (1-(altitude / 10000));
        // Vector3 localVelocity = rg.velocity;        
        vitesseRelativeFrontale = localVelocity.z;
        vitesseRelativeLaterale = localVelocity.x;
        vitesseAscensionelle = localVelocity.y;
        vitesseRelativeFrontale = Mathf.Clamp(vitesseRelativeFrontale,0,100);
        vitesseRelativeLaterale = Mathf.Abs(vitesseRelativeLaterale);
        vitesseChutte = Mathf.Clamp(vitesseAscensionelle, -11, 0);

        switch (type)
        {
            case SurfacePortante.Type.Aileron:
                // incidence = 1/(0.000000001f+Vector3.Angle(localVelocity, transform.forward));
                incidence = Mathf.Abs((Vector3.Angle(new Vector3(0,0,vitesseRelativeFrontale), transform.forward)-90) % 90)/90;
                vitesse_relative = (vitesseRelativeFrontale);
                pression_dynamique = (masse_volumique_air * vitesse_relative * vitesse_relative) / 2;
                portance_verticale = pression_dynamique * surface * (coef_portance*(0.5f+incidence*0.25f));

                if (Mathf.Sign(rg.transform.up.y)>0) 
                {
                    vector_portance = rg.transform.up * portance_verticale;
                    
                }
                else // aile à l'envers
                {
                    vector_portance = -rg.transform.up * portance_verticale;
                }
                rg.AddForceAtPosition(vector_portance/10, pointApplicationForce.position, ForceMode.Impulse);
                Debug.DrawRay(pointApplicationForce.position, -vector_portance / 10);

                break;
            /* case SurfacePortante.Type.Elevator:
                incidence = Vector3.Angle(localVelocity, transform.forward);
                vitesse_relative = (vitesseRelativeFrontale); // + vitesseRelativeLaterale * 0.25f + vitesseAscensionelle + 0.1f) / 2;
                pression_dynamique = (masse_volumique_air * vitesse_relative * vitesse_relative) / 2;
                portance_verticale = pression_dynamique * surface * coef_portance;

                if (Mathf.Sign(rg.transform.up.y) > 0)
                {
                    vector_portance = rg.transform.up * portance_verticale;
                }
                else
                {
                    vector_portance = -rg.transform.up * portance_verticale;
                }
                rg.AddForceAtPosition(vector_portance/10, pointApplicationForce.position, ForceMode.Impulse);
                Debug.DrawRay(pointApplicationForce.position, -vector_portance/10);
                break;
            case SurfacePortante.Type.Rudder:
                incidence = Vector3.Angle(localVelocity, -transform.forward);
                vitesse_relative = (vitesseRelativeFrontale + vitesseRelativeLaterale) / 2;
                pression_dynamique = (masse_volumique_air * vitesse_relative * vitesse_relative) / 2;
                portance_verticale = pression_dynamique * surface * coef_portance;
                vector_portance = -rg.transform.right * portance_verticale;
                // rg.AddForceAtPosition(vector_portance/10, pointApplicationForce.position, ForceMode.Impulse);
                Debug.DrawRay(pointApplicationForce.position, vector_portance / 10);
                break;
            */
        }
        // Debug.DrawLine(pointApplicationForce.position, pointApplicationForce.position + vector_portance/100);
    }
}