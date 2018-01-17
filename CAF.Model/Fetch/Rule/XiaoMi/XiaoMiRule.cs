using CAF.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CAF.Model.Fetch.Rule
{
    class XiaoMiRule : IDataFetcher
    {
        XiaoMiWebClient xiaomiClient = new XiaoMiWebClient();
        string uuid;

        public async Task InitFetcherAsync()
        {
            xiaomiClient.SetCookies();
            string pattern = @"userId=([0-9]+)";
            uuid = Regex.Match(xiaomiClient.Cookies, pattern).Result("$1");
        }

        public async Task<string> FetchCallRecordJsonAsync(uint pageIndex = 0)
        {
            throw new NotImplementedException();
        }

        public async Task<string> FetchContactsJsonAsync()
        {
            string currentTimeStamp = TimeConverter.GetTimeStamp();

            Dictionary<string, string> content = new Dictionary<string, string>
            {
                { "_dc", currentTimeStamp },
                {"syncTag", "0" },
                {"limit", "400" },
                {"syncIgnoreTag", "0" },
                {"uuid", uuid },
            };

            string url = WebHelper.MakeGetUrl(String.Format("https://i.mi.com/contacts/{0}/initdata", uuid), content);

            HttpRequestMessage request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri(url.ToString());

            HttpResponseMessage response = await xiaomiClient.client.SendAsync(request);
            string contactsJson = await response.Content.ReadAsStringAsync();

            return contactsJson;
        }

        public async Task<string> FetchMessageJsonAsync(uint pageNum = 1)
        {
            string currentTimeStamp = TimeConverter.GetTimeStamp();
            Dictionary<string, string> content = new Dictionary<string, string>
            {
                { "_dc", currentTimeStamp },
                {"syncTag", "0" },
                {"limit", "20" },
                {"readMode", "older" },
                {"withPhoneCall", "true" },
                {"uuid", uuid },
            };

            string url = WebHelper.MakeGetUrl(String.Format("https://i.mi.com/sms/{0}/full/thread", uuid), content);

            HttpRequestMessage request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri(url.ToString());

            HttpResponseMessage response = await xiaomiClient.client.SendAsync(request);
            string contactsJson = await response.Content.ReadAsStringAsync();

            return contactsJson;
        }
    }
}
