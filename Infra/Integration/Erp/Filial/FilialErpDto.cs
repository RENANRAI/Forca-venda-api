namespace ForcaVendas.Api.Infra.Integration.Erp.Filiais;

public class FilialErpDto
{
    public int CodEmp { get; set; }
    public int CodFil { get; set; }
    public string Nome { get; set; } = default!;
    public string? Cnpj { get; set; }
    public string? Endereco { get; set; }
    public string? Numero { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }
    public string? Uf { get; set; }
    public string? Cep { get; set; }
}
