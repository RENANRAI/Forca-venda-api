using ForcaVendas.Api.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace ForcaVendas.Api.Controllers;

[ApiController]
[Route("api/integracao")]
public class IntegracaoController : ControllerBase
{
    private readonly ClienteSyncService _clienteSync;
    private readonly EmpresasFiliaisIntegradasSyncService _empFilSync;
    private readonly FilialSyncService _filsync;

    public IntegracaoController(ClienteSyncService clienteSync, EmpresasFiliaisIntegradasSyncService empFilSync, FilialSyncService filsync) 
    {
        _clienteSync = clienteSync;
        _empFilSync = empFilSync;
        _filsync = filsync ;


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

    [HttpPost("clientes/sincronizar")]
    public async Task<IActionResult> SincronizarClientes()
    {
        await _clienteSync.SincronizarClientesAsync();
        return Ok("Sincronização de clientes disparada.");
    }
   

}
