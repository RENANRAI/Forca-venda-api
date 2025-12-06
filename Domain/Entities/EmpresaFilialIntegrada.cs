namespace ForcaVendas.Api.Domain.Entities;

public class EmpresaFilialIntegrada
{
    public Guid Id { get; set; }

    // Códigos ERP
    public string CodEmp { get; set; } = default!;
    public string CodFil { get; set; } = default!;

    // Informações opcionais
   /* public string? NomeEmpresa { get; set; }
    public string? NomeFilial { get; set; }*/

    public bool SitReg { get; set; } = true;

    public DateTime DatCri { get; set; }
    public DateTime? DatAtu { get; set; }
}
