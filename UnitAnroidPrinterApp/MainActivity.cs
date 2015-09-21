using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using SQLite;
using System;
using System.IO;
using System.Linq;
using System.Net;

namespace UnitAnroidPrinterApp
{
    [Activity (Label = "UnitAnroidPrinterApp", Icon = "@drawable/icon")]
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

        private LinearLayout _layoutInformation;
        private LinearLayout _layoutEnterInformation;
		private LinearLayout _layoutViewInformation;
		private LinearLayout _layoutBid;

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

            _layoutInformation.Visibility = ViewStates.Visible;
            _informationDevice.Text = deviceEntry.GetFormatInformation();
            _informationDevice.Enabled = false;

            Func<string, bool> checkAvailability = (string dataIsEmpty) =>
            dataIsEmpty == null ||
            dataIsEmpty.ToLower() == "none" ||
            dataIsEmpty.ToLower() == "null" ||
            dataIsEmpty == string.Empty;

            _layoutEnterInformation.Visibility = ViewStates.Visible;
            if (!checkAvailability(deviceEntry.DeviceStr))
            {
                _deviceModel.Text = deviceEntry.DeviceStr;
                _deviceModel.Enabled = false;
            }
            if (!checkAvailability(deviceEntry.AddressStr))
            {
                _address.Text = deviceEntry.AddressStr;
                _address.Enabled = false;
            }

            _layoutBid.Visibility = ViewStates.Visible;
            if (!checkAvailability(deviceEntry.DescrStr))
            {
                _comment.Text = deviceEntry.DescrStr;
                _comment.Enabled = false;
            }
        }

		private void HandleClickGetInfo(object sender, EventArgs e)
		{
			//try
			//{
   //             _deviceInfoDB = _unitApi.GetPrinterEntry(_serialKey.Text);
   //             ViewFillForm(_deviceInfoDB);
   //         }
			//catch(Exception)
			//{
				try
				{
                    using (var dbConnection = new SQLiteConnection(_dbUnitAndroidPrinterApp))
                    {
                        _deviceInfoDB = dbConnection.Get<PrinterEntryDB>(
                            x => x.DeviceSerialNum == _serialKey.Text);
                        ViewFillForm(_deviceInfoDB);
                    }
				}
				catch(Exception)
                {
					Toast.MakeText (this, Resource.String.ErrorSerialKey, ToastLength.Long).Show();
				}
            //}
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

            Device device = new Device(_serialKey.Text, _deviceInfoDB.IdDevice, _deviceModel.Text);
            Printer printer = new Printer(device, _monoCounter.Text, _colorCounter.Text);
            TypeWorDB typeWork;
            using (var dbConnection = new SQLiteConnection(_dbUnitAndroidPrinterApp))
            {
                string selectTypeWork = _spinner.SelectedItem.ToString();
                typeWork = dbConnection.Get<TypeWorDB>(x => x.Name == selectTypeWork);
            }
            Dispatch dispatch = new Dispatch(printer, _login, _comment.Text, _cityName.Text,
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
                    //Func<SQLiteConnection, DispatchDB, Type, int> updateEntryDB = (dataBase, obj, objType) =>
                    //{
                    //    var map = dataBase.GetMapping(objType);
                    //    var pk = map.PK;
                    //    var cols = from p in map.Columns
                    //               where p != pk
                    //               select p;
                    //    var vals = (from c in cols select c.GetValue(obj)).ToList();
                    //    vals.Add(obj.DeviceSerialNum);
                    //    var q = string.Format("update \"{0}\" set {1} where {2} = ? ", map.TableName,
                    //        string.Join(",", (from c in cols select "\"" + c.Name + "\" = ? ").ToArray()), 
                    //        "DeviceSerialNum");
                    //    return dataBase.Execute(q, vals.ToArray());
                    //};

                    using (var _dataBase = new SQLiteConnection(_dbUnitAndroidPrinterAppLocal))
                    {
                        //updateEntryDB(_dataBase, dispatchDB, dispatchDB.GetType());
                        _dataBase.Insert(dispatchDB);
                        var qwe = _dataBase.Table<DispatchDB>().Count();
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
            TypeWorDB[] typesWork = _unitApi.GetTypesWork();
            var typesWorkName = (from typeWork in typesWork select typeWork.Name).ToArray();
            var adapter = new ArrayAdapter<string>(this, Resource.Layout.RowSpinner, typesWorkName);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            _spinner = FindViewById<Spinner>(Resource.Id.TypeWork);
            _spinner.Adapter = adapter;
            _monoCounter = FindViewById<EditText>(Resource.Id.MonoCounter);
            _colorCounter = FindViewById<EditText>(Resource.Id.ColorCounter);
            _comment = FindViewById<EditText>(Resource.Id.Comment);

            _layoutInformation = FindViewById<LinearLayout>(Resource.Id.linearLayout1);
            _layoutEnterInformation = FindViewById<LinearLayout>(Resource.Id.linearLayout5);
            _layoutViewInformation = FindViewById<LinearLayout>(Resource.Id.linearLayout6);
            _layoutBid = FindViewById<LinearLayout>(Resource.Id.linearLayout3);
            _layoutEnterInformation.Visibility = ViewStates.Gone;
            _layoutViewInformation.Visibility = ViewStates.Gone;
            _layoutInformation.Visibility = ViewStates.Gone;
            _layoutBid.Visibility = ViewStates.Gone;

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
            _layoutEnterInformation.Visibility = ViewStates.Gone;
            _layoutViewInformation.Visibility = ViewStates.Gone;
            _layoutInformation.Visibility = ViewStates.Gone;
            _layoutBid.Visibility = ViewStates.Gone;
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
            var activity2 = new Intent(this, typeof(CompleteSaveEntry));
            activity2.PutExtra("Name", _login);
            activity2.PutExtra("Pass", _pass);
            StartActivity(activity2);
        }
    }
}


