namespace ForcaVendas.Api.Domain.Entities;
public class Empresa
{

      
    public Guid Id { get; set; }
    public int CodEmp { get; set; } = default!; // codEmp
    public string Nome { get; set; } = default!;      // nome fantasia / razão social
    public string? Cnpj { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime DatCri { get; set; }
    public DateTime? DatAtu { get; set; }

    // Se quiser depois: uma empresa tem vários clientes
    //public ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
}
