using Microsoft.Extensions.Logging;
using Models;
using Newtonsoft.Json;
using System.Xml;

namespace FileParserService
{
    public class XMLSerializer
    {
        private ILogger logger;

        public XMLSerializer(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<XMLSerializer>();
        }

        public string SerializeXmlDocumentToJson(XmlDocument xmlDocument)
        {
            string parsedXmlDoc;
            StatusModel data;
            try
            {
                parsedXmlDoc = JsonConvert.SerializeXmlNode(xmlDocument); // Парсим xml в json
                data = System.Text.Json.JsonSerializer.Deserialize<StatusModel>(parsedXmlDoc); // Парсим json в объект класса StatusModel
            }
            catch (Exception ex)
            {
                this.logger.LogError("Error parse XMLDoc. ", ex);
                throw;
            }

            List<DeviceStatus> parsedList = new List<DeviceStatus>();
            foreach (var item in data.InstrumentStatus.DeviceStatus) // Проходимся под дочернему нераспаршеному xml
            {
                XmlDocument xmlDocToParse = new XmlDocument();
                if (item.RapidControlStatus is null)
                {
                    this.logger.LogWarning("Error parse RapidControlStatus chiled XMLDOC. NULL EXP");
                    continue;
                }
                xmlDocToParse.LoadXml(item.RapidControlStatus);
                var parsedChiledXmlDoc = JsonConvert.SerializeXmlNode(xmlDocToParse); // Парсим xml в json и добавляем в итоговый список
                DeviceStatus newItem = new() { ModuleCategoryID = item.ModuleCategoryID, IndexWithinRole = item.IndexWithinRole, RapidControlStatus = parsedChiledXmlDoc };
                parsedList.Add(newItem);

            }
            data.InstrumentStatus.DeviceStatus = parsedList;
            return JsonConvert.SerializeObject(data); // возвращаем json строку с распаршенными дочерними xml компонентами

        }
    }
}