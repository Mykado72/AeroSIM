using System;


public class Avion : IObjectVolant {

    public int NbPropulseurs { get; set; }

    public float Poid { get ; set ; }
    public EnsembleDailes[] EnsembleDailes { get; set; }
    public Fuselage Fuselage { get; set; }
    public void Voler()
    {
        Console.WriteLine("Je vole grâce à " + NbPropulseurs + " ailes");
    }
}
