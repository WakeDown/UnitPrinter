using System;
using System.Threading.Tasks;
using SQLite;

namespace UnitAnroidPrinterApp
{
    class UnitAPIShellUpdater : UnitAPIShell
    {
        public UnitAPIShellUpdater(string login, string pass) : base(login, pass) { }

        public async Task<bool> CheckNeedUpdateAsync(DateTime date)
        {
            var dateLastExcharge = date.AddDays(1).ToString("yyyy-MM-dd");
            return await base.CheckNeedUpdateAsync(dateLastExcharge);
        }

        public async Task UpdateAsync()
        {
            using (var dbConnection = new SQLiteConnection(_dbUnitAndroidPrinterApp))
            {
                DateTime dateTime = GetDateLastUpdate().AddDays(1);

                TypeWorkDB[] typesWork = await GetTypesWorkAsync();
                PrinterEntryDB[] printerEntrys = await GetAllPrinterEntryAsync(dateTime.ToString("yyyy-MM-dd"));
                AccountDB[] accounts = await GetAllAccountAsync();

                dbConnection.RunInTransaction(() =>
                {
                    foreach (var item in printerEntrys)
                        dbConnection.InsertOrReplace(item);
                    foreach (var item in accounts)
                        dbConnection.InsertOrReplace(item);
                    foreach (var item in typesWork)
                        dbConnection.InsertOrReplace(item);
						dbConnection.InsertOrReplace(new LastModifyDateDB() { id = 0, LastModifyDate = DateTime.Now });
                });
            }
        }

        public async Task UpdateAsync(DateTime dateTime)
        {
            using (var dbConnection = new SQLiteConnection(_dbUnitAndroidPrinterApp))
            {
                TypeWorkDB[] typesWork = await GetTypesWorkAsync();
                PrinterEntryDB[] printerEntrys = await GetAllPrinterEntryAsync(dateTime.ToString("yyyy-MM-dd"));
                AccountDB[] accounts = await GetAllAccountAsync();

                dbConnection.RunInTransaction(() =>
                {
                    foreach (var item in printerEntrys)
                        dbConnection.InsertOrReplace(item);
                    foreach (var item in accounts)
                        dbConnection.InsertOrReplace(item);
                    foreach (var item in typesWork)
                        dbConnection.InsertOrReplace(item);
                    dbConnection.InsertOrReplace(new LastModifyDateDB() { id = 0, LastModifyDate = dateTime });
                });
            }
        }

        public DateTime GetDateLastUpdate()
        {
            try
            {
                using (var dbConnection = new SQLiteConnection(_dbUnitAndroidPrinterApp))
                {
                    return dbConnection.Table<LastModifyDateDB>().First().LastModifyDate;
                }
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }
    }

}