using Domain.Entities;
using Domain.Interfaces;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Infra.Data.Services
{
    public class QuoteCollectorService : IDataCollectorService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<QuoteCollectorService> _logger;
        private readonly string _sourceUrl;

        public QuoteCollectorService(HttpClient httpClient,IConfiguration configuration,ILogger<QuoteCollectorService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _sourceUrl = configuration["ScrapingSettings:SourceUrl"]
                         ?? "http://quotes.toscrape.com/";
        }

        public async Task<List<CollectedData>> CollectAsync(CancellationToken cancellationToken = default)
        {
            var html = await GetPageContentWithRetryAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(html))
            {
                _logger.LogWarning("Não foi possível obter conteúdo HTML da página.");
                return new List<CollectedData>();
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var quoteNodes = doc.DocumentNode.SelectNodes("//div[contains(@class,'quote')]");

            if (quoteNodes is null || quoteNodes.Count == 0)
            {
                _logger.LogWarning("Nenhum elemento esperado foi encontrado. O layout da página pode ter mudado.");
                return new List<CollectedData>();
            }

            var items = new List<CollectedData>();

            foreach (var quoteNode in quoteNodes)
            {
                var textNode = quoteNode.SelectSingleNode(".//span[contains(@class,'text')]");
                var authorNode = quoteNode.SelectSingleNode(".//small[contains(@class,'author')]");

                if (textNode is null || authorNode is null)
                    continue;

                var quoteText = HtmlEntity.DeEntitize(textNode.InnerText).Trim();
                var author = HtmlEntity.DeEntitize(authorNode.InnerText).Trim();

                if (string.IsNullOrWhiteSpace(quoteText))
                    continue;

                items.Add(new CollectedData(
                    source: "Quotes To Scrape",
                    title: $"Frase de {author}",
                    description: quoteText,
                    url: _sourceUrl
                ));
            }

            return items;
        }

        private async Task<string?> GetPageContentWithRetryAsync(CancellationToken cancellationToken)
        {
            const int maxAttempts = 3;

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    _logger.LogInformation("Tentativa {Attempt} de coleta na URL {Url}", attempt, _sourceUrl);

                    var response = await _httpClient.GetAsync(_sourceUrl, cancellationToken);
                    response.EnsureSuccessStatusCode();

                    return await response.Content.ReadAsStringAsync(cancellationToken);
                }
                catch (Exception ex) when (attempt < maxAttempts)
                {
                    _logger.LogWarning(ex, "Falha na tentativa {Attempt}. Nova tentativa em alguns segundos.", attempt);
                    await Task.Delay(TimeSpan.FromSeconds(attempt * 2), cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro final ao coletar dados da URL {Url}", _sourceUrl);
                    return null;
                }
            }

            return null;
        }
    }
}
