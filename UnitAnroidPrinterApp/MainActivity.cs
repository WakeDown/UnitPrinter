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
using Android.Views.InputMethods;

namespace UnitAnroidPrinterApp
{
    [Activity(Icon = "@drawable/icon", Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]
    public class MainActivity : Activity
    {
        private string mLogin;
        private string mPass;

        private Spinner mSpinner;
        private TextView mDateLastExcharge;
        private EditText mSerialKey;
        private EditText mDeviceModel;
        private EditText mAddress;
        private EditText mClientName;
        private EditText mMonoCounter;
        private EditText mColorCounter;
        private EditText mComment;

        private LinearLayout mInformationLayout;
        private LinearLayout mInformationDeviceLayout;
        private LinearLayout mEnterDataLayout;
        private LinearLayout mEnterDispatchLayout;
        private LinearLayout mNoServerConnectionLayout;
        private LinearLayout mChangeLayout;
        private LinearLayout mDownloadChangeLayout;

        private string mDBUnitAndroidPrinterApp = Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
            "dbUnitAndroidPrinterApp.db3");
        private string mDBUnitAndroidPrinterAppCurrent = Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
            "dbUnitAndroidPrinterAppCurrent.db3");

        private UnitAPIShellSaver m_unitAPIShellSaver;
        private UnitAPIShellUpdater m_unitAPIShellUpdater;
        private PrinterEntryDB m_deviceInfoDB;

        private void ViewFillForm(PrinterEntryDB deviceEntry)
        {
            ClearFormInformation();
            mInformationLayout.Visibility = ViewStates.Visible;
            mEnterDispatchLayout.Visibility = ViewStates.Visible;

            var informationDevice = FindViewById<TextView>(Resource.Id.InformationDevice);
            informationDevice.Text = deviceEntry.GetFormatInformation();

            if (deviceEntry.DeviceId == -1)
            {
                mInformationDeviceLayout.Visibility = ViewStates.Gone;
                mEnterDataLayout.Visibility = ViewStates.Visible;
            }
            else
            {
                mInformationDeviceLayout.Visibility = ViewStates.Visible;
                mEnterDataLayout.Visibility = ViewStates.Gone;

                mDeviceModel.Text = deviceEntry.DeviceStr;
                mAddress.Text = deviceEntry.AddressStr;
                mClientName.Text = deviceEntry.ContractorStr;
            }
        }

        private void HandleClickGetInfo(object sender, EventArgs e)
        {
            HideKeyboard();
            try
            {
                using (var dbConnection = new SQLiteConnection(mDBUnitAndroidPrinterApp))
                {
                    m_deviceInfoDB = dbConnection.Get<PrinterEntryDB>(
                        x => x.DeviceSerialNum.ToLower() == mSerialKey.Text.ToLower());
                }
            }
            catch (Exception)
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
            HideKeyboard();
            TypeWorkDB typeWork;
            string specialistSid;
            using (var dbConnection = new SQLiteConnection(mDBUnitAndroidPrinterApp))
            {
                string selectTypeWork = mSpinner.SelectedItem.ToString();
                typeWork = dbConnection.Get<TypeWorkDB>(x => x.Name == selectTypeWork);

                specialistSid = dbConnection.Get<AccountDB>(x => x.Login == mLogin).Sid;
            }
            DispatchDB dispatch = new DispatchDB()
            {
                DeviceSerialNum = mSerialKey.Text,
                Address = mAddress.Text,
                ClientName = mClientName.Text,
                CounterColor = mColorCounter.Text,
                CounterMono = mMonoCounter.Text,
                DateCreate = DateTime.Now,
                Descr = mComment.Text,
                DeviceModel = mDeviceModel.Text,
                CityName = string.Empty,
                IdDevice = m_deviceInfoDB.DeviceId,
                IdWorkType = typeWork.Id,
                SpecialistSid = specialistSid
            };

            try
            {
                m_unitAPIShellSaver.PushData(dispatch);
            }
            catch (WebException webEx)
            {
                try
                {
                    m_unitAPIShellSaver.SaveLocalData(dispatch);
                }
                catch (SQLiteException dbEx)
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
            mInformationLayout = FindViewById<LinearLayout>(Resource.Id.informationLayout);
            mEnterDataLayout = FindViewById<LinearLayout>(Resource.Id.enterDataLayout);
            mInformationDeviceLayout = FindViewById<LinearLayout>(Resource.Id.informationDeviceLayout);
            mEnterDispatchLayout = FindViewById<LinearLayout>(Resource.Id.enterDispatchLayout);
            mSpinner = FindViewById<Spinner>(Resource.Id.TypeWork);
            mInformationLayout.Visibility = ViewStates.Gone;
            mEnterDispatchLayout.Visibility = ViewStates.Gone;

            mNoServerConnectionLayout = FindViewById<LinearLayout>(Resource.Id.NoServerConnectionLayout);
            mChangeLayout = FindViewById<LinearLayout>(Resource.Id.ChangeLayout);
            mDownloadChangeLayout = FindViewById<LinearLayout>(Resource.Id.DownloadChangeLayout);
            mNoServerConnectionLayout.Visibility = ViewStates.Gone;
            mDownloadChangeLayout.Visibility = ViewStates.Gone;

            FindViewById<LinearLayout>(Resource.Id.linearLayout4).Touch += Element_Touch;
        }

        public override void OnBackPressed()
        {
            Intent intent = new Intent(Intent.ActionMain);
            intent.AddCategory(Intent.CategoryHome);
            intent.SetFlags(ActivityFlags.ClearTop);
            StartActivity(intent);
            Finish();
            Process.KillProcess(Process.MyPid());
        }

        private void InitializeButton()
        {
            Button buttonGetInfo = FindViewById<Button>(Resource.Id.GetInfoDevice);
            buttonGetInfo.Click += HandleClickGetInfo;
            Button buttonSaveInfo = FindViewById<Button>(Resource.Id.Save);
            buttonSaveInfo.Click += HandleClickSaveInfo;
            Button buttonChangeDownload = FindViewById<Button>(Resource.Id.DownloadButton);
            buttonChangeDownload.Click += async (obj, state) =>
            {
                HideKeyboard();
                try
                {
                    mDownloadChangeLayout.Visibility = ViewStates.Gone;
                    await m_unitAPIShellUpdater.UpdateAsync();
                    mDateLastExcharge.Text = m_unitAPIShellUpdater.GetDateLastUpdate().ToString("yyyy-MM-dd HH:mm:ss");
                    Toast.MakeText(this, Resource.String.CompleteDownloadData, ToastLength.Long).Show();
                }
                catch (WebException)
                {
                    mChangeLayout.Visibility = ViewStates.Gone;
                    mNoServerConnectionLayout.Visibility = ViewStates.Visible;
                    Toast.MakeText(this, Resource.String.NoConnectionServer, ToastLength.Long).Show();
                }
            };
            Button exitButton = FindViewById<Button>(Resource.Id.ExitButton);
            exitButton.Click += (obj, state) =>
            {
                HideKeyboard();
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
            using (var dbConnection = new SQLiteConnection(mDBUnitAndroidPrinterApp))
            {
                typesWorkName = dbConnection.Table<TypeWorkDB>().Select<TypeWorkDB, string>(
                    typeWork => typeWork.Name).ToArray();
            }

            var adapter = new ArrayAdapter<string>(this, Resource.Layout.RowSpinner, typesWorkName);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            mSpinner = FindViewById<Spinner>(Resource.Id.TypeWork);
            mSpinner.Adapter = adapter;
        }

        private void InitializeEnterField()
        {
            mSerialKey = FindViewById<EditText>(Resource.Id.EnterDeviceSerialKey);
            mDeviceModel = FindViewById<EditText>(Resource.Id.DeviceModel);
            mAddress = FindViewById<EditText>(Resource.Id.Address);
            mClientName = FindViewById<EditText>(Resource.Id.ClientName);
            mMonoCounter = FindViewById<EditText>(Resource.Id.MonoCounter);
            mColorCounter = FindViewById<EditText>(Resource.Id.ColorCounter);
            mComment = FindViewById<EditText>(Resource.Id.Comment);
            mDateLastExcharge = FindViewById<TextView>(Resource.Id.DateLastExcharge);
            mDateLastExcharge.Text = m_unitAPIShellUpdater.GetDateLastUpdate().ToString("yyyy-MM-dd HH:mm:ss");
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            HideKeyboard();
            return true;
        }

        private void HideKeyboard()
        {
            InputMethodManager inputMethodManager = (InputMethodManager)GetSystemService(Context.InputMethodService);
            inputMethodManager.HideSoftInputFromWindow(CurrentFocus.WindowToken, HideSoftInputFlags.None);
        }

        private void Element_Touch(object sender, View.TouchEventArgs e)
        {
            HideKeyboard();
        }

        private void Initialize()
        {
            m_unitAPIShellUpdater = new UnitAPIShellUpdater(mLogin, mPass);
            m_unitAPIShellSaver = new UnitAPIShellSaver(mLogin, mPass);

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
                            mDownloadChangeLayout.Visibility = ViewStates.Visible;
                    }
                    catch (WebException)
                    {
                        mChangeLayout.Visibility = ViewStates.Gone;
                        mNoServerConnectionLayout.Visibility = ViewStates.Visible;
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
                        mChangeLayout.Visibility = ViewStates.Visible;
                        mNoServerConnectionLayout.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        mChangeLayout.Visibility = ViewStates.Gone;
                        mNoServerConnectionLayout.Visibility = ViewStates.Visible;
                    }
                });
            }, null, 0, 30 * 1000);
            Timer timerSendDataServer = new Timer(async obj =>
            {
                try
                {
                    var localData = m_unitAPIShellSaver.GetLocalData();
                    foreach (var item in localData)
                    {
                        await m_unitAPIShellSaver.PushDataAsync(item);
                        m_unitAPIShellSaver.DeleteData(item);
                    }
                    if (localData.Length != 0)
                        RunOnUiThread(() => Toast.MakeText(this, Resource.String.CompleteSendDataServer, ToastLength.Long).Show());
                }
                catch (WebException) { }
            }, null, 0, 30 * 60 * 1000);
        }

        private void ClearFormInformation()
        {
            mMonoCounter.Text = string.Empty;
            mColorCounter.Text = string.Empty;
            mComment.Text = string.Empty;
            mDeviceModel.Text = string.Empty;
            mAddress.Text = string.Empty;
            mClientName.Text = string.Empty;
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            Initialize();

            mLogin = Intent.GetStringExtra("Name");
            mPass = Intent.GetStringExtra("Pass");
        }

        void GoCompleteSaveEntryActivity()
        {
            var intent = new Intent(this, typeof(CompleteSaveActivity));
            intent.PutExtra("Name", mLogin);
            intent.PutExtra("Pass", mPass);
            StartActivity(intent);
        }

        void GoAuthorizationActivity()
        {
            var intent = new Intent(this, typeof(AuthorizationActivity));
            StartActivity(intent);
        }
    }
}


