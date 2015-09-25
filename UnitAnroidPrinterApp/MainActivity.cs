using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using SQLite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UnitAnroidPrinterApp
{
    [Activity (Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		private string _login;
		private string _pass;

		private EditText _serialKey;

		private TextView _informationDevice;

		private EditText _deviceModel;
		private EditText _cityName;
		private EditText _address;
		private EditText _clientName;

        private Spinner _spinner;
        private EditText _monoCounter;
        private EditText _colorCounter;
        private EditText _comment;

        private LinearLayout _informationLayout;
        private LinearLayout _enterModelLayout;
        private LinearLayout _enterAddressLayout;
        private LinearLayout _enterCityNameLayout;
        private LinearLayout _enterClientNameLayout;

        private LinearLayout _enterDispatchLayout;

		private string _dbUnitAndroidPrinterApp = Path.Combine (
			System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal),
            "dbUnitAndroidPrinterApp.db3");
        private string _dbUnitAndroidPrinterAppLocal = Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
            "dbUnitAndroidPrinterAppLocal.db3");

        private UnitAPI _unitApi;

        private PrinterEntryDB _deviceInfoDB;

        private void ViewFillForm(PrinterEntryDB deviceEntry)
		{
            _serialKey.Enabled = false;

            _informationLayout.Visibility = ViewStates.Visible;
            _enterDispatchLayout.Visibility = ViewStates.Visible;
            _informationDevice.Text = deviceEntry.GetFormatInformation();
            _informationDevice.Enabled = false;

            Func<string, bool> checkIsEmpty = (string dataIsEmpty) =>
            dataIsEmpty == null ||
            dataIsEmpty.ToLower() == "none" ||
            dataIsEmpty.ToLower() == "null" ||
            dataIsEmpty == string.Empty;

            if (!checkIsEmpty(deviceEntry.DeviceStr))
            {
                _deviceModel.Text = deviceEntry.DeviceStr;
                _enterModelLayout.Visibility = ViewStates.Gone;
            }
            if (!checkIsEmpty(deviceEntry.AddressStr))
            {
                _address.Text = deviceEntry.AddressStr;
                _enterAddressLayout.Visibility = ViewStates.Gone;
            }

            if (!checkIsEmpty(deviceEntry.DescrStr))
            {
                _comment.Text = deviceEntry.DescrStr;
            }
        }

		private void HandleClickGetInfo(object sender, EventArgs e)
		{
			try
			{
                using (var dbConnection = new SQLiteConnection(_dbUnitAndroidPrinterApp))
                {
                    _deviceInfoDB = dbConnection.Get<PrinterEntryDB>(
                        x => x.DeviceSerialNum.ToLower() == _serialKey.Text.ToLower());
                    ViewFillForm(_deviceInfoDB);
                }
			}
			catch(Exception)
            {
				Toast.MakeText (this, Resource.String.ErrorSerialKey, ToastLength.Long).Show();
			}
        }

        private void HandleClickSaveInfo(object sender, EventArgs e)
		{
            int monoCounter = 0;
            int colorCounter = 0;
            try
            {
                colorCounter = int.Parse(_colorCounter.Text);
                monoCounter = int.Parse(_monoCounter.Text);
            }
            catch(FormatException)
            {
                Toast.MakeText(this, Resource.String.IncorrectColor, ToastLength.Long).Show();
                return;
            }

            Device device = new Device(_serialKey.Text, _deviceInfoDB.DeviceId, _deviceModel.Text);
            Printer printer = new Printer(device, _monoCounter.Text, _colorCounter.Text);
            TypeWorkDB typeWork;
            string specialistSid;
            using (var dbConnection = new SQLiteConnection(_dbUnitAndroidPrinterApp))
            {
                string selectTypeWork = _spinner.SelectedItem.ToString();
                typeWork = dbConnection.Get<TypeWorkDB>(x => x.Name == selectTypeWork);

                specialistSid = dbConnection.Get<AccountDB>(x => x.Login == _login).Sid;
            }
            Dispatch dispatch = new Dispatch(printer, specialistSid, _comment.Text, _cityName.Text,
                _address.Text, _clientName.Text, typeWork.Id);
            DispatchDB dispatchDB = DispatchDB.Parse(dispatch);

			try
			{
                string response = _unitApi.SavePrinterEntry(dispatchDB);
                GoCompleteSaveEntryActivity();
            }
			catch(Exception webEx)
			{
                try
                {
                    using (var _dataBase = new SQLiteConnection(_dbUnitAndroidPrinterAppLocal))
                    {
                        _dataBase.Insert(dispatchDB);
                    }
                    GoCompleteSaveEntryActivity();
                }
                catch(Exception dbEx)
                {
                    string resError = Resources.GetString(Resource.String.ErrorSaveInfo);
                    string errorDescr = string.Format(
                        resError + System.Environment.NewLine +
                        "Web error: {0}" + System.Environment.NewLine + "DB error: {1}",
                        webEx.Message, dbEx.Message);
                    Toast.MakeText(this, errorDescr, ToastLength.Long).Show();
                }
			}
		}

        private void Initialize()
        {
            _unitApi = new UnitAPI(_login, _pass, "UN1T");
            _serialKey = FindViewById<EditText>(Resource.Id.EnterDeviceSerialKey);

            _informationDevice = FindViewById<TextView>(Resource.Id.InformationDevice);

            _deviceModel = FindViewById<EditText>(Resource.Id.DeviceModel);
            _cityName = FindViewById<EditText>(Resource.Id.CityName);
            _address = FindViewById<EditText>(Resource.Id.Address);
            _clientName = FindViewById<EditText>(Resource.Id.ClientName);

            _spinner = FindViewById<Spinner>(Resource.Id.TypeWork);
            string[] typesWorkName;
            using (var dbConnection = new SQLiteConnection(_dbUnitAndroidPrinterApp))
            {
                typesWorkName = dbConnection.Table<TypeWorkDB>().Select<TypeWorkDB, string>(
                    typeWork => typeWork.Name).ToArray(); ;
            }
            
            var adapter = new ArrayAdapter<string>(this, Resource.Layout.RowSpinner, typesWorkName);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            _spinner = FindViewById<Spinner>(Resource.Id.TypeWork);
            _spinner.Adapter = adapter;
            _monoCounter = FindViewById<EditText>(Resource.Id.MonoCounter);
            _colorCounter = FindViewById<EditText>(Resource.Id.ColorCounter);
            _comment = FindViewById<EditText>(Resource.Id.Comment);

            _informationLayout = FindViewById<LinearLayout>(Resource.Id.informationLayout);
            _enterModelLayout = FindViewById<LinearLayout>(Resource.Id.enterModelLayout);
            _enterAddressLayout = FindViewById<LinearLayout>(Resource.Id.enterAddressLayout);
            _enterCityNameLayout = FindViewById<LinearLayout>(Resource.Id.enterCityNameLayout);
            _enterClientNameLayout = FindViewById<LinearLayout>(Resource.Id.enterClientNameLayout);

            _enterDispatchLayout = FindViewById<LinearLayout>(Resource.Id.enterDispatchLayout);

            _informationLayout.Visibility = ViewStates.Gone;
            _enterDispatchLayout.Visibility = ViewStates.Gone;

			var buttonGetInfo = FindViewById<Button> (Resource.Id.GetInfoDevice);
			buttonGetInfo.Click += HandleClickGetInfo;
			var buttonSaveInfo = FindViewById<Button> (Resource.Id.Save);
			buttonSaveInfo.Click += HandleClickSaveInfo;
            var buttonCancel = FindViewById<Button>(Resource.Id.Cancel);
            buttonCancel.Click += ButtonCancel_Click;

            using (var dbConnection = new SQLiteConnection(_dbUnitAndroidPrinterAppLocal))
            {
                dbConnection.CreateTable<DispatchDB>();
            }
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            _informationLayout.Visibility = ViewStates.Gone;
            _enterModelLayout.Visibility = ViewStates.Visible;
            _enterAddressLayout.Visibility = ViewStates.Visible;

            _enterDispatchLayout.Visibility = ViewStates.Gone;

            _monoCounter.Text = string.Empty;
            _colorCounter.Text = string.Empty;
            _comment.Text = string.Empty;
            _deviceModel.Text = string.Empty;
            _cityName.Text = string.Empty;
            _address.Text = string.Empty;
            _clientName.Text = string.Empty;

            _serialKey.Enabled = true;
        }

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

            Initialize();

			_login = Intent.GetStringExtra ("Name");
			_pass = Intent.GetStringExtra ("Pass");
		}

        void GoCompleteSaveEntryActivity()
        {
            var activity2 = new Intent(this, typeof(CompleteSaveActivity));
            activity2.PutExtra("Name", _login);
            activity2.PutExtra("Pass", _pass);
            StartActivity(activity2);
        }
    }
}


