using BitcoinPriceAggregator.Web.Exceptions;
using BitcoinPriceAggregator.Web.Interfaces;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BitcoinPriceAggregator.Web.Scrapers
{
    /// <summary>
    /// Scrapes closing prices from the Bitfinex external data provider
    /// </summary>
    public class BitfinexPriceScraper : IPriceScraper
    {
        private readonly string _parsingExceptionMessage = "Could not parse JSON result from Bitfinex";
        private readonly string _retrievalExceptionMessage = "Could not retrieve data point from Bitfinex for date {0}";
        private readonly string _retrievalErrorLogMessage = "Could not retrieve JSON result from Bitfinex url: {0}";
        private readonly string _endpointUrlTemplate = "https://api-pub.bitfinex.com/v2/candles/trade:1h:tBTCUSD/hist?start={startTimestamp}&end={endTimestamp}&limit=1";
        private HttpClient _client;
        private ILogger _logger;

        public BitfinexPriceScraper(ILogger<BitfinexPriceScraper> logger, HttpClient client)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Scrapes the price for the given hour (on the given date) from the Bitfinex data store
        /// </summary>
        /// <param name="date">The date (UTC) for the price point</param>
        /// <param name="hour">The hour for the price point</param>
        /// <returns>A floating point number representing the price, or null, if the price was not available</returns>
        /// <exception cref="ScrapingFailedException">Thrown when retrieval of data point failed</exception>
        public async Task<float?> GetPrice(DateTime date, int hour)
        {
            DateTime startDate = date.AddHours(hour);
            long startTimestamp = ((DateTimeOffset)startDate).ToUnixTimeMilliseconds();
            long endTimestamp = ((DateTimeOffset)startDate.AddHours(1)).ToUnixTimeMilliseconds();
            string url = _endpointUrlTemplate.Replace("{startTimestamp}", startTimestamp.ToString())
                                             .Replace("{endTimestamp}", endTimestamp.ToString());

            try
            {
                string serializedResult = await _client.GetStringAsync(_endpointUrlTemplate);
                dynamic result = JsonConvert.DeserializeObject(serializedResult);
                if (result != null) { return result[0][1]; }

                return null;
            }
            catch (RuntimeBinderException ex)
            {
                _logger.LogError(_parsingExceptionMessage, ex);
                throw GetScrapingException(startDate);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(string.Format(_retrievalErrorLogMessage, url), ex);
                throw GetScrapingException(startDate);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(string.Format(_retrievalErrorLogMessage, url), ex);
                throw GetScrapingException(startDate);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(string.Format(_retrievalErrorLogMessage, url), ex);
                throw GetScrapingException(startDate);
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
