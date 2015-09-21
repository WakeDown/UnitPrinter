using System;

namespace UnitAnroidPrinterApp
{
    public class Dispatch
    {
        public Printer Device { get; }

        public string Descr { get; set; }

        public string CityName { get; set; }

        public string Address { get; set; }

        public string ClientName { get; set; }

        public string IdWorkType { get; }

        public string SpecialistSid { get; }

        public DateTime DateCreate { get; }

        public Dispatch(Printer device, string specialistSid, string descr = null,
            string cityName = null, string address = null, string clientName = null,
            string idWorkType = null)
        {
            SpecialistSid = specialistSid;

            Device = device;
            Descr = descr;
            CityName = cityName;
            Address = address;
            ClientName = clientName;
            IdWorkType = idWorkType;

            DateCreate = DateTime.Now;
        }
    }
}

