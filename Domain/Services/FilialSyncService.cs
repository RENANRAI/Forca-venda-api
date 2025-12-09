using ForcaVendas.Api.Domain.Entities;
using ForcaVendas.Api.Infra.Data;
using ForcaVendas.Api.Infra.Integration.Erp.Filiais;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ForcaVendas.Api.Domain.Services;

public class FilialSyncService
{
    private readonly ForcaVendasContext _db;
    private readonly IFiliaisErpService _erp;
    private readonly ILogger<FilialSyncService> _logger;

    public FilialSyncService(ForcaVendasContext db, IFiliaisErpService erp, ILogger<FilialSyncService> logger)
    {
        _db = db;
        _erp = erp;
        _logger = logger;
    }

    public async Task SincronizarFiliais(CancellationToken cancellationToken = default)
    {
        var filiaisErp = await _erp.BuscarFiliaisAsync();

        var existentes = await _db.Filiais.ToListAsync();
        var dict = existentes.ToDictionary(x => (x.CodEmp, x.CodFil));

        var vistos = new HashSet<(int CodEmp, int CodFil)>();

        foreach (var dto in filiaisErp)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            var chave = (dto.CodEmp, dto.CodFil);
            vistos.Add(chave);

            if (!dict.TryGetValue(chave, out var entidade))
            {
                entidade = new Filial
                {
                    Id = Guid.NewGuid(),
                    CodEmp = dto.CodEmp,
                    CodFil = dto.CodFil,
                    NomFil = dto.NomFil,
                    NumCgc = dto.NumCgc,
                    EndFil = dto.EndFil,
                    NenFil = dto.NenFil,
                    BaiFil = dto.BaiFil,
                    CidFil = dto.CidFil,
                    SigUfs = dto.SigUfs,
                    CepFil = dto.CepFil,
                    SitFil = true,
                    DatCri = DateTime.UtcNow
                };

                _db.Filiais.Add(entidade);
            }
            else
            {
                entidade.NomFil = dto.NomFil;
                entidade.NumCgc = dto.NumCgc;
                entidade.EndFil = dto.EndFil;
                entidade.NenFil = dto.NenFil;
                entidade.BaiFil = dto.BaiFil;
                entidade.CidFil = dto.CidFil;
                entidade.SigUfs = dto.SigUfs;
                entidade.SigUfs = dto.SigUfs;

                entidade.SitFil = true;
                entidade.DatAtu = DateTime.UtcNow;
            }
        }


        // opcional: desativar filiais que não vieram mais do ERP
        foreach (var existente in existentes)
        {
            if (!vistos.Contains((existente.CodEmp, existente.CodFil)) && existente.SitFil)
            {
                existente.SitFil = false;
                existente.DatAtu = DateTime.UtcNow;
            }
        }

        await _db.SaveChangesAsync();
        _logger.LogInformation("Sincronização de filiais concluída.");
    }
}
