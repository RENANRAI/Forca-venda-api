using ForcaVendas.Api.Infra.Data;
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
        // 1) Busca do ERP
        var listaErp = await _erp.BuscarEmpresasFiliaisIntegradasAsync();

        // 2) Carrega filiais integradas existentes
        var existentesFiliais = await _db.EmpresasFiliaisIntegradas
            .ToListAsync(cancellationToken);

        var dictFiliais = existentesFiliais
            .ToDictionary(x => (x.CodEmp, x.CodFil), x => x);

        // 3) Carrega empresas existentes
        var existentesEmpresas = await _db.Empresas
            .ToListAsync(cancellationToken);

        var dictEmpresas = existentesEmpresas
            .ToDictionary(e => e.CodEmp, e => e);   // <<-- AGORA É int

        var vistosFiliais = new HashSet<(int CodEmp, int CodFil)>();

        foreach (var dto in listaErp)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            var chaveFilial = (dto.CodEmp, dto.CodFil);
            vistosFiliais.Add(chaveFilial);


            // =========================================
            // 4) Sincroniza EMPRESA-FILIAL INTEGRADA
            // =========================================
            if (!dictFiliais.TryGetValue(chaveFilial, out var entidadeFilial))
            {
                entidadeFilial = new EmpresaFilialIntegrada
                {
                    Id = Guid.NewGuid(),
                    CodEmp = dto.CodEmp,
                    CodFil = dto.CodFil,
                    // NomeEmpresa = dto.NomeEmpresa,
                    // NomeFilial = dto.NomeFilial,
                    SitReg = true,
                    DatCri = DateTime.UtcNow
                };

                _db.EmpresasFiliaisIntegradas.Add(entidadeFilial);
                dictFiliais[chaveFilial] = entidadeFilial;
            }
            else
            {
                // entidadeFilial.NomeEmpresa = dto.NomeEmpresa;
                // entidadeFilial.NomeFilial = dto.NomeFilial;
                entidadeFilial.SitReg = true;
                entidadeFilial.DatAtu = DateTime.UtcNow;
            }

            // ==========================
            // 5) Sincroniza EMPRESA
            // ==========================
            if (!dictEmpresas.TryGetValue(dto.CodEmp, out var empresa))
            {
                empresa = new Empresa
                {
                    Id = Guid.NewGuid(),
                    CodEmp = dto.CodEmp,
                    NomEmp = string.IsNullOrWhiteSpace(dto.NomEmp)
                        ? $"EMP {dto.CodEmp}"
                        : dto.NomEmp,
                    NumCgc = null,
                    SitEmp = true,
                    DatCri = DateTime.UtcNow
                };

                _db.Empresas.Add(empresa);
                dictEmpresas[dto.CodEmp] = empresa;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(dto.NomEmp) &&
                    empresa.NomEmp != dto.NomEmp)
                {
                    empresa.NomEmp = dto.NomEmp;
                    empresa.DatAtu = DateTime.UtcNow;
                }

                if (!empresa.SitEmp)
                {
                    empresa.SitEmp = true;
                    empresa.DatAtu = DateTime.UtcNow;
                }
            }

           
        }

        // 6) Desativar filiais que não vieram mais do ERP
        foreach (var existente in existentesFiliais)
        {
            if (!vistosFiliais.Contains((existente.CodEmp, existente.CodFil)) && existente.SitReg)
            {
                existente.SitReg = false;
                existente.DatAtu = DateTime.UtcNow;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}
