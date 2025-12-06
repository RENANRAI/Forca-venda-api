namespace ForcaVendas.Api.Domain.Entities;

public class EmpresaFilialIntegradaDto
{
    public Guid Id { get; set; }

    public string CodEmp { get; set; } = default!;
    public string CodFil { get; set; } = default!;

    public string? NomEmp { get; set; }
    public string? NomFil { get; set; }

    public bool SitReg { get; set; } = true;

    public DateTime DatCri { get; set; }
    public DateTime? DatAtu { get; set; }
}
