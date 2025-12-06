using Forca_venda_api.Domain.Services;
using ForcaVendas.Api.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace ForcaVendas.Api.Controllers;

[ApiController]
[Route("api/integracao")]
public class IntegracaoController : ControllerBase
{
    private readonly ClienteSyncService _clienteSync;
    private readonly EmpresasFiliaisIntegradasSyncService _empFilSync;

    public IntegracaoController(ClienteSyncService clienteSync, EmpresasFiliaisIntegradasSyncService empFilSync)
    {
        _clienteSync = clienteSync;
        _empFilSync = empFilSync;
    }

    [HttpPost("clientes/sincronizar")]
    public async Task<IActionResult> SincronizarClientes()
    {
        await _clienteSync.SincronizarClientesAsync();
        return Ok("Sincronização de clientes disparada.");
    }
    [HttpPost("empresas-filiais/sincronizar")]
    public async Task<IActionResult> SincronizarEmpresasFiliais(CancellationToken ct)
    {
        await _empFilSync.SincronizarAsync(ct);
        return Ok("Sincronização de empresas/filiais integradas disparada.");
    }
}
