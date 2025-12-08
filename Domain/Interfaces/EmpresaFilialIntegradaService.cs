using ForcaVendas.Api.Domain.Entities;

namespace ForcaVendas.Api.Domain.Interfaces;

public interface IEmpresasFiliaisIntegradasReader
{

     //tetse
    Task<List<EmpresaFilialIntegrada>> ListarAtivasAsync();
}
