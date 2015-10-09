using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using SQLite;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Net;
using Android.Net;
using System.Net.NetworkInformation;

namespace UnitAnroidPrinterApp
{
    [Activity (Icon = "@drawable/icon", Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]
	public class MainActivity : Activity
	{
		private string _login;
		private string _pass;

		private EditText _serialKey;

		private EditText _deviceModel;
		private EditText _address;
		private EditText _clientName;

        private Spinner _spinner;
        private EditText _monoCounter;
        private EditText _colorCounter;
        private EditText _comment;

        private LinearLayout _informationLayout;
        private LinearLayout _informationDeviceLayout;
        private LinearLayout _enterDataLayout;

        private LinearLayout _enterDispatchLayout;

		private string _dbUnitAndroidPrinterApp = Path.Combine (
			System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal),
            "dbUnitAndroidPrinterApp.db3");
        private string _dbUnitAndroidPrinterAppCurrent = Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
            "dbUnitAndroidPrinterAppCurrent.db3");

        private UnitAPIShellSaver _unitAPIShellSaver;
        private UnitAPIShellUpdater _unitAPIShellUpdater;
        private PrinterEntryDB _deviceInfoDB;

        private void ViewFillForm(PrinterEntryDB deviceEntry)
		{
            ClearFormInformation();
            _informationLayout.Visibility = ViewStates.Visible;
            _enterDispatchLayout.Visibility = ViewStates.Visible;

            var informationDevice = FindViewById<TextView>(Resource.Id.InformationDevice);
            informationDevice.Text = deviceEntry.GetFormatInformation();

            if (deviceEntry.DeviceId == -1)
            {
                _informationDeviceLayout.Visibility = ViewStates.Gone;
                _enterDataLayout.Visibility = ViewStates.Visible;
            }
            else
            {
                _informationDeviceLayout.Visibility = ViewStates.Visible;
                _enterDataLayout.Visibility = ViewStates.Gone;

                _deviceModel.Text = deviceEntry.DeviceStr;
                _address.Text = deviceEntry.AddressStr;
                _clientName.Text = deviceEntry.ContractorStr;
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
                }
			}
			catch(Exception)
            {
                _deviceInfoDB = new PrinterEntryDB();
                _deviceInfoDB.AddressStr = string.Empty;
                _deviceInfoDB.ContractorStr = string.Empty;
                _deviceInfoDB.ContractStr = string.Empty;
                _deviceInfoDB.DescrStr = string.Empty;
                _deviceInfoDB.DeviceId = -1;
                _deviceInfoDB.DeviceStr = string.Empty;
            }
            ViewFillForm(_deviceInfoDB);
        }

        private void HandleClickSaveInfo(object sender, EventArgs e)
		{
            int monoCounter = 0;
            int colorCounter = 0;
            if (!(int.TryParse(_colorCounter.Text, out colorCounter) && int.TryParse(_monoCounter.Text, out monoCounter)))
            {
                Toast.MakeText(this, Resource.String.IncorrectColor, ToastLength.Long).Show();
                return;
            }
            TypeWorkDB typeWork;
            string specialistSid;
            using (var dbConnection = new SQLiteConnection(_dbUnitAndroidPrinterApp))
            {
                string selectTypeWork = _spinner.SelectedItem.ToString();
                typeWork = dbConnection.Get<TypeWorkDB>(x => x.Name == selectTypeWork);

                specialistSid = dbConnection.Get<AccountDB>(x => x.Login == _login).Sid;
            }
            DispatchDB dispatch = new DispatchDB()
            {
                DeviceSerialNum = _serialKey.Text,
                Address = _address.Text,
                ClientName = _clientName.Text,
                CounterColor = _colorCounter.Text,
                CounterMono = _monoCounter.Text,
                DateCreate = DateTime.Now,
                Descr = _comment.Text,
                DeviceModel = _deviceModel.Text,
                CityName = string.Empty,
                IdDevice = _deviceInfoDB.DeviceId,
                IdWorkType = typeWork.Id,
                SpecialistSid = specialistSid
            };

			try
			{
                _unitAPIShellSaver.PushData(dispatch);
            }
			catch(WebException webEx)
			{
                try
                {
                    _unitAPIShellSaver.SaveLocalData(dispatch);
                }
                catch(SQLiteException dbEx)
                {
                    string resError = Resources.GetString(Resource.String.ErrorSaveInfo);
                    string errorDescr = string.Format(
                        resError + System.Environment.NewLine +
                        "Web error: {0}" + System.Environment.NewLine + "DB error: {1}",
                        webEx.Message, dbEx.Message);
                    Toast.MakeText(this, errorDescr, ToastLength.Long).Show();
                }
			}

            GoCompleteSaveEntryActivity();
		}

        private void Initialize()
        {
            _unitAPIShellUpdater = new UnitAPIShellUpdater(_login, _pass);
            _unitAPIShellSaver = new UnitAPIShellSaver(_login, _pass);
            _serialKey = FindViewById<EditText>(Resource.Id.EnterDeviceSerialKey);

            _deviceModel = FindViewById<EditText>(Resource.Id.DeviceModel);
            _address = FindViewById<EditText>(Resource.Id.Address);
            _clientName = FindViewById<EditText>(Resource.Id.ClientName);

            _spinner = FindViewById<Spinner>(Resource.Id.TypeWork);
            string[] typesWorkName;
            using (var dbConnection = new SQLiteConnection(_dbUnitAndroidPrinterApp))
            {
                typesWorkName = dbConnection.Table<TypeWorkDB>().Select<TypeWorkDB, string>(
                    typeWork => typeWork.Name).ToArray();
            }
            
            var adapter = new ArrayAdapter<string>(this, Resource.Layout.RowSpinner, typesWorkName);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            _spinner = FindViewById<Spinner>(Resource.Id.TypeWork);
            _spinner.Adapter = adapter;
            _monoCounter = FindViewById<EditText>(Resource.Id.MonoCounter);
            _colorCounter = FindViewById<EditText>(Resource.Id.ColorCounter);
            _comment = FindViewById<EditText>(Resource.Id.Comment);

            _informationLayout = FindViewById<LinearLayout>(Resource.Id.informationLayout);
            _enterDataLayout = FindViewById<LinearLayout>(Resource.Id.enterDataLayout);
            _informationDeviceLayout = FindViewById<LinearLayout>(Resource.Id.informationDeviceLayout);
            _enterDispatchLayout = FindViewById<LinearLayout>(Resource.Id.enterDispatchLayout);

            _informationLayout.Visibility = ViewStates.Gone;
            _enterDispatchLayout.Visibility = ViewStates.Gone;

			var buttonGetInfo = FindViewById<Button> (Resource.Id.GetInfoDevice);
			buttonGetInfo.Click += HandleClickGetInfo;
			var buttonSaveInfo = FindViewById<Button> (Resource.Id.Save);
			buttonSaveInfo.Click += HandleClickSaveInfo;

            LinearLayout noServerConnectionLayout = FindViewById<LinearLayout>(Resource.Id.NoServerConnectionLayout);
            LinearLayout changeLayout = FindViewById<LinearLayout>(Resource.Id.ChangeLayout);
            LinearLayout downloadChangeLayout = FindViewById<LinearLayout>(Resource.Id.DownloadChangeLayout);
            noServerConnectionLayout.Visibility = ViewStates.Gone;
            downloadChangeLayout.Visibility = ViewStates.Gone;
            Timer checkNeedUpdate = new Timer(obj =>
            {
                    RunOnUiThread(async () =>
                    {
                        try
                        {
                            if (await _unitAPIShellUpdater.CheckNeedUpdateAsync(_unitAPIShellUpdater.GetDateLastUpdate()))
                                downloadChangeLayout.Visibility = ViewStates.Visible;
                        }
                        catch (WebException)
                        {
                            changeLayout.Visibility = ViewStates.Gone;
                            noServerConnectionLayout.Visibility = ViewStates.Visible;
                        }
                    });
            }, null, 0, 30 * 60 * 1000);
            Timer checkIsOnline = new Timer(obj =>
            {
                RunOnUiThread(() =>
                {
                    ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
                    NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;
                    bool isOnline = (activeConnection != null) && activeConnection.IsConnected;
                    if (isOnline)
                    {
                        changeLayout.Visibility = ViewStates.Visible;
                        noServerConnectionLayout.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        changeLayout.Visibility = ViewStates.Gone;
                        noServerConnectionLayout.Visibility = ViewStates.Visible;
                    }
                });
            }, null, 0, 30 * 1000);
            Timer timerSendDataServer = new Timer(async obj => {
                try
                {
                    var localData = _unitAPIShellSaver.GetLocalData();
                    foreach (var item in localData)
                    {
                        await _unitAPIShellSaver.PushDataAsync(item);
                        _unitAPIShellSaver.DeleteData(item);
                    }
                    if(localData.Length != 0)
                        RunOnUiThread(() => Toast.MakeText(this, Resource.String.CompleteSendDataServer, ToastLength.Long).Show());
                }
                catch (WebException) { }
            }, null, 0, 30 * 60 * 1000);

        var dateLastExcharge = FindViewById<TextView>(Resource.Id.DateLastExcharge);
            dateLastExcharge.Text = _unitAPIShellUpdater.GetDateLastUpdate().ToString("yyyy-MM-dd");

            var buttonChangeDownload = FindViewById<Button>(Resource.Id.DownloadButton);
            buttonChangeDownload.Click += async (obj, state) =>
            {
                try
                {
                    downloadChangeLayout.Visibility = ViewStates.Gone;
                    await _unitAPIShellUpdater.UpdateAsync();
                    dateLastExcharge.Text = _unitAPIShellUpdater.GetDateLastUpdate().ToString("yyyy-MM-dd");
                    Toast.MakeText(this, Resource.String.CompleteDownloadData, ToastLength.Long).Show();
                }
                catch (WebException)
                {
                    changeLayout.Visibility = ViewStates.Gone;
                    noServerConnectionLayout.Visibility = ViewStates.Visible;
                    Toast.MakeText(this, Resource.String.NoConnectionServer, ToastLength.Long).Show();
                }
            };
        }

        private void ClearFormInformation()
        {
            _monoCounter.Text = string.Empty;
            _colorCounter.Text = string.Empty;
            _comment.Text = string.Empty;
            _deviceModel.Text = string.Empty;
            _address.Text = string.Empty;
            _clientName.Text = string.Empty;
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


