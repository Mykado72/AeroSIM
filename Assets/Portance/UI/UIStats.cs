using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStats : MonoBehaviour {
    public Text Poid;
    public Text Vitesse;
    public Text PortanceAileG;
    public Text PortanceAileD;
    public Text Moteur;
    public PlaneControleur plane;
    public SurfacePortante surfPortAileG;
    public SurfacePortante surfPortAileD;

    // Use this for initialization
    void Start () {
        Poid.text = "Poid : " + plane.rg.mass + " Kg"; ;

    }
	
	// Update is called once per frame
	void Update () {
        Vitesse.text = "Vitesse frontale : "+Mathf.RoundToInt(plane.forwardSpeed*3.6f).ToString()+ " Km/h";
        PortanceAileG.text = "Portance Aile G : " + Mathf.RoundToInt(surfPortAileG.portance_verticale).ToString() + " Newtons";
        PortanceAileD.text = "Portance Aile D : " + Mathf.RoundToInt(surfPortAileD.portance_verticale).ToString() + " Newtons";
        Moteur.text = "Puissance Moteur : "+ Mathf.RoundToInt(( plane.EnginePower/ plane.m_MaxEnginePower) *100).ToString()+" %";
    }
}
