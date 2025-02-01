using System;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PositionsService.Data;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PositionsService.Messaging
{
    public class RateChangeConsumer
    {
        private readonly PositionsDbContext _context;

        public RateChangeConsumer(PositionsDbContext context)
        {
            _context = context;
        }

        public async Task StartListening()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();
            await channel.QueueDeclareAsync(queue: "rate_changes", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var data = JsonConvert.DeserializeObject<dynamic>(message);

                string instrumentId = data.InstrumentId;
                decimal newRate = data.NewRate;

                var position = await _context.Positions.FirstOrDefaultAsync(p => p.InstrumentId == instrumentId);
                if (position != null)
                {
                    position.CurrentRate = newRate;
                    position.ProfitLoss = position.Quantity * (newRate - position.InitialRate) * Convert.ToDecimal(position.Side);
                    await _context.SaveChangesAsync();
                }
            };

            await channel.BasicConsumeAsync(queue: "rate_changes", autoAck: true, consumer: consumer);
            Console.ReadLine();
        }
    }
}

