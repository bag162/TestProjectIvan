using FileParserService;
using Microsoft.Extensions.Logging;

// Create logger
using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
ILogger logger = factory.CreateLogger<Program>();
logger.LogInformation("Start app");

TaskManager taskManager = new(factory);

logger.LogInformation("Start monitoring XMLDoc");

while (true)
{
    Task task = taskManager.StartXmlMonitoring();
    task.Wait();
    Task.Delay(1000).Wait();
}