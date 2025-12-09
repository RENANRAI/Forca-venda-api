namespace ForcaVendas.Api.Infra.Integration.Erp.Filiais;

public interface IFiliaisErpService
{
    Task<IReadOnlyList<FilialErpDto>> BuscarFiliaisAsync(int codEmp, int codFil);
}
