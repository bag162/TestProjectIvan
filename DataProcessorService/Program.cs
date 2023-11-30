using DataProcessorService.DB;
using DataProcessorService.RMQ;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using (var db = new XMLDataContext())
{
    db.Database.Migrate();
}

using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
ILogger logger = factory.CreateLogger<Program>();
logger.LogInformation("Start app");

RabbitMqListener rabbitMqListener = new RabbitMqListener(factory);
logger.LogInformation("Start listening to the queue");
while (true)
{
    rabbitMqListener.StartAsync(new CancellationToken()).Wait();
    Task.Delay(1000).Wait();
}