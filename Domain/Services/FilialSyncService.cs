
using ForcaVendas.Api.Domain.Entities;
using ForcaVendas.Api.Infra.Data;
using ForcaVendas.Api.Infra.Integration.Erp.Filiais;
using Microsoft.EntityFrameworkCore;


namespace ForcaVendas.Api.Domain.Services;

public class FiliaisSyncService
{
    private readonly ForcaVendasContext _db;
    private readonly IFiliaisErpService _erp;

    public FiliaisSyncService(
        ForcaVendasContext db,
        IFiliaisErpService erp)
    {
        _db = db;
        _erp = erp;
    }

    public async Task SincronizarFiliais(CancellationToken cancellationToken = default)
    {
        // 1) Busca todas empresas/filiais integradas ativas
        var pares = await _db.EmpresasFiliaisIntegradas
            .AsNoTracking()
            .Where(x => x.SitReg)
            .Select(x => new { x.CodEmp, x.CodFil })
            .ToListAsync(cancellationToken);

        foreach (var par in pares)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            var filiaisErp = await _erp.BuscarFiliaisAsync(par.CodEmp, par.CodFil);

            foreach (var fErp in filiaisErp)
            {
                var existente = await _db.Filiais
                    .FirstOrDefaultAsync(f =>
                        f.CodEmp == fErp.CodEmp &&
                        f.CodFil == fErp.CodFil,
                        cancellationToken);

                if (existente is null)
                {
                    var nova = new Filial
                    {
                        Id = Guid.NewGuid(),
                        CodEmp = fErp.CodEmp,
                        CodFil = fErp.CodFil,
                        NomFil = fErp.NomFil,
                        NumCgc = fErp.CgcCpf ?? string.Empty,
                        EndFil = fErp.EndFil,
                        NenFil = fErp.NenFil,
                        BaiFil = fErp.BaiFil,
                        CidFil = fErp.CidFil,
                        SigUfs = fErp.SigUfs,
                        CepFil = fErp.CepFil,
                        SitFil = true,
                        DatCri = DateTime.UtcNow
                    };

                    _db.Filiais.Add(nova);
                }
                else
                {
                    existente.NomFil = fErp.NomFil;
                    existente.NumCgc = fErp.CgcCpf ?? string.Empty;
                    existente.EndFil = fErp.EndFil;
                    existente.NenFil = fErp.NenFil;
                    existente.BaiFil = fErp.BaiFil;
                    existente.CidFil = fErp.CidFil;
                    existente.SigUfs = fErp.SigUfs;
                    existente.CepFil = fErp.CepFil;
                    existente.SitFil = true;
                    existente.DatAtu = DateTime.UtcNow;
                }
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}
