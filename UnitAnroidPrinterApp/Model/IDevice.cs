namespace UnitAnroidPrinterApp
{
    public interface IDevice
	{
		string DeviceSerialNum { get; }
        int IdDevice { get; }
        string DeviceModel { get; set; }
	}
}

