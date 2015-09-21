using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;

namespace UnitAnroidPrinterApp
{
    class UnitAPI
    {
        private readonly string _login;
        private readonly string _pass;
        private readonly string _domain;

        public UnitAPI(string login, string pass, string domain)
        {
            _login = login;
            _pass = pass;
            _domain = domain;
            _domain = "UN1T";
            _login = "mobileUnit_Service";
            _pass = "1qazXSW@";
        }

        private HttpWebRequest _getApiRequest(string url)
        {
            ServicePointManager.ServerCertificateValidationCallback =
                new System.Net.Security.RemoteCertificateValidationCallback(delegate { return true; });

            HttpWebRequest requestGetPrinterEntry = (HttpWebRequest)WebRequest.Create(url);
            requestGetPrinterEntry.ContentType = "text/json";
            requestGetPrinterEntry.Credentials = new NetworkCredential(_login,
                _pass, _domain);

            return requestGetPrinterEntry;
        }

        private HttpWebRequest _getApiRequest(string url, string json)
        {
            ServicePointManager.ServerCertificateValidationCallback =
                new System.Net.Security.RemoteCertificateValidationCallback(delegate { return true; });

            HttpWebRequest requestGetPrinterEntry = (HttpWebRequest)WebRequest.Create(url);
            requestGetPrinterEntry.ContentType = "application/json; charset=utf-8";
            requestGetPrinterEntry.Method = "POST";
            using (var streamWriter = new StreamWriter(requestGetPrinterEntry.GetRequestStream()))
            {
                streamWriter.Write(json);
            }
            requestGetPrinterEntry.Credentials = new NetworkCredential(_login,
                _pass, _domain);

            return requestGetPrinterEntry;
        }

        private string _getApiResponse(HttpWebRequest request)
        {
            using (var response = request.GetResponse())
            {
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        private async Task<string> _getApiResponseAsync(HttpWebRequest request)
        {
            using (var response = await request.GetResponseAsync())
            {
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        public TypeWorDB[] GetTypesWork()
        {
            string url = "http://test.api.unitgroup.ru/data/ServiceMobile/GetPlanActionTypeList";
            string responseStr = _getApiResponse(_getApiRequest(url));
            TypeWorDB[] typesWork = JsonConvert.DeserializeObject<TypeWorDB[]>(responseStr);
            return typesWork;
        }

        public PrinterEntryDB GetPrinterEntry(string serialKey)
        {
            string urlApi = "https://test.api.unitgroup.ru/data/Device/GetInfo?serialNum={0}";
            string urlSerialKey = string.Format(urlApi, serialKey);

            string responseStr = _getApiResponse(_getApiRequest(urlSerialKey));

            PrinterEntryDB deviceInfoResult = JsonConvert.DeserializeObject<PrinterEntryDB>(responseStr);
            if (deviceInfoResult.DeviceSerialNum == null)
                throw new WebException("Incorrect serial number device");
            return deviceInfoResult;
        }

        public PrinterEntryDB[] GetAllPrinterEntry()
        {
            string urlApi = "http://test.api.unitgroup.ru/data/ServiceMobile/GetDeviceInfoList";

            string responseStr = _getApiResponse(_getApiRequest(urlApi));

            PrinterEntryDB[] deviceInfoResult = JsonConvert.DeserializeObject<PrinterEntryDB[]>(responseStr);
            return deviceInfoResult;

        }

        public AccountDB[] GetAllAccount()
        {
            string urlApi = "http://test.api.unitgroup.ru/data/ServiceMobile/GetMobileUserList";

            string responseStr = _getApiResponse(_getApiRequest(urlApi));

            AccountDB[] deviceInfoResult = JsonConvert.DeserializeObject<AccountDB[]>(responseStr);
            return deviceInfoResult;
        }

        public static byte[] GetChecksum(object obj)
        {
            var binFormatter = new BinaryFormatter();
            var mStream = new MemoryStream();
            binFormatter.Serialize(mStream, obj);
            var array = mStream.ToArray();
            var hash = MD5.Create().ComputeHash(array);
            return hash;
        }

        public async Task<PrinterEntryDB[]> GetAllPrinterEntryAsync()
        {
            string urlApi = "http://test.api.unitgroup.ru/data/ServiceMobile/GetDeviceInfoList";

            string responseStr = await _getApiResponseAsync(_getApiRequest(urlApi));
            PrinterEntryDB[] deviceInfoResult = JsonConvert.DeserializeObject<PrinterEntryDB[]>(responseStr);
            return deviceInfoResult;
        }

        public async Task<AccountDB[]> GetAllAccountAsync()
        {
            string urlApi = "http://test.api.unitgroup.ru/data/ServiceMobile/GetMobileUserList";

            string responseStr = await _getApiResponseAsync(_getApiRequest(urlApi));

            AccountDB[] deviceInfoResult = JsonConvert.DeserializeObject<AccountDB[]>(responseStr);
            return deviceInfoResult;
        }

        public string SavePrinterEntry(DispatchDB dispatch)
        {
            string urlApi = "http://test.api.unitgroup.ru/data/ServiceMobile/SavePlanServiceIssue";

            string jsonStr = JsonConvert.SerializeObject(dispatch);

            string responseStr = _getApiResponse(_getApiRequest(urlApi, jsonStr));

            var id = JsonConvert.DeserializeAnonymousType(responseStr, new { id = string.Empty });
            if (id.id == null)
            {
                var errorMessage = JsonConvert.DeserializeAnonymousType(
                    responseStr, new { errorMessage = string.Empty });
                throw new WebException(errorMessage.errorMessage);
            }

            return id.id;
        }
    }
}