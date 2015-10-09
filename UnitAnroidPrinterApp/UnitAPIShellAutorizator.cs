using System;
using SQLite;

namespace UnitAnroidPrinterApp
{
    class UnitAPIShellAutorizator : UnitAPIShell
    {
        public UnitAPIShellAutorizator(string login, string pass) : base(login, pass) { }

        public void RememberMe(AccountDB account)
        {
            using (var dbConnection = new SQLiteConnection(_dbUnitAndroidPrinterAppCurrent))
            {
                dbConnection.DeleteAll<AccountDB>();
                dbConnection.Insert(new AccountDB()
                {
                    Sid = account.Sid,
                    CurUserAdSid = account.CurUserAdSid,
                    Login = account.Login,
                    Password = account.Password
                });
            }
        }

        public void DontRememberMe()
        {
            using (var dbConnection = new SQLiteConnection(_dbUnitAndroidPrinterAppCurrent))
            {
                dbConnection.DeleteAll<AccountDB>();
            }
        }

        public AccountDB GetRememberAccount()
        {
            try
            {
                using (var dbConnection = new SQLiteConnection(_dbUnitAndroidPrinterAppCurrent))
                {
                    return dbConnection.Table<AccountDB>().First();
                }
            }
            catch(Exception)
            {
                return new AccountDB()
                {
                    CurUserAdSid = string.Empty,
                    Sid = string.Empty,
                    Login = string.Empty,
                    Password = string.Empty
                };
            }
        }

        public bool LogIn(AccountDB account)
        {
            using (var dbConnection = new SQLiteConnection(_dbUnitAndroidPrinterApp))
            {
                try
                {
                    dbConnection.Get<AccountDB>(x => x.Login == account.Login && x.Password == account.Password);
                    return true;
                }
                catch(Exception)
                {
                    return false;
                }
            }
        }

        public bool CheckRememberMe()
        {
            using (var dbConnection = new SQLiteConnection(_dbUnitAndroidPrinterAppCurrent))
            {
                if (dbConnection.Table<AccountDB>().Count() != 0)
                    return true;
                else
                    return false;
            }
        }
    }
}