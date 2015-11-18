using System;
using System.IO;
using SQLite;

namespace UnitAnroidPrinterApp
{
    abstract class UnitAPIShell : UnitAPI
    {
        protected string _dbUnitAndroidPrinterApp = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Personal),
            "dbUnitAndroidPrinterApp.db3");
        protected string _dbUnitAndroidPrinterAppCurrent = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Personal),
            "dbUnitAndroidPrinterCurrent.db3");

        protected UnitAPIShell(string login, string pass) : base(login, pass, "UN1T")
        {
            using (var dbConnection = new SQLiteConnection(_dbUnitAndroidPrinterApp))
            using(var dbConnectionCurrent = new SQLiteConnection(_dbUnitAndroidPrinterAppCurrent))
            {
                dbConnection.CreateTable<AccountDB>();
                dbConnection.CreateTable<PrinterEntryDB>();
                dbConnection.CreateTable<TypeWorkDB>();
                dbConnection.CreateTable<LastModifyDateDB>();

                dbConnectionCurrent.CreateTable<DispatchDB>();
                dbConnectionCurrent.CreateTable<AccountDB>();
            }
        }

        public bool CheckEmptyDB()
        {
            using (var dbConnection = new SQLiteConnection(_dbUnitAndroidPrinterApp))
            {
                return dbConnection.Table<LastModifyDateDB>().Count() == 0 ? true : false;
            }
        }
    }
}