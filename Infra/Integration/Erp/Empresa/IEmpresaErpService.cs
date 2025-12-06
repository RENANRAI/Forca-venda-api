namespace ForcaVendas.Api.Infra.Integration.Erp.Empresa;

public interface IEmpresaErpService
{
    Task<IReadOnlyList<EmpresaErpDto>> BuscarEmpresasAsync();
}
