using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;


namespace Application.UseCases
{
    public class CollectDataUseCase
    {
        private readonly IDataCollectorService _collectorService;
        private readonly ICollectedDataRepository _repository;
        private readonly ILogger<CollectDataUseCase> _logger;

        public CollectDataUseCase(
            IDataCollectorService collectorService,
            ICollectedDataRepository repository,
            ILogger<CollectDataUseCase> logger)
        {
            _collectorService = collectorService;
            _repository = repository;
            _logger = logger;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var collectedItems = await _collectorService.CollectAsync(cancellationToken);

            if (!collectedItems.Any())
            {
                _logger.LogInformation("Nenhum dado foi coletado nesta execução.");
                return;
            }

            var newItems = new List<CollectedData>();

            foreach (var item in collectedItems)
            {
                var alreadyExists = await _repository.ExistsAsync(
                    item.Source,
                    item.Title,
                    item.Url,
                    cancellationToken);

                if (!alreadyExists)
                {
                    newItems.Add(item);
                }
            }

            if (!newItems.Any())
            {
                _logger.LogInformation("Todos os dados coletados já existiam no banco.");
                return;
            }

            await _repository.AddRangeAsync(newItems, cancellationToken);

            _logger.LogInformation("{Count} novos registros foram salvos.", newItems.Count);
        }
    }
}
