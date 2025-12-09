using ForcaVendas.Api.Infra.Data;
using ForcaVendas.Api.Infra.Integration.Erp.Clientes;
using ForcaVendas.Api.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace ForcaVendas.Api.Domain.Services;

public class ClienteSyncService
{
    private readonly ForcaVendasContext _db;
    private readonly IClienteErpService _erp;

    public ClienteSyncService(ForcaVendasContext db, IClienteErpService erp)
    {
        _db = db;
        _erp = erp;
    }

    public async Task SincronizarClientesAsync(CancellationToken cancellationToken = default)
    {
        // Aqui você poderia ler a data da última sync global de clientes
        // Por simplicidade, vamos mandar null (todos)
        DateTime? alteradosDesde = null;

        var clientesErp = await _erp.BuscarClientesAsync(alteradosDesde);

        foreach (var cliErp in clientesErp)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            var existente = await _db.Clientes
                .FirstOrDefaultAsync(c => c.CodCli == cliErp.CodCli, cancellationToken);

            if (existente is null)
            {
                var novo = new Cliente
                {
                    Id = Guid.NewGuid(),
                    CodCli = cliErp.CodCli,
                    NomCli = cliErp.NomCli,
                    NumCgc = cliErp.NumCgc,
                    CidCli = cliErp.CidCli,
                    SigUfs = cliErp.SigUfs,
                    SitCli = true,
                    DatCri = DateTime.UtcNow,
                    DatSyn = DateTime.UtcNow
                };

                _db.Clientes.Add(novo);
            }
            else
            {
                existente.NomCli = cliErp.NomCli;
                existente.NumCgc = cliErp.NumCgc;
                existente.CidCli = cliErp.CidCli;
                existente.SigUfs = cliErp.SigUfs;
                existente.SitCli = true;
                existente.DatAtu = DateTime.UtcNow;
                existente.DatSyn = DateTime.UtcNow;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}
