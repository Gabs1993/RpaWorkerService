using System.Text.Json;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infra.Data.Services
{
    public class CurrencyCollectorService : IDataCollectorService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CurrencyCollectorService> _logger;
        private readonly string _sourceUrl;

        public CurrencyCollectorService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<CurrencyCollectorService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _sourceUrl = configuration["CurrencySettings:SourceUrl"]
                         ?? "https://api.frankfurter.dev/v2/rates?base=BRL&quotes=USD,EUR,GBP";
        }

        public async Task<List<CollectedData>> CollectAsync(CancellationToken cancellationToken = default)
        {
            var json = await GetContentWithRetryAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(json))
            {
                _logger.LogWarning("Nenhum conteúdo foi retornado pela API de moedas.");
                return new List<CollectedData>();
            }

            _logger.LogInformation("Resposta bruta da API: {Json}", json);

            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            if (root.ValueKind != JsonValueKind.Array)
            {
                _logger.LogWarning("A resposta da API não veio em formato de array.");
                return new List<CollectedData>();
            }

            var items = new List<CollectedData>();

            foreach (var element in root.EnumerateArray())
            {
                var baseCurrency = element.TryGetProperty("base", out var baseElement)
                    ? baseElement.GetString() ?? "BRL"
                    : "BRL";

                var quoteCurrency = element.TryGetProperty("quote", out var quoteElement)
                    ? quoteElement.GetString() ?? "UNKNOWN"
                    : "UNKNOWN";

                string rateValue = "0";

                if (element.TryGetProperty("rate", out var rateElement))
                {
                    if (rateElement.ValueKind == JsonValueKind.Number)
                    {
                        rateValue = rateElement.GetDecimal().ToString("F6");
                    }
                    else
                    {
                        rateValue = rateElement.ToString();
                    }
                }

                items.Add(new CollectedData(
                    source: "Frankfurter API",
                    title: $"{baseCurrency}/{quoteCurrency}",
                    description: rateValue,
                    url: _sourceUrl
                ));
            }

            return items;
        }

        private async Task<string?> GetContentWithRetryAsync(CancellationToken cancellationToken)
        {
            const int maxAttempts = 3;

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    _logger.LogInformation("Tentativa {Attempt} de coleta de moedas.", attempt);

                    var response = await _httpClient.GetAsync(_sourceUrl, cancellationToken);
                    response.EnsureSuccessStatusCode();

                    return await response.Content.ReadAsStringAsync(cancellationToken);
                }
                catch (Exception ex) when (attempt < maxAttempts)
                {
                    _logger.LogWarning(ex, "Falha na tentativa {Attempt}. Tentando novamente...", attempt);
                    await Task.Delay(TimeSpan.FromSeconds(attempt * 2), cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro final ao consultar a API de moedas.");
                    return null;
                }
            }

            return null;
        }
    }
}