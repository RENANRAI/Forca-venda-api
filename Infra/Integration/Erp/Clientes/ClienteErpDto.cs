namespace ForcaVendas.Api.Infra.Integration.Erp.Clientes;

public class ClienteErpDto
{
    public string CodigoErp { get; set; } = default!;
    public string Nome { get; set; } = default!;
    public string Documento { get; set; } = default!;
    public string? Cidade { get; set; }
    public string? Uf { get; set; }
}
