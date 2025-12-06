namespace ForcaVendas.Api.Domain.Entities;

public class EmpresaFilialIntegrada
{
    public Guid Id { get; set; }

    // Códigos ERP
    public int CodEmp { get; set; } = default!;
    public int CodFil { get; set; } = default!;

   // Informações opcionais
     public string? NomEmp { get; set; }
    public string? NomFil { get; set; }

    public bool SitReg { get; set; } = true;

    public DateTime DatCri { get; set; }
    public DateTime? DatAtu { get; set; }
}
