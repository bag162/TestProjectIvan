using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Configuration;
using System.Text;
using System.Text.Json;

namespace FileParserService.QueueManager
{
    public class RabbitMqService : IQueueService
    {
        private ILogger logger;
        private ConnectionFactory connectionFactory;
        public RabbitMqService(ILoggerFactory loggerFactory)
        {
            this.connectionFactory = new ConnectionFactory() { HostName = ConfigurationManager.AppSettings.Get(0)};
            using (var connection = this.connectionFactory.CreateConnection())
            {
                connection.CreateModel().QueueDeclare(queue: "XMlStatusDoc",
                               durable: false,
                               exclusive: false,
                               autoDelete: false,
                               arguments: null);
            }
            this.logger = loggerFactory.CreateLogger<RabbitMqService>();
        }

        public async Task SendMessage(object obj)
        {
            var message = JsonSerializer.Serialize(obj);
            await SendMessage(message);
        }

        public async Task SendMessage(string message)
        {
            try
            {
                using (var connection = this.connectionFactory.CreateConnection()) // Создаем подключение
                {
                    var body = Encoding.UTF8.GetBytes(message); 
                    connection.CreateModel().BasicPublish(exchange: "",
                                       routingKey: "XMlStatusDoc",
                                       basicProperties: null,
                                       body: body); // Отправляем данные в очередь
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("Error send data to RMQ. ", ex);
                throw;
            }
        }
    }
}