using System;
using Microsoft.EntityFrameworkCore;
using RatesService.Data;
using RatesService.Messaging;
using RatesService.Model;

namespace RatesService.Services
{
    public class RateChangeService
    {
        private readonly RatesDbContext _context;
        private readonly RateChangePublisher _publisher;

        public RateChangeService(RatesDbContext context, RateChangePublisher publisher)
        {
            _context = context;
            _publisher = publisher;
        }

        public async Task CheckForRateChangesAsync()
        {
            var newRates = await new CoinMarketCapClient().GetCryptoRatesAsync();
            var oldRates = await _context.Rates.ToDictionaryAsync(r => r.InstrumentId, r => r.Price);

            foreach (var rate in newRates)
            {
                if (oldRates.ContainsKey(rate.Key))
                {
                    var oldRate = oldRates[rate.Key];
                    if (Math.Abs((rate.Value - oldRate) / oldRate) > 0.05m)
                    {
                        await _publisher.PublishRateChange(rate.Key, oldRate, rate.Value);
                    }
                }

                _context.Rates.Update(new Rate { InstrumentId = rate.Key, Price = rate.Value, Timestamp = DateTime.UtcNow });
            }

            await _context.SaveChangesAsync();
        }
    }
}

