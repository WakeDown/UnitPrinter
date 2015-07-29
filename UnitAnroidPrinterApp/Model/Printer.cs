using System;

namespace UnitAnroidPrinterApp
{
	class Printer : Device
	{
		public int CounterWhiteAndBlack{ get; set; }
		public int CounterColor{ get; set; }

		public Printer(Device device, int counterWhiteAndBlack, int counterColor) : base(device)
		{
			CounterWhiteAndBlack = counterWhiteAndBlack;
			CounterColor = counterColor;
		}
	}
}

