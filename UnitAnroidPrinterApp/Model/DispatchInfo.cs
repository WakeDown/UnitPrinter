using System;

namespace UnitAnroidPrinterApp
{
	public class DispatchInfo
	{
		public string Name { get; set; }
		public string Pass { get; set; }
		public IDevice Device { get; set; }
		public string Type { get; set; }
		public string Comment { get; set; }

		public DispatchInfo (string name, string pass, IDevice device, string type, string comment = "None")
		{
			Name = name;
			Pass = pass;
			Device = device;
			Type = type;
			Comment = comment;
		}
	}
}

