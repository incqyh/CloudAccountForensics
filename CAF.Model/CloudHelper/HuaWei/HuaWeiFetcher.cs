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

            UpdateCSRFToken(cookies["CSRFToken"].Value);
        }

        string GetTraceId(string e)
        {
            string t = "";
            Random ra=new Random(2);
            for (int n = 0; n < 8; n++) {
                t += ra.Next(1, 9);
            }
            return e + "_02_" + TimeConverter.GetTimeStamp().Substring(0, 10) + "_" + t;
        }

        async Task<string> FetchContactAsync()
        {
            // EventManager.runJSEventManager.GetTraceID(new RunJSEventArgs("03111"));

            // try
            // {
            //     string homeUrl = "https://cloud.huawei.com/home";
            //     string re = await client.GetStringAsync(homeUrl);
            //     string pattern = @"uid='(\S+)'";
            //     string uid = Regex.Match(re, pattern).Result("$1");
            //     client.DefaultRequestHeaders.Add("uid", uid);
            // }
            // catch(Exception)
            // {
            //     throw new Exception("通讯录获取出错，请尝试重新获取数据，请检查登陆是否失效");
            // }

            // while (!WebHelper.GetTraceIDDone)
            // {
            //     Thread.Sleep(10);
            // }
            // WebHelper.GetTraceIDDone = false;

            string url = "https://cloud.huawei.com/contact/queryContactsByPage";

            var values = new Dictionary<string, string>
            {
                {"softDel", "0"},
                { "traceId", GetTraceId("03111") }
            };
            var json = JsonConvert.SerializeObject(values);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            byte[] data = new byte[0];
            HttpResponseMessage response = await client.PostAsync(url, content);
            data = await response.Content.ReadAsByteArrayAsync();

            // client.DefaultRequestHeaders.Remove("uid");

            if (data.Length == 0)
                throw new Exception("通讯录暂时无法获取");

            return Convert.ToBase64String(data);
        }

        async Task<List<CallRecord>> FetchCallRecordAsync()
        {
            List<CallRecord> callRecords = new List<CallRecord>();

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

                var token = response.Headers.GetValues("CSRFToken");
                UpdateCSRFToken(token.First());

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

            // EventManager.runJSEventManager.GetTraceID(new RunJSEventArgs("08105"));
            // while (!WebHelper.GetTraceIDDone)
            // {
            //     Thread.Sleep(10);
            // }
            // WebHelper.GetTraceIDDone = false;

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

            EventManager.runJSEventManager.GetTraceID(new RunJSEventArgs("08101"));
            while (!WebHelper.GetTraceIDDone)
            {
                Thread.Sleep(10);
            }
            WebHelper.GetTraceIDDone = false;
            url = "https://cloud.huawei.com/notepad/simplenote/query";
 
            values = new Dictionary<string, string>
            {
                {"guids", ""},
                {"index", "0"},
                { "traceId", WebHelper.TraceID}
            };
            json = JsonConvert.SerializeObject(values);
            content = new StringContent(json, Encoding.UTF8, "application/json");

            response = await client.PostAsync(url, content);
            data.Add(await response.Content.ReadAsStringAsync());

            return data;
        }

        async Task<string> FetchRecordAsync()
        {
            // EventManager.runJSEventManager.GetTraceID(new RunJSEventArgs("09101"));
            // while (!WebHelper.GetTraceIDDone)
            // {
            //     Thread.Sleep(10);
            // }
            // WebHelper.GetTraceIDDone = false;
            string url = string.Format("https://cloud.huawei.com/nspservice/getDefaultRecordingDevice?traceId={0}", GetTraceId("09101"));

            var content = new StringContent("", Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(url, content);
            string data = await response.Content.ReadAsStringAsync();

            return data;
        }
    }
}
