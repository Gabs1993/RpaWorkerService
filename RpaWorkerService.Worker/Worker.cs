using Application.UseCases;

namespace RpaWorkerService.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public Worker(
            ILogger<Worker> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var intervalMinutes = _configuration.GetValue<int>("WorkerSettings:IntervalInMinutes");
            if (intervalMinutes <= 0)
                intervalMinutes = 5;

            _logger.LogInformation("Worker iniciado. Intervalo configurado: {Interval} minutos.", intervalMinutes);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var useCase = scope.ServiceProvider.GetRequiredService<CollectDataUseCase>();

                    await useCase.ExecuteAsync(stoppingToken);

                    _logger.LogInformation("Execução da coleta finalizada em {DateTime}.", DateTime.UtcNow);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro durante a execução do worker.");
                }

                await Task.Delay(TimeSpan.FromMinutes(intervalMinutes), stoppingToken);
            }
        }
    }
}
