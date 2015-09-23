namespace UnitAnroidPrinterApp
{
    public class Device : IDevice
	{
		public string DeviceSerialNum { get; }

        public string IdDevice { get; }

        public string DeviceModel { get; set; }

        public Device (string serialNum, string idDevice)
        {
            DeviceSerialNum = serialNum;
            IdDevice = idDevice;
        }

		public Device (string serialNum, string idDevice, string deviceModel)
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

