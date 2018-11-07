public interface IObjectVolant   {
    int NbPropulseurs { get; set; }
    float Poid { get; set; }
    EnsembleDailes[] EnsembleDailes { get; set; }
    void Voler();
}