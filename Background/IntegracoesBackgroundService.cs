using ForcaVendas.Api.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ForcaVendas.Api.Background
{
    public class IntegracoesBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<IntegracoesBackgroundService> _logger;

        public IntegracoesBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<IntegracoesBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Background de integrações iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();

                    /* 1️⃣ Sincroniza empresas/filiais integradas
                     Só sincriza os codigos de empresa e filial que estão parametrizas do ERP
                    apartir dessa integração que a rotina integra os cadastrs de empresa e filials 
                    que serão integradas, seria apenas uma tabela de parametrização recebida do erp
                    com as empresas e filiais que estão parametrizadas para usar o força de vendas*/


                    _logger.LogInformation("Verificando Empresas/Filiais que serão Integradas.");
                    var empFilSync = scope.ServiceProvider
                        .GetRequiredService<EmpresasFiliaisIntegradasSyncService>();


                    await empFilSync.SincronizarAsync(stoppingToken);
                    _logger.LogInformation("Sincronização de Empresas/Filiais Integradas concluída.");


                    // SINCRONIZA CADASTO DE FILIAL
                    _logger.LogInformation("Verificando Sincronia de Filial.");
                    var filialSync = scope.ServiceProvider.GetRequiredService<FiliaisSyncService>();

                    await filialSync.SincronizarFiliais(stoppingToken);
                    _logger.LogInformation("Verificando Sincronia de Filial Concluida");


                    _logger.LogInformation("Inciando Sincronia de Representante");
                    // SINCRONIZA CADASTO DE FILIAL
                    var repreSync = scope.ServiceProvider.GetRequiredService<RepresentanteSyncService>();
                    await repreSync.BuscarRepresentantesAsync(stoppingToken);

                    _logger.LogInformation("Inciando Sincronia de Representante Conccluida");

                    // 2️⃣ Sincroniza clientes
                    _logger.LogInformation("Inciando Sincronia de Representante Clientes");
                    var cliSync = scope.ServiceProvider
                        .GetRequiredService<ClienteSyncService>();

                    await cliSync.SincronizarClientesAsync(stoppingToken);
                    _logger.LogInformation("Sincronização de Clientes concluída.");


                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao executar ciclo de integrações.");
                }

                // intervalo entre os ciclos
                await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
            }

            _logger.LogInformation("Background de integrações finalizado.");
        }
    }
}
