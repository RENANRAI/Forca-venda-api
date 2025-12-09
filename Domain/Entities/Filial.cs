namespace ForcaVendas.Api.Domain.Entities;

public class Filial
{
      // ajste fillial 
    public Guid Id { get; set; }
    public int CodEmp { get; set; }     // Empresa
    public int CodFil { get; set; }     // Filial
    public string NomFil { get; set; } = default!;
    public string? NumCgc { get; set; }

    public string? EndFil { get; set; }
    public string? NenFil { get; set; }
    public string? BaiFil { get; set; }
    public string? CidFil { get; set; }
    public string? SigUfs { get; set; }
    public string? CepFil { get; set; }

    public bool SitFil { get; set; } = true;
    public DateTime DatCri { get; set; }
    public DateTime? DatAtu { get; set; }

    public Empresa? Empresa { get; set; }
}
