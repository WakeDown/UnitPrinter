namespace UnitAnroidPrinterApp
{
    public class Device : IDevice
	{
		public string DeviceSerialNum { get; }

        public int IdDevice { get; }

        public string DeviceModel { get; set; }

        public Device (string serialNum, int idDevice)
        {
            DeviceSerialNum = serialNum;
            IdDevice = idDevice;
        }

		public Device (string serialNum, int idDevice, string deviceModel)
		{
            DeviceSerialNum = serialNum;
            IdDevice = idDevice;
            DeviceModel = deviceModel;
		}

        public Device (Device device)
        {
            DeviceSerialNum = device.DeviceSerialNum;
            IdDevice = device.IdDevice;
            DeviceModel = device.DeviceModel;
        }
	}
}

