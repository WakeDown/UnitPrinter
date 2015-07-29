using System;

namespace UnitAnroidPrinterApp
{
	public class Device : IDevice
	{
		private string _serialKey;
		public string SerialKey {
			get {
				return _serialKey;
			}
		}

		private readonly string _nameContrahens;
		public string NameContrahens {
			get {
				return _nameContrahens;
			}
		}
		private readonly string _numberAgreement;
		public string NumberAgreement {
			get {
				return _numberAgreement;
			}
		}
		private readonly string _periodContract;
		public string PeriodContract {
			get{
				return _periodContract;
			}
		}
		private readonly string _addressMachine;
		public string AddressMachine {
			get {
				return _addressMachine;
			}
		}

		public string Model { get; set; }
		public string City { get; set; }
		public string Address { get; set; }
		public string Contrahens { get; set; }

		public Device (string serialKey, Agreement agreement, DeviceInfo deviceInfo)
		{
			_serialKey = serialKey;

			_nameContrahens = agreement.NameContrahens;
			_numberAgreement = agreement.NumberAgreement;
			_periodContract = agreement.PeriodContract;
			_addressMachine = agreement.AddressApparat;

			Model = deviceInfo.Model;
			City = deviceInfo.City;
			Address = deviceInfo.Address;
			Contrahens = deviceInfo.Contrahens;
		}

		public Device(string serialKey, Agreement agreement)
		{
			_serialKey = serialKey;

			_nameContrahens = agreement.NameContrahens;
			_numberAgreement = agreement.NumberAgreement;
			_periodContract = agreement.PeriodContract;
			_addressMachine = agreement.AddressApparat;
		}

		public Device(Device devicePrew)
		{
			_serialKey = devicePrew.SerialKey;
			_nameContrahens = devicePrew.NameContrahens;
			_numberAgreement = devicePrew.NumberAgreement;
			_periodContract = devicePrew.PeriodContract;
			_addressMachine = devicePrew.AddressMachine;
			Model = devicePrew.Model;
			City = devicePrew.City;
			Address = devicePrew.Address;
			Contrahens = devicePrew.Contrahens;
		}
	}
}

