using BitcoinPriceAggregator.Data.Exceptions;
using BitcoinPriceAggregator.Data.Interfaces;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BitcoinPriceAggregator.Data.Scrapers
{
    /// <summary>
    /// Scrapes closing prices from the Bitstamp external data provider
    /// </summary>
    public class BitstampPriceScraper : IPriceScraper
    {
        private readonly string _parsingExceptionMessage = "Could not parse JSON result from Bitstamp";
        private readonly string _retrievalExceptionMessage = "Could not retrieve data point from Bitfinex for date {0}";
        private readonly string _retrievalErrorLogMessage = "Could not retrieve JSON result from Bitstamp url: {0}";
        private readonly string _endpointUrlTemplate = "https://www.bitstamp.net/api/v2/ohlc/btcusd/?step=3600&limit=1&start={startTimestamp}";
        private HttpClient _client;
        private ILogger _logger;

        public BitstampPriceScraper(ILogger<BitstampPriceScraper> logger, HttpClient client)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Scrapes the price for the given hour (on the given date) from the Bitstamp data store
        /// </summary>
        /// <param name="startHourUtc">The date and time (in hours, UTC) for the price point</param>
        /// <returns>A floating point number representing the price, or null, if the price was not available</returns>
        /// <exception cref="ScrapingFailedException">Thrown when retrieval of data point failed</exception>
        public async Task<float?> GetPrice(DateTime startHourUtc)
        {
            long timestamp = ((DateTimeOffset)startHourUtc).ToUnixTimeSeconds();
            string url = _endpointUrlTemplate.Replace("{startTimestamp}", timestamp.ToString());
            try
            {
                string serializedResult = await _client.GetStringAsync(url);
                dynamic result = JsonConvert.DeserializeObject(serializedResult);
                if (result != null) { return result.data.ohlc[0].close; }

                return null;
            }
            catch (RuntimeBinderException ex)
            {
                _logger.LogError(_parsingExceptionMessage, ex);
                throw GetScrapingException(startHourUtc);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(string.Format(_retrievalErrorLogMessage, url), ex);
                throw GetScrapingException(startHourUtc);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(string.Format(_retrievalErrorLogMessage, url), ex);
                throw GetScrapingException(startHourUtc);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(string.Format(_retrievalErrorLogMessage, url), ex);
                throw GetScrapingException(startHourUtc);
            }
        }

        /// <summary>
        /// Throws exception when data retrieval failed
        /// </summary>
        /// <param name="startHourUtc">The date and time (in hours, UTC) for the requested price point</param>
        /// <returns>The configured scraping exception</returns>
        private ScrapingFailedException GetScrapingException(DateTime startHourUtc)
        {
            string errorMessage = string.Format(_retrievalExceptionMessage, startHourUtc);
            // Raise exception without exposing implementation details to caller
            return new ScrapingFailedException(errorMessage);
        }
    }
}
