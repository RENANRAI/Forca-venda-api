using Forca_venda_api.Domain.Dtos;
using ForcaVendas.Api.Infra.Data;
using ForcaVendas.Api.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ForcaVendas.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly ForcaVendasContext _context;

    public ClientesController(ForcaVendasContext context)
    {
        _context = context;
    }

    // GET api/clientes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClienteDto>>> GetClientes()
    {
        var clientes = await _context.Clientes
            .AsNoTracking()
            .OrderBy(c => c.NomCli)
            .ToListAsync();

        var result = clientes.Select(c => new ClienteDto
        {
            Id = c.Id.ToString(),
            NomCli = c.NomCli,
            NumCgc = c.NumCgc,
            CidCli = c.CidCli,
            SigUfs = c.SigUfs
        });

        return Ok(result);
    }

    // GET api/clientes/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ClienteDto>> GetClientePorId(Guid id)
    {
        var cliente = await _context.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cliente is null)
            return NotFound();

        var dto = new ClienteDto
        {
            Id = cliente.Id.ToString(),
            NomCli = cliente.NomCli,
            NumCgc = cliente.NumCgc,
            CidCli = cliente.CidCli,
            SigUfs = cliente.SigUfs
        };

        return Ok(dto);
    }

    // POST api/clientes
    [HttpPost]
    public async Task<ActionResult<ClienteDto>> CriarCliente([FromBody] ClienteDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.NomCli) || string.IsNullOrWhiteSpace(dto.NumCgc))
            return BadRequest("Nome e CPF/CNPJ são obrigatórios.");

        var entidade = new Cliente
        {
            Id = Guid.NewGuid(),
            NomCli = dto.NomCli,
            NumCgc = dto.NumCgc,
            CidCli = dto.CidCli,
            SigUfs = dto.SigUfs,
            SitCli= true,
            DatCri = DateTime.UtcNow
        };

        _context.Clientes.Add(entidade);
        await _context.SaveChangesAsync();

        dto.Id = entidade.Id.ToString();

        return CreatedAtAction(nameof(GetClientePorId), new { id = entidade.Id }, dto);
    }

    // PUT api/clientes/{id}
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ClienteDto>> AtualizarCliente(Guid id, [FromBody] ClienteDto dto)
    {
        var entidade = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
        if (entidade is null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(dto.NomCli) || string.IsNullOrWhiteSpace(dto.NumCgc))
            return BadRequest("Nome e Documento são obrigatórios.");

        entidade.NomCli = dto.NomCli;
        entidade.NumCgc = dto.NumCgc;
        entidade.CidCli = dto.CidCli;
        entidade.SigUfs = dto.SigUfs;
        entidade.DatAtu = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        dto.Id = entidade.Id.ToString();

        return Ok(dto);
    }
}
