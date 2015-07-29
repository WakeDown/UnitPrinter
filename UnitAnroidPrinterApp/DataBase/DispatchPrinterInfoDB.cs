using System;
using SQLite;

namespace UnitAnroidPrinterApp
{
	[Table("DispatchPrinterInfo")]
	public class DispatchPrinterInfoDB
	{
		[MaxLength(100)]
		public string SerialKey { get; set; }

		[MaxLength(100)]
		public string NameContrahens { get; set; }
		[MaxLength(100)]
		public string NumberAgreement { get; set; }
		[MaxLength(100)]
		public string PeriodContract { get; set; }
		[MaxLength(100)]
		public string AddressMachine { get; set; }

		[MaxLength(100)]
		public string Model { get; set; }
		[MaxLength(100)]
		public string City { get; set; }
		[MaxLength(100)]
		public string Address { get; set; }
		[MaxLength(100)]
		public string Contrahens { get; set; }
		[MaxLength(100)]
		public int CounterWhiteAndBlack{ get; set; }
		[MaxLength(100)]
		public int CounterColor{ get; set; }
		[MaxLength(100)]
		public string Name { get; set;}
		[MaxLength(100)]
		public string Pass { get; set;}
		[MaxLength(100)]
		public string Type { get; set; }
		[MaxLength(1000)]
		public string Comment { get; set; }

		public DispatchPrinterInfoDB(DispatchInfo dispatchInfo)
		{
			var printer = (Printer)dispatchInfo.Device;

			SerialKey = printer.SerialKey;
			NameContrahens = printer.NameContrahens;
			NumberAgreement = printer.NumberAgreement;
			PeriodContract = printer.PeriodContract;
			AddressMachine = printer.AddressMachine;
			Model = printer.Model;
			City = printer.City;
			Address = printer.Address;
			Contrahens = printer.Contrahens;
			CounterWhiteAndBlack = printer.CounterWhiteAndBlack;
			CounterColor = printer.CounterColor;

			Name = dispatchInfo.Name;
			Pass = dispatchInfo.Pass;
			Type = dispatchInfo.Type;
			Comment = dispatchInfo.Comment;
		}

		public DispatchPrinterInfoDB() { }
	}
}

