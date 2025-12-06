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
                .FirstOrDefaultAsync(c => c.CodigoErp == cliErp.CodigoErp, cancellationToken);

            if (existente is null)
            {
                var novo = new Cliente
                {
                    Id = Guid.NewGuid(),
                    CodigoErp = cliErp.CodigoErp,
                    Nome = cliErp.Nome,
                    Documento = cliErp.Documento,
                    Cidade = cliErp.Cidade,
                    Uf = cliErp.Uf,
                    Ativo = true,
                    DataCriacao = DateTime.UtcNow,
                    DataUltimaSincronizacao = DateTime.UtcNow
                };

                _db.Clientes.Add(novo);
            }
            else
            {
                existente.Nome = cliErp.Nome;
                existente.Documento = cliErp.Documento;
                existente.Cidade = cliErp.Cidade;
                existente.Uf = cliErp.Uf;
                existente.Ativo = true;
                existente.DataAtualizacao = DateTime.UtcNow;
                existente.DataUltimaSincronizacao = DateTime.UtcNow;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}
