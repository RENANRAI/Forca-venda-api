using Forca_venda_api.Infra.Data;
using ForcaVendas.Api.Domain.Entities;
using ForcaVendas.Api.Infra.Integration.Erp.EmpresasFiliais;
using Microsoft.EntityFrameworkCore;

namespace ForcaVendas.Api.Domain.Services;

public class EmpresasFiliaisIntegradasSyncService
{
    private readonly ForcaVendasContext _db;
    private readonly IEmpresasFiliaisErpService _erp;

    public EmpresasFiliaisIntegradasSyncService(
        ForcaVendasContext db,
        IEmpresasFiliaisErpService erp)
    {
        _db = db;
        _erp = erp;
    }

    public async Task SincronizarAsync(CancellationToken cancellationToken = default)
    {
        var listaErp = await _erp.BuscarEmpresasFiliaisIntegradasAsync();

        var existentes = await _db.EmpresasFiliaisIntegradas
            .ToListAsync(cancellationToken);

        var dictExistentes = existentes
            .ToDictionary(x => (x.CodEmp, x.CodFil), x => x);

        var vistos = new HashSet<(string CodEmp, string CodFil)>();

        foreach (var dto in listaErp)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            var chave = (dto.CodEmp, dto.CodFil);
            vistos.Add(chave);

            if (!dictExistentes.TryGetValue(chave, out var entidade))
            {
                entidade = new EmpresaFilialIntegrada
                {
                    Id = Guid.NewGuid(),
                    CodEmp = dto.CodEmp,
                    CodFil = dto.CodFil,
                   /* NomeEmpresa = dto.NomeEmpresa,
                    NomeFilial = dto.NomeFilial,*/
                    SitReg = true,
                    DatCri = DateTime.UtcNow
                };

                _db.EmpresasFiliaisIntegradas.Add(entidade);
            }
            else
            {
               /* entidade.NomeEmpresa = dto.NomeEmpresa;
                entidade.NomeFilial = dto.NomeFilial;*/
                entidade.SitReg = true;
                entidade.DatAtu = DateTime.UtcNow;
            }
        }

        // Opcional: desativar o que não veio mais
        foreach (var existente in existentes)
        {
            if (!vistos.Contains((existente.CodEmp, existente.CodFil)) && existente.SitReg)
            {
                existente.SitReg = false;
                existente.DatAtu = DateTime.UtcNow;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}
