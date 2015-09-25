namespace UnitAnroidPrinterApp
{
    class CheckSum
    {
        public string CheckSumStr { get; set; }

        public CheckSum() { }

        public static bool operator ==(CheckSum checkSumLeft, CheckSum checkSumRight)
        {
            return checkSumLeft.CheckSumStr == checkSumRight.CheckSumStr;
        }

        public static bool operator !=(CheckSum checkSumLeft, CheckSum checkSumRight)
        {
            return checkSumLeft.CheckSumStr != checkSumRight.CheckSumStr;
        }

        public CheckSum(string checkSum)
        {
            CheckSumStr = checkSum;
        }
    }
}