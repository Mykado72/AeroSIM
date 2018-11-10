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
                pointApplicationForce.position= new Vector3(pointApplicationForce.position.x, rg.transform.position.y+rg.centerOfMass.y, rg.transform.position.z+rg.centerOfMass.z);
                break;
            case SurfacePortante.Type.Elevator:
                pointApplicationForce.position = new Vector3(rg.transform.position.x+rg.centerOfMass.x, rg.transform.position.y+rg.centerOfMass.y, pointApplicationForce.position.z);
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
        vitesseRelativeFrontale = Mathf.Abs(vitesseRelativeFrontale);
        vitesseRelativeLaterale = Mathf.Abs(vitesseRelativeLaterale);
        vitesseAscensionelle = Mathf.Abs(vitesseAscensionelle);
        switch (type)
        {
            case SurfacePortante.Type.Aileron:
                vitesse_relative = (vitesseRelativeFrontale);
                pression_dynamique = (masse_volumique_air * vitesse_relative * vitesse_relative) / 2;
                portance_verticale = pression_dynamique * surface * coef_portance;

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
            case SurfacePortante.Type.Elevator:
                vitesse_relative = (vitesseRelativeFrontale+ vitesseRelativeLaterale * 0.25f + vitesseAscensionelle + 0.1f) / 2;
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
                vitesse_relative = (vitesseRelativeFrontale + vitesseRelativeLaterale) / 2;
                pression_dynamique = (masse_volumique_air * vitesse_relative * vitesse_relative) / 2;
                portance_verticale = pression_dynamique * surface * coef_portance;
                vector_portance = -rg.transform.right * portance_verticale;
                rg.AddForceAtPosition(vector_portance/10, pointApplicationForce.position, ForceMode.Impulse);
                Debug.DrawRay(pointApplicationForce.position, vector_portance / 10);
                break;
        }
        // Debug.DrawLine(pointApplicationForce.position, pointApplicationForce.position + vector_portance/100);
    }
}