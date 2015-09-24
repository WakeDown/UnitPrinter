using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

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
                    foreach(var dispatch in saveLocalTableDispatch)
                    {
                        try
                        {
                            await unitApi.SavePrinterEntryAsync(dispatch);
                            dbConnection.Delete(dispatch);
                        }
                        catch(WebException) { }
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

                if(false)
                    GoToAutorizationActivity();
                else
                {
                    try
                    {
                        TypeWorkDB[] typesWork = await unitApi.GetTypesWorkAsync();
                        PrinterEntryDB[] printerEntrys = await unitApi.GetAllPrinterEntryAsync();
                        AccountDB[] accounts = await unitApi.GetAllAccountAsync();

                        dbConnection.InsertAll(accounts);
                        dbConnection.InsertAll(printerEntrys);
                        dbConnection.InsertAll(typesWork);

                        //List<DeviceInfoResult> listPrinterEntry = new List<DeviceInfoResult>();
                        //foreach (var printerEntry in printerEntrys)
                        //    listPrinterEntry.Add(new DeviceInfoResult
                        //    {
                        //        id_device = printerEntry.DeviceId,
                        //        contractor_name = printerEntry.ContractorStr,
                        //        contract_number = printerEntry.ContractStr,
                        //        DescrStr = printerEntry.DescrStr,
                        //        device_address = printerEntry.AddressStr,
                        //        device_name = printerEntry.DeviceStr,
                        //        serial_num = printerEntry.DeviceSerialNum
                        //    });


                        //byte[] calculateCheckSum = UnitAPI.CalculateChecksum(listPrinterEntry);
                        //var json = JsonConvert.SerializeObject(calculateCheckSum);
                        //var str = JsonConvert.DeserializeObject(json);

                        //var checkSum = await unitApi.GetCheckSumAsync();

                        GoToAutorizationActivity();
                    }
                    catch (Exception)
                    {
                        if (dbConnection.Table<TypeWorkDB>().Count() != 0 &&
                            dbConnection.Table<PrinterEntryDB>().Count() != 0 &&
                            dbConnection.Table<AccountDB>().Count() != 0)
                            GoToAutorizationActivity();
                        else
                            FindViewById<TextView>(Resource.Id.TitleWait).Text =
                                Resources.GetString(Resource.String.NoConnectionServer);
                    }
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