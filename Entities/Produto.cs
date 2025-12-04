namespace ForcaVendas.Api.Entities;

public class Produto
{
    public Guid Id { get; set; }
    public string CodigoExterno { get; set; } = default!;
    public string Nome { get; set; } = default!;
    public string? Unidade { get; set; }
    public decimal PrecoBase { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }

    public ICollection<PedidoItem> Itens { get; set; } = new List<PedidoItem>();
}
