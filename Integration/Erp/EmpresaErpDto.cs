namespace ForcaVendas.Api.Integration.Erp;

public class EmpresaErpDto
{
    public string CodigoErp { get; set; } = default!;
    public string Nome { get; set; } = default!;
    public string? Cnpj { get; set; }
}
