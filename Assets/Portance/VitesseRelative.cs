using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VitesseRelative : MonoBehaviour {
    [SerializeField] public float vitesse_vent; // en m/s
    // [SerializeField] private ParticleSystem particleS;
    [SerializeField] private WindZone windzone;
    // Use this for initialization
    void Start () {
        vitesse_vent = 35.5f;  //5.5m/s
        // var main = particleS.main;
        // main.startSpeed = 0.01f;
    }
	
	// Update is called once per frame
	void Update () {
        vitesse_vent = vitesse_vent + Input.GetAxis("Vertical") * Time.deltaTime;
        vitesse_vent = Mathf.Clamp(vitesse_vent, 0, 10000);
        // var main = particleS.main;
        // main.startSpeed = vitesse_vent;
        windzone.windMain = vitesse_vent / 2;

    }
}
