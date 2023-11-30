using DataProcessorService.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessorService.RMQ
{
	public class RabbitMqListener : BackgroundService
	{
		private IConnection connection;
		private IModel channel;
		private ConnectionFactory connectionFactory;
		private StatusDBService statusesDBService;
		private ILogger logger;

		public RabbitMqListener(ILoggerFactory loggerFactory)
		{
			this.statusesDBService = new StatusDBService(loggerFactory);
			this.connectionFactory = new ConnectionFactory() { HostName = ConfigurationManager.AppSettings.Get(0) };
			this.connection = this.connectionFactory.CreateConnection();
			this.channel = this.connection.CreateModel();
			this.channel.QueueDeclare(queue: "XMlStatusDoc", durable: false, exclusive: false, autoDelete: false, arguments: null);
			this.logger = loggerFactory.CreateLogger<RabbitMqListener>();
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			stoppingToken.ThrowIfCancellationRequested();

			var consumer = new EventingBasicConsumer(this.channel);
			consumer.Received += async (ch, ea) =>
			{
				this.logger.LogInformation("Data received. Start of processing. RoutingK: " + ea.RoutingKey);
				var content = Encoding.UTF8.GetString(ea.Body.ToArray()); // Получаем message
				StatusModel model;
                try
                {
					model = JsonConvert.DeserializeObject<StatusModel>(content); // Парсим message в объект класса StatusModel
				}
                catch (Exception ex)
                {
					this.logger.LogError("Eror pars queue data.", ex);
                    throw;
                }
				this.logger.LogInformation("Data is parsed. RoutingK: " + ea.RoutingKey);
				this.statusesDBService.AddStatusModelAsync(model); // Отправляем на добавление в БД
				this.logger.LogInformation("Data processed. RoutingK: " + ea.RoutingKey);
				this.channel.BasicAck(ea.DeliveryTag, false);
			};

			this.channel.BasicConsume("XMlStatusDoc", false, consumer);

			return;
		}

		public override void Dispose()
		{
			this.channel.Close();
			this.connection.Close();
			base.Dispose();
		}
	}
}