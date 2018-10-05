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

namespace CAF.Model.Fetch.Rule
{
    class HuaWeiRule : IDataFetcher
    {
        HuaWeiWebClient huaweiClient = new HuaWeiWebClient();

        public async Task InitFetcherAsync()
        {
            huaweiClient.Init();

            //huaweiClient.UpdateCSRFToken();
        }

        public async Task<string> FetchContactsJsonAsync()
        {
            EventManager.runJSEventManager.RaiseEvent();

            try
            {
                string homeUrl = "https://cloud.huawei.com/home";
                string re = await huaweiClient.client.GetStringAsync(homeUrl);
                string pattern = @"uid='(\S+)'";
                string uid = Regex.Match(re, pattern).Result("$1");
                huaweiClient.client.DefaultRequestHeaders.Add("uid", uid);
            }
            catch(Exception)
            {
                throw new Exception("通讯录获取出错，请尝试重新获取数据，请检查登陆是否失效");
            }

            while (!WebHelper.GetTraceIDDone)
            {
                Thread.Sleep(100);
            }
            WebHelper.GetTraceIDDone = false;

            string url = "https://cloud.huawei.com/contact/getAllContactsProtoBuf";

            var values = new Dictionary<string, string>
            {
                {"softDel", "0"},
                { "traceId", WebHelper.TraceID}
            };
            var json = JsonConvert.SerializeObject(values);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            byte[] contactsJson = new byte[0];
            for (int i = 0; i < 10; ++i)
            {
                HttpResponseMessage response = await huaweiClient.client.PostAsync(url, content);
                contactsJson = await response.Content.ReadAsByteArrayAsync();
                if (contactsJson.Length != 0)
                    break;
            }

            huaweiClient.client.DefaultRequestHeaders.Remove("uid");

            if (contactsJson.Length == 0)
                throw new Exception("通讯录暂时无法获取");

            return Convert.ToBase64String(contactsJson);
        }

        public async Task<string> FetchCallRecordJsonAsync(UInt32 pageIndex)
        {
            try
            {
                string callPageUrl = "https://cloud.huawei.com/v1/call";
                string re = await huaweiClient.client.GetStringAsync(callPageUrl);
                string pattern = @"CSRFToken = ""(\S+)""";
                string CSRFToken = Regex.Match(re, pattern).Result("$1");
                huaweiClient.UpdateCSRFToken(CSRFToken);
            }
            catch(Exception)
            {
                throw new Exception("通话记录获取出错，请尝试重新获取数据，请检查登陆是否失效");
            }

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

            HttpResponseMessage response = await huaweiClient.client.PostAsync(url, content);
            string contactsJson = await response.Content.ReadAsStringAsync();

            return contactsJson;
        }

        public async Task<string> FetchMessageJsonAsync(UInt32 pageNum)
        {
            try
            {
                string messagePageUrl = "https://cloud.huawei.com/v1/message";
                string re = await huaweiClient.client.GetStringAsync(messagePageUrl);
                string pattern = @"CSRFToken = ""(\S+)""";
                string CSRFToken = Regex.Match(re, pattern).Result("$1");
                huaweiClient.UpdateCSRFToken(CSRFToken);
            }
            catch (Exception)
            {
                throw new Exception("短信获取出错，请尝试重新获取数据，请检查登陆是否失效");
            }

            string currentTimeStamp = TimeConverter.GetTimeStamp();
            string url = String.Format("https://cloud.huawei.com/v1/message/getAllMessage.action?dt={0}", currentTimeStamp);

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("pageSize", "256"),
                new KeyValuePair<string, string>("direction", "0"),
                new KeyValuePair<string, string>("pageNum", pageNum.ToString()),
                new KeyValuePair<string, string>("deviceId", ""),
            });

            HttpResponseMessage response = await huaweiClient.client.PostAsync(url, content);
            string contactsJson = await response.Content.ReadAsStringAsync();

            return contactsJson;
        }
    }
}
