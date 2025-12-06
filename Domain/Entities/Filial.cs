namespace ForcaVendas.Api.Domain.Entities;

public class Filial
{
    public Guid Id { get; set; }
    public int CodEmp { get; set; }     // Empresa
    public int CodFil { get; set; }     // Filial
    public string Nome { get; set; } = default!;
    public string? Cnpj { get; set; }

    public string? Endereco { get; set; }
    public string? Numero { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }
    public string? Uf { get; set; }
    public string? Cep { get; set; }

    public bool Ativo { get; set; } = true;
    public DateTime DatCri { get; set; }
    public DateTime? DatAtu { get; set; }
}
