namespace ForcaVendas.Api.Infra.Integration.Erp.EmpresasFiliais;

public class EmpresasFiliaisErpDto
{
    public int CodEmp { get; set; } = default!;
    public int CodFil { get; set; } = default!;
    public string? NomEmp { get; set; }
    public string? NomFil { get; set; }
}