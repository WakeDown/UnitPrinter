using System;

namespace UnitAnroidPrinterApp
{
	public interface IDevice
	{
		string SerialKey { get; }

		string NameContrahens { get; }
		string NumberAgreement { get; }
		string PeriodContract { get; }
		string AddressMachine { get; }

		string Model { get; set; }
		string City { get; set; }
		string Address { get; set; }
		string Contrahens { get; set; }
	}
}

