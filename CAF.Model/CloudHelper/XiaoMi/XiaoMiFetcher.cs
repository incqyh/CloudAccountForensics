using CAF.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CAF.Model.CloudHelper.XiaoMi
{
    partial class XiaoMiHelper
    {
        string uuid;
        public HttpClient client;

        public void InitFetcher()
        {
            string uri = "https://i.mi.com";
            CookieCollection cookies = WebHelper.GetCookies(uri);
            uuid = cookies["userId"].Value;

            HttpClientHandler handler = new HttpClientHandler()
            {
                UseCookies = true,
                AutomaticDecompression = DecompressionMethods.GZip
            };
            handler.CookieContainer.Add(new Uri(uri), cookies);

            client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromMilliseconds(30000)
            };

            client.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript, */*; q=0.01");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
            client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("Host", "i.mi.com");
            client.DefaultRequestHeaders.Add("Pragma", "no-cache");
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.84 Safari/537.36");
            client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
        }

        public async Task<string> FetchCallRecordAsync()
        {
            string currentTimeStamp = TimeConverter.GetTimeStamp();

            Dictionary<string, string> content = new Dictionary<string, string>
            {
                { "_dc", currentTimeStamp },
                {"isDeletedView", "false" },
                {"readMode", "older" },
                {"limit", "100" },
                {"syncTag", runtimeData.syncTag },
                {"uuid", uuid },
            };

            string url = WebHelper.MakeGetUrl(String.Format("https://i.mi.com/phonecall/{0}/full", uuid), content);

            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url.ToString())
            };

            HttpResponseMessage response = await client.SendAsync(request);
            string data = await response.Content.ReadAsStringAsync();

            return data;
        }

        public async Task<string> FetchContactsAsync()
        {
            string currentTimeStamp = TimeConverter.GetTimeStamp();

            Dictionary<string, string> content = new Dictionary<string, string>
            {
                { "_dc", currentTimeStamp },
                {"syncTag", runtimeData.syncTag },
                {"limit", "400" },
                {"syncIgnoreTag", runtimeData.syncIgnoreTag },
                {"uuid", uuid },
            };

            string url = WebHelper.MakeGetUrl(String.Format("https://i.mi.com/contacts/{0}/initdata", uuid), content);

            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url.ToString())
            };

            HttpResponseMessage response = await client.SendAsync(request);
            string data = await response.Content.ReadAsStringAsync();

            return data;
        }

        public async Task<string> FetchMessageAsync()
        {
            string currentTimeStamp = TimeConverter.GetTimeStamp();
            Dictionary<string, string> content = new Dictionary<string, string>
            {
                { "_dc", currentTimeStamp },
                {"syncTag", runtimeData.syncTag },
                {"limit", "100000" },
                {"readMode", "older" },
                {"withPhoneCall", "true" },
                {"uuid", uuid },
            };

            string url = WebHelper.MakeGetUrl(String.Format("https://i.mi.com/sms/{0}/full/thread", uuid), content);

            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url.ToString())
            };

            HttpResponseMessage response = await client.SendAsync(request);
            string data = await response.Content.ReadAsStringAsync();

            return data;
        }
    }
}
