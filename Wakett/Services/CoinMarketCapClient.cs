using System;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace RatesService.Services
{
    public class CoinMarketCapClient
    {
        private const string ApiKey = "820ed4d3-0bad-40ee-8399-0011ccde98d6";
        private const string ApiUrl = "https://pro-api.coinmarketcap.com/v1/cryptocurrency/listings/latest?convert=USD";

        public async Task<Dictionary<string, decimal>> GetCryptoRatesAsync()
        {
            var client = new RestClient(ApiUrl);
            var request = new RestRequest();
            request.AddHeader("X-CMC_PRO_API_KEY", ApiKey);

            var response = await client.ExecuteAsync(request);
            var json = JObject.Parse(response.Content);
            var rates = new Dictionary<string, decimal>();

            foreach (var item in json["data"])
            {
                string symbol = item["symbol"].ToString();
                decimal price = item["quote"]["USD"]["price"].Value<decimal>();
                rates[symbol] = price;
            }

            return rates;
        }
    }
}

