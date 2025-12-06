namespace ForcaVendas.Api.Infra.Integration.Erp.Empresa;

public class EmpresaErpDto
{
    public int CodEmp { get; set; } = default!;
    public string Nome { get; set; } = default!;
    public string? Cnpj { get; set; }
}
