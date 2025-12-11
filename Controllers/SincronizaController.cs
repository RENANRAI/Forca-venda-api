using ForcaVendas.Api.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace ForcaVendas.Api.Controllers;

[ApiController]
[Route("api/integracao")]
public class IntegracaoController : ControllerBase
{
    private readonly ClienteSyncService _clienteSync;
    private readonly EmpresasFiliaisIntegradasSyncService _empFilSync;
    private readonly FiliaisSyncService _filsync;
    private readonly RepresentanteSyncService _representanteSync;



    public IntegracaoController(ClienteSyncService clienteSync,
              EmpresasFiliaisIntegradasSyncService empFilSync,
              FiliaisSyncService filsync, RepresentanteSyncService representanteSync)
    {
        _clienteSync = clienteSync;
        _empFilSync = empFilSync;
        _filsync = filsync;
        _representanteSync = representanteSync;

    }


    [HttpPost("empresas-filiais/sincronizar")]
    public async Task<IActionResult> SincronizarEmpresasFiliais(CancellationToken ct)
    {
        await _empFilSync.SincronizarAsync(ct);
        return Ok("Sincronização de empresas/filiais integradas disparada.");
    }

    [HttpPost("filiais/sincronizar")]
    public async Task<IActionResult> Sincronizar()
    {
        await _filsync.SincronizarFiliais();
        return Ok("Sincronização de filiais concluída.");
    }

    [HttpPost("Representantes/sincronizar")]
    public async Task<IActionResult> SincroniarRepresentantes(CancellationToken ct)
    {
        //await _clienteSync.SincronizarClientesAsync();
        await _representanteSync.BuscarRepresentantesAsync(ct);
        return Ok("Sincronização de Representantes disparada.");
    }


    [HttpPost("clientes/sincronizar")]
    public async Task<IActionResult> SincronizarClientes()
    {
        await _clienteSync.SincronizarClientesAsync();
        return Ok("Sincronização de clientes disparada.");
    }


}
