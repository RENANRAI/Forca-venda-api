using ForcaVendas.Api.Domain.Entities;

namespace ForcaVendas.Api.Domain.Interfaces;

public interface IEmpresasFiliaisIntegradasReader
{
    Task<List<EmpresaFilialIntegrada>> ListarAtivasAsync();
}
