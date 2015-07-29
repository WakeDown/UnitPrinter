using System;
using SQLite;

namespace UnitAnroidPrinterApp
{
	[Table("Account")]
	public class AccountDB
	{
		[MaxLength(100)]
		public string Name{ get; set; }
		[MaxLength(100)]
		public string Pass{ get; set; }

		public AccountDB(string name, string pass)
		{
			Name = name;
			Pass = pass;
		}

		public AccountDB(){}
	}
}

