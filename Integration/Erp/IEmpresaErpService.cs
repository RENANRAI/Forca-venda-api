namespace ForcaVendas.Api.Integration.Erp;

public interface IEmpresaErpService
{
    Task<IReadOnlyList<EmpresaErpDto>> BuscarEmpresasAsync();
}
