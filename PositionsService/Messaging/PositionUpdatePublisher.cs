using System;
using Newtonsoft.Json;
using PositionsService.Model;
using RabbitMQ.Client;
using System.Text;

namespace PositionsService.Messaging
{
    public class PositionUpdatePublisher
    {
        private readonly string _queueName = "position_updates";

        public async Task PublishPositionUpdate(Position position)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();
            await channel.QueueDeclareAsync(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var message = new { position.InstrumentId, position.ProfitLoss };
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

