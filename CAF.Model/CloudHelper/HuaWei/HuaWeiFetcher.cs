using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using CAF.Model.Common;
using System.Text.RegularExpressions;
using System.Threading;
using Newtonsoft.Json;
using System.IO;
using System.Net;

namespace CAF.Model.CloudHelper.HuaWei
{
    partial class HuaWeiHelper
    {
        public static HashSet<string> pictureInfo = new HashSet<string>();
        public static bool getPictureInfoDone { get; set; } = false;

        HttpClient client;

        void UpdateCSRFToken(string token)
        {
            client.DefaultRequestHeaders.Remove("CSRFToken");
            client.DefaultRequestHeaders.Add("CSRFToken", token);
        }

        void InitFetcher()
        {
            string uri = "https://cloud.huawei.com";
            CookieCollection cookies = WebHelper.GetCookies(uri);

            HttpClientHandler handler = new HttpClientHandler()
            {
                UseCookies = true,
                AutomaticDecompression = DecompressionMethods.GZip
            };
            handler.CookieContainer.Add(new Uri(uri), cookies);

            // 每次网络请求的最长等待时间
            client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromMilliseconds(50000)
            };

            client.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript, */*; q=0.01");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
            client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("Pragma", "no-cache");
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.84 Safari/537.36");
            client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");

            if (cookies["CSRFToken"] != null)
            {
                UpdateCSRFToken(cookies["CSRFToken"].Value);
            }
        }

        /// <summary>
        /// 华为云服务的特殊参数，可从网页源码中找到
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        string GetTraceId(string e)
        {
            string t = "";
            Random ra=new Random(2);
            for (int n = 0; n < 8; n++) {
                t += ra.Next(1, 9);
            }
            return e + "_02_" + TimeConverter.GetTimeStamp().Substring(0, 10) + "_" + t;
        }

        async Task<List<string>> FetchContactAsync()
        {
            List<string> re = new List<string>();

            string url = "https://cloud.huawei.com/contact/getAllGroupsProtobuf";

            string currentTimeStamp = TimeConverter.GetTimeStamp();
            var values = new Dictionary<string, string>
            {
                { "traceId", GetTraceId("03111") },
                { "currentDate",  currentTimeStamp }
            };

            byte[] data = new byte[0];

            var content = new StringContent("", Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(WebHelper.MakeGetUrl(url, values), content);
            data = await response.Content.ReadAsByteArrayAsync();

            re.Add(Convert.ToBase64String(data));

            if (data.Length == 0)
                throw new Exception("通讯录暂时无法获取");

            url = "https://cloud.huawei.com/contact/queryContactsByPage";

            values = new Dictionary<string, string>
            {
                {"softDel", "0"},
                { "traceId", GetTraceId("03111") }
            };
            var json = JsonConvert.SerializeObject(values);
            content = new StringContent(json, Encoding.UTF8, "application/json");

            data = new byte[0];
            response = await client.PostAsync(url, content);
            data = await response.Content.ReadAsByteArrayAsync();

            if (data.Length == 0)
                throw new Exception("通讯录暂时无法获取");

            re.Add(Convert.ToBase64String(data));

            return re;
        }

        async Task<List<CallRecord>> FetchCallRecordAsync()
        {
            List<CallRecord> callRecords = new List<CallRecord>();

            // 首先初始化CSRFToken值
            try
            {
                string callPageUrl = "https://cloud.huawei.com/v1/call";
                string re = await client.GetStringAsync(callPageUrl);
                string pattern = @"CSRFToken = ""(\S+)""";
                string CSRFToken = Regex.Match(re, pattern).Result("$1");
                UpdateCSRFToken(CSRFToken);
            }
            catch(Exception)
            {
                throw new Exception("通话记录获取出错，请尝试重新获取数据，请检查登陆是否失效");
            }

            uint pageIndex = 0;
            while (!runtimeData.isEnd)
            {
                string currentTimeStamp = TimeConverter.GetTimeStamp();
                string url = String.Format("https://cloud.huawei.com/v1/call/getCallRecord.action?dt={0}", currentTimeStamp);

                var content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("pageSize", ""),
                new KeyValuePair<string, string>("direction", "0"),
                new KeyValuePair<string, string>("pageIndex", pageIndex.ToString()),
                new KeyValuePair<string, string>("deviceId", ""),
                new KeyValuePair<string, string>("sortMode", "0"),
                });
                pageIndex += 1;

                HttpResponseMessage response = await client.PostAsync(url, content);
                string data = await response.Content.ReadAsStringAsync();

                // CSRF必须每次都更新
                var token = response.Headers.GetValues("CSRFToken");
                UpdateCSRFToken(token.First());

                // 解析当前获取到的数据
                callRecords.AddRange(ParseCallRecord(data));
            }

            return callRecords;
        }

        async Task<List<Message>> FetchMessageAsync()
        {
            List<Message> messages = new List<Message>();
            try
            {
                string messagePageUrl = "https://cloud.huawei.com/v1/message";
                string re = await client.GetStringAsync(messagePageUrl);
                string pattern = @"CSRFToken = ""(\S+)""";
                string CSRFToken = Regex.Match(re, pattern).Result("$1");
                UpdateCSRFToken(CSRFToken);
            }
            catch (Exception e)
            {
                throw new Exception("短信获取出错，请尝试重新获取数据，请检查登陆是否失效");
            }

            uint pageNum = 1;
            while (!runtimeData.isEnd)
            {
                string currentTimeStamp = TimeConverter.GetTimeStamp();
                string url = String.Format("https://cloud.huawei.com/v1/message/getAllMessage.action?dt={0}", currentTimeStamp);

                var content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("pageSize", "256"),
                new KeyValuePair<string, string>("direction", "0"),
                new KeyValuePair<string, string>("pageNum", pageNum.ToString()),
                new KeyValuePair<string, string>("deviceId", ""),
                });
                pageNum += 1;

                HttpResponseMessage response = await client.PostAsync(url, content);
                string data = await response.Content.ReadAsStringAsync();
                messages.AddRange(ParseMessage(data));

                var token = response.Headers.GetValues("CSRFToken");
                UpdateCSRFToken(token.First());

                if (pageNum * 256 >= runtimeData.totalCount)
                    runtimeData.isEnd = true;
            }

            return messages;
        }

        async Task<List<string>> FetchNoteAsync()
        {
            List<string> data = new List<string>();

            string url = "https://cloud.huawei.com/notepad/notetag/query";

            var values = new Dictionary<string, string>
            {
                {"guids", ""},
                {"index", "0"},
                { "traceId", GetTraceId("08105") }
            };
            var json = JsonConvert.SerializeObject(values);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(url, content);
            data.Add(await response.Content.ReadAsStringAsync());

            url = "https://cloud.huawei.com/notepad/simplenote/query";
 
            values = new Dictionary<string, string>
            {
                {"guids", ""},
                {"index", "0"},
                { "traceId", GetTraceId("08101")}
            };
            json = JsonConvert.SerializeObject(values);
            content = new StringContent(json, Encoding.UTF8, "application/json");

            response = await client.PostAsync(url, content);
            data.Add(await response.Content.ReadAsStringAsync());

            return data;
        }

        async Task<string> FetchRecordAsync()
        {
            string url = string.Format("https://cloud.huawei.com/nspservice/getDefaultRecordingDevice?traceId={0}", GetTraceId("09101"));

            var content = new StringContent("", Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(url, content);
            string data = await response.Content.ReadAsStringAsync();

            return data;
        }
    }
}
