namespace UnitAnroidPrinterApp
{
    public interface IDevice
	{
		string DeviceSerialNum { get; }
        string IdDevice { get; }
        string DeviceModel { get; set; }
	}
}

