using ForcaVendas.Api.Infra.Data;
using ForcaVendas.Api.Infra.Integration.Erp.Clientes;
using ForcaVendas.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ForcaVendas.Api.Domain.Services;

public class ClienteSyncService
{
    private readonly ForcaVendasContext _db;
    private readonly IClienteErpService _erp;

    public ClienteSyncService(
        ForcaVendasContext db,
        IClienteErpService erp)
    {
        _db = db;
        _erp = erp;
    }

    public async Task SincronizarClientesAsync(CancellationToken cancellationToken = default)
    {
        // Pega todas empresas/filiais integradas
        var pares = await _db.EmpresasFiliaisIntegradas
            .AsNoTracking()
            .Where(x => x.SitReg)
            .Select(x => new { x.CodEmp, x.CodFil })
            .ToListAsync(cancellationToken);

        foreach (var par in pares)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            // CHAMA O WS SÓ UMA VEZ POR (EMPRESA,FILIAL)
            var clientesErp = await _erp.BuscarClientesAsync(par.CodEmp, par.CodFil);

            foreach (var cliErp in clientesErp)
            {
                // 1) SINCRONIZA O CADASTRO DO CLIENTE
                var cliente = await _db.Clientes
                    .FirstOrDefaultAsync(c => c.CodCli == cliErp.CodCli, cancellationToken);

                if (cliente is null)
                {
                    cliente = new Cliente
                    {
                        Id = Guid.NewGuid(),
                        CodCli = cliErp.CodCli,
                        NomCli = cliErp.NomCli,
                        CgcCpf = cliErp.CgcCpf,
                        CidCli = cliErp.CidCli,
                        SigUfs = cliErp.SigUfs,
                        SitCli = true,
                        DatCri = DateTime.UtcNow,
                        DatSyn = DateTime.UtcNow
                    };

                    _db.Clientes.Add(cliente);
                }
                else
                {
                    cliente.NomCli = cliErp.NomCli;
                    cliente.CgcCpf = cliErp.CgcCpf;
                    cliente.CidCli = cliErp.CidCli;
                    cliente.SigUfs = cliErp.SigUfs;
                    cliente.SitCli = true;
                    cliente.DatAtu = DateTime.UtcNow;
                    cliente.DatSyn = DateTime.UtcNow;
                }

                // 2) SINCRONIZA OS PARÂMETROS POR FILIAL (historico)
                if (cliErp.ParametrosPorFilial is null || cliErp.ParametrosPorFilial.Count == 0)
                    continue;

                // filtra só os históricos daquela empresa/filial do loop atual
                /* var historicosDaFilial = cliErp.ParametrosPorFilial
                     .Where(h => h.CodEmp == par.CodEmp && h.CodFil == par.CodFil);*/

                var historicosDaFilial = cliErp.ParametrosPorFilial;

                foreach (var paramErp in historicosDaFilial)
                {
                    var chave = new { paramErp.CodCli, paramErp.CodEmp, paramErp.CodFil };

                    var param = await _db.ClienteParametrosFiliais
                        .FirstOrDefaultAsync(x =>
                            x.CodCli == chave.CodCli &&
                            x.CodEmp == chave.CodEmp &&
                            x.CodFil == chave.CodFil,
                            cancellationToken);

                    if (param is null)
                    {
                        param = new ClienteParametrosFilial
                        {
                            Id = Guid.NewGuid(),
                            CodCli = paramErp.CodCli,
                            CodEmp = paramErp.CodEmp,
                            CodFil = paramErp.CodFil,
                            SitReg = true,
                            DatCri = DateTime.UtcNow
                        };

                        _db.ClienteParametrosFiliais.Add(param);
                    }

                    // Atualiza campos principais
                    param.SalDup = paramErp.SalDup;
                    param.SalOut = paramErp.SalOut;
                    param.SalCre = paramErp.SalCre;
                    param.DatLim = paramErp.DatLim;
                    param.VlrLim = paramErp.VlrLim;
                    param.LimApr = paramErp.LimApr;
                    param.VlrPfa = paramErp.VlrPfa;
                    param.DatMac = paramErp.DatMac;
                    param.VlrMac = paramErp.VlrMac;

                    param.CatCli = paramErp.CatCli;
                    param.CodCpg = paramErp.CodCpg;
                    param.CodFpg = paramErp.CodFpg;
                    param.CodTpr = paramErp.CodTpr;
                    param.PerDsc = paramErp.PerDsc;
                    param.PerFre = paramErp.PerFre;
                    param.PerIss = paramErp.PerIss;
                    param.CifFob = paramErp.CifFob;
                    param.CodTab = paramErp.CodTab;

                    param.SitReg = true;
                    param.DatAtu = DateTime.UtcNow;
                }
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}
