using System;
using SQLite;

namespace UnitAnroidPrinterApp
{
    class DispatchDB
    {
        public string DeviceSerialNum { get; set; }
        [PrimaryKey]
        public int IdDevice { get; set; }
        public string DeviceModel { get; set; }
        public string Descr { get; set; }
        public string CityName { get; set; }
        public string Address { get; set; }
        public string ClientName { get; set; }
        public string IdWorkType { get; set; }
        public string CounterMono { get; set; }
        public string CounterColor { get; set; }
        public string SpecialistSid { get; set; }
        public DateTime DateCreate { get; set; }
    }
}