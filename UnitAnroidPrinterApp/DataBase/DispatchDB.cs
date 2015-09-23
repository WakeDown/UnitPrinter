using System;

namespace UnitAnroidPrinterApp
{
    class DispatchDB
    {
        public string DeviceSerialNum { get; set; }

        public string IdDevice { get; set; }

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

        public static DispatchDB Parse(Dispatch dispatch)
        {
            DispatchDB dispatchDB = new DispatchDB();
            dispatchDB.DeviceSerialNum = dispatch.Device.DeviceSerialNum;
            dispatchDB.IdDevice = dispatch.Device.IdDevice;
            dispatchDB.DeviceModel = dispatch.Device.DeviceModel;
            dispatchDB.Descr = dispatch.Descr;
            dispatchDB.CityName = dispatch.CityName;
            dispatchDB.Address = dispatch.Address;
            dispatchDB.ClientName = dispatch.ClientName;
            dispatchDB.IdWorkType = dispatch.IdWorkType;
            dispatchDB.CounterMono = dispatch.Device.ColorCounter;
            dispatchDB.CounterMono = dispatch.Device.MonoCounter;
            dispatchDB.SpecialistSid = dispatch.SpecialistSid;
            dispatchDB.DateCreate = dispatch.DateCreate;

            return dispatchDB;
        }
    }
}