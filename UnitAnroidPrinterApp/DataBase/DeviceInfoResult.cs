using System;

namespace UnitAnroidPrinterApp
{
    [Serializable]
    class DeviceInfoResult
    {
        public string id_device { get; set; }
        public string serial_num { get; set; }
        public string contractor_name { get; set; }
        public string contract_number { get; set; }
        public string device_address { get; set; }
        public string device_name { get; set; }
        public string DescrStr { get; set; }
    }
}