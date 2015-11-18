using System;
using SQLite;

namespace UnitAnroidPrinterApp
{
    class LastModifyDateDB
    {
        [PrimaryKey]
		public int id { get; set; }
        public DateTime LastModifyDate { get; set; }
    }
}