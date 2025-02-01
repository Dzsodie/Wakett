using System;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace RatesService.Messaging
{
    public class RateChangePublisher
    {
        private readonly string _queueName = "rate_changes";

        public async Task PublishRateChange(string instrumentId, decimal oldRate, decimal newRate)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();
            await channel.QueueDeclareAsync(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var message = new { InstrumentId = instrumentId, OldRate = oldRate, NewRate = newRate };
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            var properties = new BasicProperties();

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: _queueName,
                mandatory: false,
                basicProperties: properties,
                body: body
            );
        }
    }
}

