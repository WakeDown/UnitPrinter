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
    [Activity(MainLauncher = true, Icon = "@drawable/icon")]
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

            SaveLocalDataAsync();
            GetDataAsync();
        }

        private async void SaveLocalDataAsync()
        {
            using (var dbConnection = new SQLiteConnection(_dbUnitAndroidPrinterAppLocal))
            {
                dbConnection.CreateTable<DispatchDB>();
                var saveLocalTableDispatch = dbConnection.Table<DispatchDB>();
                if (saveLocalTableDispatch.Count() != 0)
                {
                    try
                    {
                        foreach (var dispatch in saveLocalTableDispatch)
                        {

                            await unitApi.SavePrinterEntryAsync(dispatch);
                            dbConnection.Delete(dispatch);
                        }
                        Toast.MakeText(this, Resource.String.CompleteSaveInfo, ToastLength.Long).Show();
                    }
                    catch (Exception e)
                    {
                        Toast.MakeText(this, e.Message, ToastLength.Long).Show();
                    }
                }
            }
        }

        private async void GetDataAsync()
        {
            using (var dbConnection = new SQLiteConnection(_dbUnitAndroidPrinterApp))
            {
                dbConnection.CreateTable<AccountDB>();
                dbConnection.CreateTable<PrinterEntryDB>();
                dbConnection.CreateTable<TypeWorkDB>();
                dbConnection.CreateTable<CheckSum>();

                var oldCheckSumTable = dbConnection.Table<CheckSum>();
                if (oldCheckSumTable.Count() == 0)
                {
                    try
                    {
                        var newCheckSum = new CheckSum(await unitApi.GetCheckSumAsync());
                        TypeWorkDB[] typesWork = await unitApi.GetTypesWorkAsync();
                        PrinterEntryDB[] printerEntrys = await unitApi.GetAllPrinterEntryAsync();
                        AccountDB[] accounts = await unitApi.GetAllAccountAsync();

                        dbConnection.InsertAll(accounts);
                        dbConnection.InsertAll(printerEntrys);
                        dbConnection.InsertAll(typesWork);
                        dbConnection.Insert(newCheckSum);

                        GoToAutorizationActivity();
                    }
                    catch (WebException)
                    {
                        FindViewById<TextView>(Resource.Id.TitleWait).Text =
                                Resources.GetString(Resource.String.NoConnectionServer);
                    }
                }
                else
                {
                    try
                    {
                        var newCheckSum = new CheckSum(await unitApi.GetCheckSumAsync());
                        if (oldCheckSumTable.First() != newCheckSum)
                        {
                            TypeWorkDB[] typesWork = await unitApi.GetTypesWorkAsync();
                            PrinterEntryDB[] printerEntrys = await unitApi.GetAllPrinterEntryAsync();
                            AccountDB[] accounts = await unitApi.GetAllAccountAsync();

                            dbConnection.DeleteAll<AccountDB>();
                            dbConnection.DeleteAll<PrinterEntryDB>();
                            dbConnection.DeleteAll<TypeWorkDB>();
                            dbConnection.DeleteAll<CheckSum>();
                            dbConnection.InsertAll(accounts);
                            dbConnection.InsertAll(printerEntrys);
                            dbConnection.InsertAll(typesWork);
                            dbConnection.Insert(newCheckSum);
                        }
                    }
                    catch (WebException) { }
                    GoToAutorizationActivity();
                }
            }
        }

        private void GoToAutorizationActivity()
        {
            var activity2 = new Intent(this, typeof(AuthorizationActivity));
            StartActivity(activity2);
        }
    }
}