using SQLite;

namespace UnitAnroidPrinterApp
{
    class TypeWorkDB
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string IdParent { get; set; }
        public string Name { get; set; }
        public string SysName { get; set; }
        public string CurUserAdSid { get; set; }
    }
}