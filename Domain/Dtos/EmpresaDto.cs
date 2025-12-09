namespace Forca_venda_api.Domain.Dtos;

public class EmpresaDto
{
    public string Id { get; set; } = default!;
    public int CodEmp { get; set; } = default!;
    public string NomEmp { get; set; } = default!;
    public string? NumCgc { get; set; }
}
