namespace ForcaVendas.Api.Models;

public class ClienteDto
{
    public string Id { get; set; } = default!;
    public string Nome { get; set; } = default!;
    public string Documento { get; set; } = default!;
    public string? Cidade { get; set; }
    public string? Uf { get; set; }
}
