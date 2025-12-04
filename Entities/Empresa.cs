namespace ForcaVendas.Api.Entities;

public class Empresa
{
    public Guid Id { get; set; }
    public string CodigoErp { get; set; } = default!; // codEmp
    public string Nome { get; set; } = default!;      // nome fantasia / razão social
    public string? Cnpj { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }

    // Se quiser depois: uma empresa tem vários clientes
    public ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
}
