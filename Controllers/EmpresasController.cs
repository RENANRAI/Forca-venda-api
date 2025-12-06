using Forca_venda_api.Domain.Dtos;
using ForcaVendas.Api.Infra.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ForcaVendas.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmpresasController : ControllerBase
{
    private readonly ForcaVendasContext _db;

    public EmpresasController(ForcaVendasContext db)
    {
        _db = db;
    }

    // GET /api/empresas
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmpresaDto>>> GetEmpresas()
    {
        var empresas = await _db.Empresas
            .AsNoTracking()
            .Where(e => e.Ativo)
            .OrderBy(e => e.Nome)
            .ToListAsync();

        var result = empresas.Select(e => new EmpresaDto
        {
            Id = e.Id.ToString(),
            CodEmp = e.CodEmp,
            Nome = e.Nome,
            Cnpj = e.Cnpj
        });

        return Ok(result);
    }
}
