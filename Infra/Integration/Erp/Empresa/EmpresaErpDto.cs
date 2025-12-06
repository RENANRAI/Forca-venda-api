namespace Forca_venda_api.Infra.Integration.Erp.Empresa;

public class EmpresaErpDto
{
    public string CodigoErp { get; set; } = default!;
    public string Nome { get; set; } = default!;
    public string? Cnpj { get; set; }
}
