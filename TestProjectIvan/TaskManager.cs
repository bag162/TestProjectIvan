using FileParserService.QueueManager;
using FileParserService.Services;
using Microsoft.Extensions.Logging;
using Models;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Xml;

namespace FileParserService
{
    public class TaskManager
    {
        private ILogger logger;
        private RabbitMqService rabbitMqService;
        static Random random = new Random();
        private XMLSerializer XMLSerializer;
        private DataService dataService;
        public TaskManager(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<TaskManager>();
            this.XMLSerializer = new XMLSerializer(loggerFactory);
            this.dataService = new DataService(loggerFactory);
            this.rabbitMqService = new RabbitMqService(loggerFactory);
        }

        public async Task StartXmlMonitoring()
        {
            this.logger.LogInformation("Receiving an XML file");
            XmlDocument xmlData = this.dataService.GetXmlDocument(); // Получение XML файла
            this.logger.LogInformation("XML File Processing");
            string jsonXMLData = XMLSerializer.SerializeXmlDocumentToJson(xmlData); // Сериализация его в JSON, в том числе и его дочерних компонентов (RapidControlStatus)
            var data = JsonConvert.DeserializeObject<StatusModel>(jsonXMLData); // Сериализуем в объект для удобной обработки
            List<DeviceStatus> correctDeviceStatus = new();
            if (data.InstrumentStatus.DeviceStatus is null)
            {
                this.logger.LogWarning("Device status is null");
                return;
            }
            foreach (var item in data.InstrumentStatus.DeviceStatus)
            {
                Regex regex = new Regex(@".ModuleState.:.(\w+).");
                var enumValues = Enum.GetValues(typeof(ModuleState)).Cast<ModuleState>().ToArray();
                string replacement = enumValues[random.Next(enumValues.Length)].ToString();
                try
                {
                    if (item.RapidControlStatus is null)
                        continue;

                    item.RapidControlStatus = regex.Replace(item.RapidControlStatus, (group) =>
                    {
                        return group.Value.Replace("Online", replacement); 
                    });
                    // Используем регулярное выражение для замены ModuleState на случайное значение, т.к. типизация RapidControlStatus невозможна в данном случае. ==>
                    // Тестовые данные status.xml: ln6 col78, ln11 col 78, ln16 col 78
                    // (Родительские элементы имеют разное именование: CombinedPumpStatus, CombinedSamplerStatus, CombinedOvenStatus)
                    // Собрать классы для их парсинга не является правильным решением, т.к. я не знаю точный формат входных данных
                    // Перебирать свойства анонимного объекта является небезопасным.
                }
                catch (Exception ex )
                {
                    this.logger.LogError("Error processing regular expression. ", ex);
                    throw;
                }
            }
            string sendedData = JsonConvert.SerializeObject(data); // Сериализуем данные в JSON
            this.logger.LogInformation("Sending an XML file to the queue");
            await rabbitMqService.SendMessageAsync(sendedData); // Отправляем в очередь
            return;
        }
    }
}