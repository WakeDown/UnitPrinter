namespace UnitAnroidPrinterApp
{
    class CheckSum
    {
        public string checkSumStr { get; set; }

        public CheckSum() { }

        public static bool operator == (CheckSum checkSumLeft, CheckSum checkSumRight)
        {
            return checkSumLeft.checkSumStr == checkSumRight.checkSumStr;
        }

        public static bool operator != (CheckSum checkSumLeft, CheckSum checkSumRight)
        {
            return checkSumLeft.checkSumStr == checkSumRight.checkSumStr;
        }

        public CheckSum(string checkSum)
        {
            checkSumStr = checkSum;
        }
    }
}