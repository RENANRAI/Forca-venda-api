namespace Forca_venda_api.Domain.Dtos;

public class EmpresaDto
{
    public string Id { get; set; } = default!;
    public string CodigoErp { get; set; } = default!;
    public string Nome { get; set; } = default!;
    public string? Cnpj { get; set; }
}
