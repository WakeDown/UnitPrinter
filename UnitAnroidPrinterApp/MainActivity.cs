using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Linq;
using System.IO;
using System.Net;
using Android.Net;

namespace UnitAnroidPrinterApp
{
	[Activity (Label = "UnitAnroidPrinterApp", Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		private string _urlApi;
		private string _name;
		private string _pass;

		private EditText _serialKey;

		private TextView _nameContrahens;
		private TextView _numberArgreement;
		private TextView _periodContract;
		private TextView _addressMachine;

		private EditText _model;
		private EditText _city;
		private EditText _address;
		private EditText _contrahens;

        private Spinner _spinner;
        private EditText _counterBlackAndWhite;
        private EditText _counterColor;
        private EditText _comment;

		private LinearLayout _layoutMachine;
		private LinearLayout _layoutAgreement;
		private LinearLayout _layoutBid;

		private string _dbPathLocal = Path.Combine (
			System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal),
			"dbDispatchInfoLocal.db3");
		private string _dbPathGlobal = Path.Combine (
			System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal),
			"dbDispatchInfoGlobal.db3");

		private void ViewFillForm(DispatchPrinterInfoDB deviceInfoDB)
		{
			_layoutAgreement.Visibility = ViewStates.Visible;
			_layoutBid.Visibility = ViewStates.Visible;

			_serialKey.Enabled = false;

			_nameContrahens.Text = deviceInfoDB.NameContrahens;
			_numberArgreement.Text = deviceInfoDB.NumberAgreement;
			_periodContract.Text = deviceInfoDB.PeriodContract;
			_addressMachine.Text = deviceInfoDB.AddressMachine;

			_model.Text = deviceInfoDB.Model;
			_city.Text = deviceInfoDB.City;
			_address.Text = deviceInfoDB.Address;
			_contrahens.Text = deviceInfoDB.Contrahens;

			if (deviceInfoDB.Model == string.Empty || deviceInfoDB.City == string.Empty ||
				deviceInfoDB.Address == string.Empty || deviceInfoDB.Contrahens == string.Empty)
				_layoutMachine.Visibility = ViewStates.Visible;
		}

		private void HandleClickGetInfo(object sender, EventArgs e)
		{
			DispatchPrinterInfoDB deviceInfoDB;

			try
			{
				HttpWebRequest requestInfo = (HttpWebRequest)WebRequest.Create(_urlApi);
				requestInfo.Accept = "Application/Json";
				requestInfo.Method = "Post";
				using(var responseInfo = requestInfo.GetResponse())
				{
					using(var streamReader = new StreamReader(responseInfo.GetResponseStream()))
					{
						string info = streamReader.ReadToEnd();
						deviceInfoDB = JsonConvert.DeserializeObject<DispatchPrinterInfoDB>(info);
						ViewFillForm (deviceInfoDB);
					}
				}
			}
			catch(Exception)
			{
				try
				{
					using (var dataBase = new SQLiteConnection (_dbPathGlobal)) {
						deviceInfoDB = dataBase.Get<DispatchPrinterInfoDB> (x => x.SerialKey == _serialKey.Text);
						}
					ViewFillForm (deviceInfoDB);
				}
				catch(Exception) {
					Toast.MakeText (this, Resource.String.ErrorSerialKey, ToastLength.Long).Show();
				}
			}
		}

		private void HandleClickSaveInfo(object sender, EventArgs e)
		{
			Device device = new Device (_serialKey.Text,
				                new Agreement () {
					NameContrahens = _nameContrahens.Text,
					NumberAgreement = _numberArgreement.Text,
					PeriodContract = _periodContract.Text,
					AddressApparat = _addressMachine.Text
				},
				                new DeviceInfo () {
					Model = _model.Text,
					City = _city.Text,
					Address = _address.Text,
					Contrahens = _contrahens.Text
				});
            Printer printer = new Printer(
                device,
                Int32.Parse(_counterBlackAndWhite.Text),
                Int32.Parse(_counterColor.Text));
			DispatchInfo dispatchtInfo = new DispatchInfo(
				_name,
				_pass,
				printer,
				_spinner.SelectedItem.ToString(),
                _comment.Text);
			DispatchPrinterInfoDB deviceInfoDB = new DispatchPrinterInfoDB (dispatchtInfo);

			try
			{
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_urlApi);
				using(StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
				{
					string jsonObj = JsonConvert.SerializeObject(deviceInfoDB);
					streamWriter.WriteLine(jsonObj);
				}
				request.Accept = "Application/Json";
				request.Method = "Post";
				using(var responseInfo = request.GetResponse()) { 
					using(var streamReader = new StreamReader(responseInfo.GetResponseStream()))
					{
						string strResponse = streamReader.ReadToEnd();
						if (strResponse == "OK")
							Toast.MakeText (this, Resource.String.CompleteSaveInfo, ToastLength.Long);
					}
				}
			}
			catch(Exception)
			{
				using (var _dataBase = new SQLiteConnection (_dbPathGlobal)) {
					_dataBase.Insert (deviceInfoDB);
				}
				using (var _dataBase = new SQLiteConnection (_dbPathLocal)) {
					_dataBase.Insert (deviceInfoDB);
				}
				Toast.MakeText (this, Resource.String.CompleteSaveInfo, ToastLength.Long);
			}
		}

        private void Initialize()
        {
            _serialKey = FindViewById<EditText>(Resource.Id.EnterDeviceSerialKey);

            _nameContrahens = FindViewById<TextView>(Resource.Id.NameContrahens);
            _numberArgreement = FindViewById<TextView>(Resource.Id.NumberArgreement);
            _periodContract = FindViewById<TextView>(Resource.Id.PeriodContract);
            _addressMachine = FindViewById<TextView>(Resource.Id.AddressMachine);

            _model = FindViewById<EditText>(Resource.Id.Model);
            _city = FindViewById<EditText>(Resource.Id.City);
            _address = FindViewById<EditText>(Resource.Id.Address);
            _contrahens = FindViewById<EditText>(Resource.Id.Contrahens);

            _spinner = FindViewById<Spinner>(Resource.Id.TypeWork);
            var adapter = ArrayAdapter.CreateFromResource(
                this, Resource.Array.TypeWorkArray, Resource.Layout.RowSpinner);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            _spinner = FindViewById<Spinner>(Resource.Id.TypeWork);
            _spinner.Adapter = adapter;
            _counterBlackAndWhite = FindViewById<EditText>(Resource.Id.BlackAndWhiteCounter);
            _counterColor = FindViewById<EditText>(Resource.Id.ColorCounter);
            _comment = FindViewById<EditText>(Resource.Id.Comment);

            _layoutMachine = FindViewById<LinearLayout>(Resource.Id.linearLayout5);
            _layoutAgreement = FindViewById<LinearLayout>(Resource.Id.linearLayout1);
            _layoutBid = FindViewById<LinearLayout>(Resource.Id.linearLayout3);
            _layoutMachine.Visibility = ViewStates.Gone;
            _layoutAgreement.Visibility = ViewStates.Gone;
            _layoutBid.Visibility = ViewStates.Gone;

			var buttonGetInfo = FindViewById<Button> (Resource.Id.GetInfoDevice);
			buttonGetInfo.Click += HandleClickGetInfo;
			var buttonSaveInfo = FindViewById<Button> (Resource.Id.Save);
			buttonSaveInfo.Click += HandleClickSaveInfo;

			using (var dataBase = new SQLiteConnection (_dbPathGlobal)) {
				dataBase.CreateTable<DispatchPrinterInfoDB> ();
			}
			using (var dataBase = new SQLiteConnection (_dbPathLocal)) {
				dataBase.CreateTable<DispatchPrinterInfoDB> ();
			}

			_urlApi = Resources.GetString (Resource.String.URLApi);
        }

		private void test()
		{
			var deviceInfoDB =
				new DispatchPrinterInfoDB (
					new DispatchInfo (
						_name,
						_pass,
						new Printer (
							new Device (
								"123",
								new Agreement () {
									NameContrahens = "NameContrahens",
									NumberAgreement = "NumberAgreement",
									PeriodContract = "PeriodContract",
									AddressApparat = "AddressApparat"
								}),
							123,
							123
						),
						"Bid"
					));

			using (var _dataBase = new SQLiteConnection (_dbPathGlobal)) {
				_dataBase.Insert (deviceInfoDB);
			}
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

            Initialize();

			_name = Intent.GetStringExtra ("Name");
			_pass = Intent.GetStringExtra ("Pass");

			test ();
		
//			var connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
//			var activeConnection = connectivityManager.ActiveNetworkInfo;
//			if ((activeConnection != null)  && activeConnection.IsConnected)
//			{
//				try{
//					using (var dataBase = new SQLiteConnection (_dbPathLocal)) {
//						var dispatchInfoDBAll = dataBase.Table<DispatchPrinterInfoDB> ().ToArray();
//						string jsonObj = JsonConvert.SerializeObject(dispatchInfoDBAll);
//						HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_urlApi);
//						using(StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
//						{
//							streamWriter.WriteLine(jsonObj);
//						}
//						request.Accept = "Application/Json";
//						request.Method = "Post";
//						using(var responseInfo = request.GetResponse()) { }
//
//						dataBase.DeleteAll<DispatchPrinterInfoDB>();
//					}
//				}
//				catch(Exception) {
//				}
//
//				using(var dataBase = new SQLiteConnection(_dbPathGlobal)) {
//					//закачать всю базу с сервера
//				}
//			}
		}
	}
}


