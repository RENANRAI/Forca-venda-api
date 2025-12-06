namespace ForcaVendas.Api.Domain.Entities;

public class Pedido
{
    public Guid Id { get; set; }
    public int Numero { get; set; }
    public Guid ClienteId { get; set; }
    public DateTime DataEmissao { get; set; }
    public byte Status { get; set; }  // 0=Rascunho,1=Pendente,2=Enviado,3=Erro
    public string? Observacao { get; set; }
    public decimal ValorTotal { get; set; }
    public string? UsuarioVendedor { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }

    public Cliente Cliente { get; set; } = default!;
    public ICollection<PedidoItem> Itens { get; set; } = new List<PedidoItem>();
}
