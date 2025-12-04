using ForcaVendas.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ForcaVendas.Api.Controllers;

[ApiController]
[Route("api/integracao")]
public class IntegracaoController : ControllerBase
{
    private readonly ClienteSyncService _clienteSync;

    public IntegracaoController(ClienteSyncService clienteSync)
    {
        _clienteSync = clienteSync;
    }

    [HttpPost("clientes/sincronizar")]
    public async Task<IActionResult> SincronizarClientes()
    {
        await _clienteSync.SincronizarClientesAsync();
        return Ok("Sincronização de clientes disparada.");
    }
}
