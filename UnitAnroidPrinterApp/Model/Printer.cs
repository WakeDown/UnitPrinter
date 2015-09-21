namespace UnitAnroidPrinterApp
{
    public class Printer : Device
	{
		public string MonoCounter { get; set; }
		public string ColorCounter { get; set; }

        public Printer(Device device) : base(device) { }

		public Printer(Device device, string counterMono,
            string counterColor) : base(device)
		{
			MonoCounter = counterMono;
            ColorCounter = counterColor;
		}
	}
}

