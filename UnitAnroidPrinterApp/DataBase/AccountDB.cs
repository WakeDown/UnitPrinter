﻿using SQLite;

namespace UnitAnroidPrinterApp
{
    public class AccountDB
	{
        [PrimaryKey]
        public string Sid { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string CurUserAdSid { get; set; }
	}
}

