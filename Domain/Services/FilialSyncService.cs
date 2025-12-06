using ForcaVendas.Api.Domain.Entities;
using ForcaVendas.Api.Infra.Data;
using ForcaVendas.Api.Infra.Integration.Erp.Filiais;
using Microsoft.EntityFrameworkCore;

namespace ForcaVendas.Api.Domain.Services;

public class FilialSyncService
{
    private readonly ForcaVendasContext _db;
    private readonly IFiliaisErpService _erp;

    public FilialSyncService(ForcaVendasContext db, IFiliaisErpService erp)
    {
        _db = db;
        _erp = erp;
    }

    public async Task SincronizarAsync()
    {
        var filiaisErp = await _erp.BuscarFiliaisAsync();

        var existentes = await _db.Filiais.ToListAsync();
        var dict = existentes.ToDictionary(x => (x.CodEmp, x.CodFil));

        foreach (var dto in filiaisErp)
        {
            var chave = (dto.CodEmp, dto.CodFil);

            if (!dict.TryGetValue(chave, out var entidade))
            {
                entidade = new Filial
                {
                    Id = Guid.NewGuid(),
                    CodEmp = dto.CodEmp,
                    CodFil = dto.CodFil,
                    Nome = dto.Nome,
                    Cnpj = dto.Cnpj,
                    Endereco = dto.Endereco,
                    Numero = dto.Numero,
                    Bairro = dto.Bairro,
                    Cidade = dto.Cidade,
                    Uf = dto.Uf,
                    Cep = dto.Cep,
                    Ativo = true,
                    DatCri = DateTime.UtcNow
                };

                _db.Filiais.Add(entidade);
            }
            else
            {
                entidade.Nome = dto.Nome;
                entidade.Cnpj = dto.Cnpj;
                entidade.Endereco = dto.Endereco;
                entidade.Numero = dto.Numero;
                entidade.Bairro = dto.Bairro;
                entidade.Cidade = dto.Cidade;
                entidade.Uf = dto.Uf;
                entidade.Cep = dto.Cep;

                entidade.Ativo = true;
                entidade.DatAtu = DateTime.UtcNow;
            }
        }

        await _db.SaveChangesAsync();
    }
}
