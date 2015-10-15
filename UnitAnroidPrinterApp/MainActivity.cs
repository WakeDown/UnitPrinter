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

namespace UnitAnroidPrinterApp
{
    [Activity (Icon = "@drawable/icon", Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]
	public class MainActivity : Activity
	{
		private string m_login;
		private string m_pass;

        private Spinner m_spinner;
        private TextView m_dateLastExcharge;
		private EditText m_serialKey;
		private EditText m_deviceModel;
		private EditText m_address;
		private EditText m_clientName;
        private EditText m_monoCounter;
        private EditText m_colorCounter;
        private EditText m_comment;

        private LinearLayout m_informationLayout;
        private LinearLayout m_informationDeviceLayout;
        private LinearLayout m_enterDataLayout;
        private LinearLayout m_enterDispatchLayout;
        private LinearLayout m_noServerConnectionLayout;
        private LinearLayout m_changeLayout;
        private LinearLayout m_downloadChangeLayout;

		private string m_dbUnitAndroidPrinterApp = Path.Combine (
			System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal),
            "dbUnitAndroidPrinterApp.db3");
        private string m_dbUnitAndroidPrinterAppCurrent = Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
            "dbUnitAndroidPrinterAppCurrent.db3");

        private UnitAPIShellSaver m_unitAPIShellSaver;
        private UnitAPIShellUpdater m_unitAPIShellUpdater;
        private PrinterEntryDB m_deviceInfoDB;

        private void ViewFillForm(PrinterEntryDB deviceEntry)
		{
            ClearFormInformation();
            m_informationLayout.Visibility = ViewStates.Visible;
            m_enterDispatchLayout.Visibility = ViewStates.Visible;

            var informationDevice = FindViewById<TextView>(Resource.Id.InformationDevice);
            informationDevice.Text = deviceEntry.GetFormatInformation();

            if (deviceEntry.DeviceId == -1)
            {
                m_informationDeviceLayout.Visibility = ViewStates.Gone;
                m_enterDataLayout.Visibility = ViewStates.Visible;
            }
            else
            {
                m_informationDeviceLayout.Visibility = ViewStates.Visible;
                m_enterDataLayout.Visibility = ViewStates.Gone;

                m_deviceModel.Text = deviceEntry.DeviceStr;
                m_address.Text = deviceEntry.AddressStr;
                m_clientName.Text = deviceEntry.ContractorStr;
            }
        }

		private void HandleClickGetInfo(object sender, EventArgs e)
		{
            try
			{
                using (var dbConnection = new SQLiteConnection(m_dbUnitAndroidPrinterApp))
                {
                    m_deviceInfoDB = dbConnection.Get<PrinterEntryDB>(
                        x => x.DeviceSerialNum.ToLower() == m_serialKey.Text.ToLower());
                }
			}
			catch(Exception)
            {
                m_deviceInfoDB = new PrinterEntryDB()
                {
                    AddressStr = string.Empty,
                    ContractorStr = string.Empty,
                    ContractStr = string.Empty,
                    DescrStr = string.Empty,
                    DeviceId = -1,
                    DeviceSerialNum = string.Empty,
                    DeviceStr = string.Empty
                };
            }
            ViewFillForm(m_deviceInfoDB);
        }

        private void HandleClickSaveInfo(object sender, EventArgs e)
		{
            TypeWorkDB typeWork;
            string specialistSid;
            using (var dbConnection = new SQLiteConnection(m_dbUnitAndroidPrinterApp))
            {
                string selectTypeWork = m_spinner.SelectedItem.ToString();
                typeWork = dbConnection.Get<TypeWorkDB>(x => x.Name == selectTypeWork);

                specialistSid = dbConnection.Get<AccountDB>(x => x.Login == m_login).Sid;
            }
            DispatchDB dispatch = new DispatchDB()
            {
                DeviceSerialNum = m_serialKey.Text,
                Address = m_address.Text,
                ClientName = m_clientName.Text,
                CounterColor = m_colorCounter.Text,
                CounterMono = m_monoCounter.Text,
                DateCreate = DateTime.Now,
                Descr = m_comment.Text,
                DeviceModel = m_deviceModel.Text,
                CityName = string.Empty,
                IdDevice = m_deviceInfoDB.DeviceId,
                IdWorkType = typeWork.Id,
                SpecialistSid = specialistSid
            };

			try
			{
                m_unitAPIShellSaver.PushData(dispatch);
            }
			catch(WebException webEx)
			{
                try
                {
                    m_unitAPIShellSaver.SaveLocalData(dispatch);
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

        private void InitializeLayout()
        {
            m_informationLayout = FindViewById<LinearLayout>(Resource.Id.informationLayout);
            m_enterDataLayout = FindViewById<LinearLayout>(Resource.Id.enterDataLayout);
            m_informationDeviceLayout = FindViewById<LinearLayout>(Resource.Id.informationDeviceLayout);
            m_enterDispatchLayout = FindViewById<LinearLayout>(Resource.Id.enterDispatchLayout);
            m_spinner = FindViewById<Spinner>(Resource.Id.TypeWork);
            m_informationLayout.Visibility = ViewStates.Gone;
            m_enterDispatchLayout.Visibility = ViewStates.Gone;

            m_noServerConnectionLayout = FindViewById<LinearLayout>(Resource.Id.NoServerConnectionLayout);
            m_changeLayout = FindViewById<LinearLayout>(Resource.Id.ChangeLayout);
            m_downloadChangeLayout = FindViewById<LinearLayout>(Resource.Id.DownloadChangeLayout);
            m_noServerConnectionLayout.Visibility = ViewStates.Gone;
            m_downloadChangeLayout.Visibility = ViewStates.Gone;
        }

        private void InitializeButton()
        {
			Button buttonGetInfo = FindViewById<Button> (Resource.Id.GetInfoDevice);
			buttonGetInfo.Click += HandleClickGetInfo;
			Button buttonSaveInfo = FindViewById<Button> (Resource.Id.Save);
			buttonSaveInfo.Click += HandleClickSaveInfo;
            Button buttonChangeDownload = FindViewById<Button>(Resource.Id.DownloadButton);
            buttonChangeDownload.Click += async (obj, state) =>
            {
                try
                {
                    m_downloadChangeLayout.Visibility = ViewStates.Gone;
                    await m_unitAPIShellUpdater.UpdateAsync();
                    m_dateLastExcharge.Text = m_unitAPIShellUpdater.GetDateLastUpdate().ToString("yyyy-MM-dd HH:mm:ss");
                    Toast.MakeText(this, Resource.String.CompleteDownloadData, ToastLength.Long).Show();
                }
                catch (WebException)
                {
                    m_changeLayout.Visibility = ViewStates.Gone;
                    m_noServerConnectionLayout.Visibility = ViewStates.Visible;
                    Toast.MakeText(this, Resource.String.NoConnectionServer, ToastLength.Long).Show();
                }
            };
            Button exitButton = FindViewById<Button>(Resource.Id.ExitButton);
            exitButton.Click += (obj, state) =>
            {
                var login = "mobileUnit_Service";
                var pass = "1qazXSW@";
                UnitAPIShellAuthorizator authorizator = new UnitAPIShellAuthorizator(login, pass);
                authorizator.DontRememberMe();
                GoAuthorizationActivity();
            };
        }

        private void InitializeSpinner()
        {
            string[] typesWorkName;
            using (var dbConnection = new SQLiteConnection(m_dbUnitAndroidPrinterApp))
            {
                typesWorkName = dbConnection.Table<TypeWorkDB>().Select<TypeWorkDB, string>(
                    typeWork => typeWork.Name).ToArray();
            }
            
            var adapter = new ArrayAdapter<string>(this, Resource.Layout.RowSpinner, typesWorkName);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            m_spinner = FindViewById<Spinner>(Resource.Id.TypeWork);
            m_spinner.Adapter = adapter;
        }

        private void InitializeEnterField()
        {
            m_serialKey = FindViewById<EditText>(Resource.Id.EnterDeviceSerialKey);
            m_deviceModel = FindViewById<EditText>(Resource.Id.DeviceModel);
            m_address = FindViewById<EditText>(Resource.Id.Address);
            m_clientName = FindViewById<EditText>(Resource.Id.ClientName);
            m_monoCounter = FindViewById<EditText>(Resource.Id.MonoCounter);
            m_colorCounter = FindViewById<EditText>(Resource.Id.ColorCounter);
            m_comment = FindViewById<EditText>(Resource.Id.Comment);
            m_dateLastExcharge = FindViewById<TextView>(Resource.Id.DateLastExcharge);
            m_dateLastExcharge.Text = m_unitAPIShellUpdater.GetDateLastUpdate().ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void Initialize()
        {
            m_unitAPIShellUpdater = new UnitAPIShellUpdater(m_login, m_pass);
            m_unitAPIShellSaver = new UnitAPIShellSaver(m_login, m_pass);

            InitializeLayout();
            InitializeButton();
            InitializeSpinner();
            InitializeEnterField();

            Timer checkNeedUpdate = new Timer(obj =>
            {
                    RunOnUiThread(async () =>
                    {
                        try
                        {
                            if (await m_unitAPIShellUpdater.CheckNeedUpdateAsync())
                                m_downloadChangeLayout.Visibility = ViewStates.Visible;
                        }
                        catch (WebException)
                        {
                            m_changeLayout.Visibility = ViewStates.Gone;
                            m_noServerConnectionLayout.Visibility = ViewStates.Visible;
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
                        m_changeLayout.Visibility = ViewStates.Visible;
                        m_noServerConnectionLayout.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        m_changeLayout.Visibility = ViewStates.Gone;
                        m_noServerConnectionLayout.Visibility = ViewStates.Visible;
                    }
                });
            }, null, 0, 30 * 1000);
            Timer timerSendDataServer = new Timer(async obj => {
                try
                {
                    var localData = m_unitAPIShellSaver.GetLocalData();
                    foreach (var item in localData)
                    {
                        await m_unitAPIShellSaver.PushDataAsync(item);
                        m_unitAPIShellSaver.DeleteData(item);
                    }
                    if(localData.Length != 0)
                        RunOnUiThread(() => Toast.MakeText(this, Resource.String.CompleteSendDataServer, ToastLength.Long).Show());
                }
                catch (WebException) { }
            }, null, 0, 30 * 60 * 1000);
        }

        private void ClearFormInformation()
        {
            m_monoCounter.Text = string.Empty;
            m_colorCounter.Text = string.Empty;
            m_comment.Text = string.Empty;
            m_deviceModel.Text = string.Empty;
            m_address.Text = string.Empty;
            m_clientName.Text = string.Empty;
        }

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

            Initialize();

			m_login = Intent.GetStringExtra ("Name");
			m_pass = Intent.GetStringExtra ("Pass");
		}

        void GoCompleteSaveEntryActivity()
        {
            var activity2 = new Intent(this, typeof(CompleteSaveActivity));
            activity2.PutExtra("Name", m_login);
            activity2.PutExtra("Pass", m_pass);
            StartActivity(activity2);
        }

        void GoAuthorizationActivity()
        {
            var activity2 = new Intent(this, typeof(AuthorizationActivity));
            StartActivity(activity2);
        }
    }
}


