using ForcaVendas.Api.Data;
using ForcaVendas.Api.Entities;
using ForcaVendas.Api.Models;
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
            .OrderBy(c => c.Nome)
            .ToListAsync();

        var result = clientes.Select(c => new ClienteDto
        {
            Id = c.Id.ToString(),
            Nome = c.Nome,
            Documento = c.Documento,
            Cidade = c.Cidade,
            Uf = c.Uf
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
            Nome = cliente.Nome,
            Documento = cliente.Documento,
            Cidade = cliente.Cidade,
            Uf = cliente.Uf
        };

        return Ok(dto);
    }

    // POST api/clientes
    [HttpPost]
    public async Task<ActionResult<ClienteDto>> CriarCliente([FromBody] ClienteDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome) || string.IsNullOrWhiteSpace(dto.Documento))
            return BadRequest("Nome e Documento são obrigatórios.");

        var entidade = new Cliente
        {
            Id = Guid.NewGuid(),
            Nome = dto.Nome,
            Documento = dto.Documento,
            Cidade = dto.Cidade,
            Uf = dto.Uf,
            Ativo = true,
            DataCriacao = DateTime.UtcNow
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

        if (string.IsNullOrWhiteSpace(dto.Nome) || string.IsNullOrWhiteSpace(dto.Documento))
            return BadRequest("Nome e Documento são obrigatórios.");

        entidade.Nome = dto.Nome;
        entidade.Documento = dto.Documento;
        entidade.Cidade = dto.Cidade;
        entidade.Uf = dto.Uf;
        entidade.DataAtualizacao = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        dto.Id = entidade.Id.ToString();

        return Ok(dto);
    }
}
