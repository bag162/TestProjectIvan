using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    // Json Model
    public class StatusModel
    {
        public string Id { get; set; }
        public InstrumentStatus InstrumentStatus { get; set; }
    }

    public class InstrumentStatus
    {
        public string Id { get; set; }
        [JsonProperty("@xmlns:xsi")]
        public string ?xmlnsxsi { get; set; }

        [JsonProperty("@xmlns:xsd")]
        public string ?xmlnsxsd { get; set; }

        [JsonProperty("@schemaVersion")]
        public string ?schemaVersion { get; set; }
        public string ?PackageID { get; set; }
        public List<DeviceStatus> ?DeviceStatus { get; set; }
    }

    public class DeviceStatus
    {
        public string Id { get; set; }
        public string ?ModuleCategoryID { get; set; }
        public string ?IndexWithinRole { get; set; }
        public string ?RapidControlStatus { get; set; }
    }

    // Model for typing
    public enum ModuleState
    {
        Online,
        Run,
        NotReady,
        Offline
    }

    // DB Model
    public class RapidControlStatus
    {
        [Key]
        public string ModuleCategoryID { get; set; }
        public string ModuleState { get; set; }
    }
}


