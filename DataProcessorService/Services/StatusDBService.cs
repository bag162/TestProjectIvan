using DataProcessorService.DB;
using Microsoft.Extensions.Logging;
using Models;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace DataProcessorService.Services
{
    public class StatusDBService
    {
        private XMLDataContext DataContext;
        private ILogger logger;
        public StatusDBService(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<StatusDBService>();
        }

        public async Task AddStatusModelAsync(StatusModel xmlDoc)
        {
            this.DataContext = new XMLDataContext();
            if (xmlDoc.InstrumentStatus.DeviceStatus is not null) // EXP если нет данных
            {
                foreach (DeviceStatus deviceStatus in xmlDoc.InstrumentStatus.DeviceStatus)
                {
                    if (deviceStatus.RapidControlStatus is not null & deviceStatus.ModuleCategoryID is not null) // EXP если нет данных
                    {
                        Regex regex = new Regex(@".ModuleState.:.(\w+)."); // Вытягиваем ModuleState value
                        string moduleState;
                        try
                        {
                            moduleState = regex.Match(deviceStatus.RapidControlStatus).Groups[1].ToString();
                        }
                        catch (Exception ex)
                        {
                            this.logger.LogError("Error parse \'ModuleState\' Data.", ex);
                            throw;
                        }
                        

                        RapidControlStatus addedData = new() { ModuleCategoryID = deviceStatus.ModuleCategoryID, ModuleState = moduleState };
                        if (DataContext.status.Contains(addedData)) // Проверяем есть ли RapidControlStatus уже в базе
                        {
                            DataContext.status.Update(addedData); // Обновляем старый RapidControlStatus
                        }
                        else
                        {
                            await DataContext.status.AddAsync(addedData); // Добавляем новый RapidControlStatus
                        }

                    }
                    else
                    {
                        this.logger.LogWarning("Error parsing RapidControlStatus & ModuleCategoryID. NULL EXP");
                    }
                }

                await DataContext.SaveChangesAsync(); // Сохраняем изменения RapidControlStatus
            }
            else
            {
                this.logger.LogWarning("Error unpacking DeviceStatus. NULL EXP");
            }
        }
    }
}