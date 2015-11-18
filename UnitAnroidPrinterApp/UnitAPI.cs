using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace UnitAnroidPrinterApp
{
    abstract class UnitAPI
    {
        private readonly string _login;
        private readonly string _pass;
        private readonly string _domain;

        protected UnitAPI(string login, string pass, string domain)
        {
            _login = login;
            _pass = pass;
            _domain = domain;
            _domain = "UN1T";
            _login = "mobileUnit_Service";
            _pass = "1qazXSW@";
        }

        protected async Task<bool> CheckNeedUpdateAsync(string date)
        {
            string urlApi = "http://test.api.unitgroup.ru/data/ServiceMobile/CheckDeviceInfoListIsChanged?lastModifyDate={0}";
            urlApi = string.Format(urlApi, date);

            string responseStr = await _getApiResponseAsync(_getApiRequest(urlApi));
            var changed = new { isChanged = true };
            var needChange = JsonConvert.DeserializeAnonymousType(responseStr, changed);

            return needChange.isChanged;
        }

        protected HttpWebRequest _getApiRequest(string url)
        {
            ServicePointManager.ServerCertificateValidationCallback =
                new System.Net.Security.RemoteCertificateValidationCallback(delegate { return true; });

            HttpWebRequest requestGetPrinterEntry = (HttpWebRequest)WebRequest.Create(url);
            requestGetPrinterEntry.ContentType = "text/json";
            requestGetPrinterEntry.Credentials = new NetworkCredential(_login,
                _pass, _domain);

            return requestGetPrinterEntry;
        }

        protected HttpWebRequest _getApiRequest(string url, string json)
        {
            ServicePointManager.ServerCertificateValidationCallback =
                new System.Net.Security.RemoteCertificateValidationCallback(delegate { return true; });

            HttpWebRequest requestGetPrinterEntry = (HttpWebRequest)WebRequest.Create(url);
            requestGetPrinterEntry.ContentType = "application/json; charset=utf-8";
            requestGetPrinterEntry.Method = "POST";
            requestGetPrinterEntry.Timeout = int.MaxValue;
            using (var streamWriter = new StreamWriter(requestGetPrinterEntry.GetRequestStream()))
            {
                streamWriter.Write(json);
            }
            requestGetPrinterEntry.Credentials = new NetworkCredential(_login,
                _pass, _domain);

            return requestGetPrinterEntry;
        }

        protected string _getApiResponse(HttpWebRequest request)
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

        protected TypeWorkDB[] GetTypesWork()
        {
            string url = "http://test.api.unitgroup.ru/data/ServiceMobile/GetPlanActionTypeList";
            string responseStr = _getApiResponse(_getApiRequest(url));
            TypeWorkDB[] typesWork = JsonConvert.DeserializeObject<TypeWorkDB[]>(responseStr);
            return typesWork;
        }

        protected async Task<TypeWorkDB[]> GetTypesWorkAsync()
        {
            string url = "http://test.api.unitgroup.ru/data/ServiceMobile/GetPlanActionTypeList";
            string responseStr = await _getApiResponseAsync(_getApiRequest(url));
            TypeWorkDB[] typesWork = JsonConvert.DeserializeObject<TypeWorkDB[]>(responseStr);
            return typesWork;
        }

        protected PrinterEntryDB GetPrinterEntry(string serialKey)
        {
            string urlApi = "https://test.api.unitgroup.ru/data/Device/GetInfo?serialNum={0}";
            string urlSerialKey = string.Format(urlApi, serialKey);

            string responseStr = _getApiResponse(_getApiRequest(urlSerialKey));

            PrinterEntryDB deviceInfoResult = JsonConvert.DeserializeObject<PrinterEntryDB>(responseStr);
            if (deviceInfoResult.DeviceSerialNum == null)
                throw new WebException("Incorrect serial number device");
            return deviceInfoResult;
        }

        protected PrinterEntryDB[] GetAllPrinterEntry(string date = "")
        {
            string urlApi = "http://test.api.unitgroup.ru/data/ServiceMobile/GetDeviceInfoList?lastModifyDate={0}";
            urlApi = string.Format(urlApi, date);

            string responseStr = _getApiResponse(_getApiRequest(urlApi));

            PrinterEntryDB[] deviceInfoResult = JsonConvert.DeserializeObject<PrinterEntryDB[]>(responseStr);
            return deviceInfoResult;

        }

        protected AccountDB[] GetAllAccount()
        {
            string urlApi = "http://test.api.unitgroup.ru/data/ServiceMobile/GetMobileUserList";

            string responseStr = _getApiResponse(_getApiRequest(urlApi));

            AccountDB[] deviceInfoResult = JsonConvert.DeserializeObject<AccountDB[]>(responseStr);
            return deviceInfoResult;
        }

        protected async Task<PrinterEntryDB[]> GetAllPrinterEntryAsync(string date = "")
        {
            string urlApi = "http://test.api.unitgroup.ru/data/ServiceMobile/GetDeviceInfoList?lastModifyDate={0}";
            urlApi = string.Format(urlApi, date);

            string responseStr = await _getApiResponseAsync(_getApiRequest(urlApi));
            PrinterEntryDB[] deviceInfoResult = JsonConvert.DeserializeObject<PrinterEntryDB[]>(responseStr);
            return deviceInfoResult;
        }

        protected async Task<PrinterEntryDB[]> GetAllPrinterEntryAsync()
        {
            string urlApi = "http://test.api.unitgroup.ru/data/ServiceMobile/GetDeviceInfoList";

            string responseStr = await _getApiResponseAsync(_getApiRequest(urlApi));
            PrinterEntryDB[] deviceInfoResult = JsonConvert.DeserializeObject<PrinterEntryDB[]>(responseStr);
            return deviceInfoResult;
        }

        protected async Task<AccountDB[]> GetAllAccountAsync()
        {
            string urlApi = "http://test.api.unitgroup.ru/data/ServiceMobile/GetMobileUserList";

            string responseStr = await _getApiResponseAsync(_getApiRequest(urlApi));

            AccountDB[] deviceInfoResult = JsonConvert.DeserializeObject<AccountDB[]>(responseStr);
            return deviceInfoResult;
        }

        protected string PushPrinterEntry(DispatchDB dispatch)
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

        protected async Task<string> PushPrinterEntryAsync(DispatchDB dispatch)
        {
            string urlApi = "http://test.api.unitgroup.ru/data/ServiceMobile/SavePlanServiceIssue";

            string jsonStr = JsonConvert.SerializeObject(dispatch);

            string responseStr = await _getApiResponseAsync(_getApiRequest(urlApi, jsonStr));

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