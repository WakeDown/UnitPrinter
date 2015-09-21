using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using SQLite;
using System;
using System.IO;
using System.Net;

namespace UnitAnroidPrinterApp
{
    [Activity(Label = "StartActivity", MainLauncher = true, Icon = "@drawable/icon")]
    public class StartActivity : Activity
    {
        private string _dbUnitAndroidPrinterApp = Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
            "dbUnitAndroidPrinterApp.db3");
        private string _dbUnitAndroidPrinterAppLocal = Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
            "dbUnitAndroidPrinterAppLocal.db3");
        private UnitAPI unitApi = new UnitAPI("mobileUnit_Service", "1qazXSW@", "UN1T");

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Start);
            FindViewById<TextView>(Resource.Id.TitleWait).Text = Resources.GetString(Resource.String.Wait);

            SaveLocalData();
            GetDataAsync();
        }

        private void SaveLocalData()
        {
            using (var dbConnection = new SQLiteConnection(_dbUnitAndroidPrinterAppLocal))
            {
                dbConnection.CreateTable<DispatchDB>();
                var saveLocalTableDispatch = dbConnection.Table<DispatchDB>();
                if (saveLocalTableDispatch.Count() != 0)
                {
                    for(int i = 0;i<saveLocalTableDispatch.Count();i++)
                    {
                        var dispatch = saveLocalTableDispatch.ElementAt(i);
                        try
                        {
                            unitApi.SavePrinterEntry(dispatch);
                            dbConnection.Delete(dispatch);
                        }
                        catch(WebException) { }
                    }
                }
            }
        }

        private async void GetDataAsync()
        {
            using (var dataBaseConnection = new SQLiteConnection(_dbUnitAndroidPrinterApp))
            {
                dataBaseConnection.CreateTable<AccountDB>();
                dataBaseConnection.CreateTable<PrinterEntryDB>();
                dataBaseConnection.CreateTable<TypeWorDB>();

                if (dataBaseConnection.Table<PrinterEntryDB>().Count() == 0 &&
                    dataBaseConnection.Table<AccountDB>().Count() == 0)
                {
                    try
                    {
                        TypeWorDB[] typesWork = unitApi.GetTypesWork();
                        PrinterEntryDB[] printerEntrys = await unitApi.GetAllPrinterEntryAsync();

                        var utf8 = System.Text.Encoding.UTF8.GetString(UnitAPI.GetChecksum(printerEntrys));
                        var ascii = System.Text.Encoding.ASCII.GetString(UnitAPI.GetChecksum(printerEntrys));
                        var Unicode = System.Text.Encoding.Unicode.GetString(UnitAPI.GetChecksum(printerEntrys));
                        var utf32 = System.Text.Encoding.UTF32.GetString(UnitAPI.GetChecksum(printerEntrys));

                        AccountDB[] accounts = await unitApi.GetAllAccountAsync();
                        dataBaseConnection.InsertAll(accounts);
                        dataBaseConnection.InsertAll(printerEntrys);
                        dataBaseConnection.InsertAll(typesWork);
                    }
                    catch (Exception)
                    {
                        FindViewById<TextView>(Resource.Id.TitleWait).Text = Resources.GetString(Resource.String.NoConnectionServer);
                    }
                }
                GoToAutorizationActivity();
            }
        }

        private void GoToAutorizationActivity()
        {
            var activity2 = new Intent(this, typeof(AuthorizationActivity));
            StartActivity(activity2);
        }
    }
}