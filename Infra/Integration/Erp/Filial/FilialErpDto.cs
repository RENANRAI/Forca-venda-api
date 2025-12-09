namespace ForcaVendas.Api.Infra.Integration.Erp.Filiais;

public class FilialErpDto
{
    public int CodEmp { get; set; }
    public int CodFil { get; set; }

    public string NomFil { get; set; } = default!;   // nomFil
    public string? NumCgc { get; set; }             // numCgc
    public string? EndFil { get; set; }         // endFil
    public string? NenFil { get; set; }           // nenFil
    public string? BaiFil { get; set; }           // baiFil
    public string? CidFil { get; set; }           // cidFil
    public string? SigUfs { get; set; }               // sigUfs
    public string? CepFil { get; set; }              // cepFil
}
