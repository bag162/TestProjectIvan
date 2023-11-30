using Microsoft.Extensions.Logging;
using System.Xml;

namespace FileParserService.Services
{
    public class DataService
    {
        private ILogger logger;

        public DataService(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<DataService>();
        }

        public XmlDocument GetXmlDocument()
        {
            XmlDocument xDoc = new XmlDocument();
            try
            {
                using (FileStream fs = new FileStream("status.xml", FileMode.OpenOrCreate)) // читаем файл в XmlDocument
                {
                    xDoc.Load(fs);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("Error parse XMLDoc.", ex);
                throw;
            }

            return xDoc;
        }
    }
}