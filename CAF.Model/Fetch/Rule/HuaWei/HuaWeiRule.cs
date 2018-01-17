using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using CAF.Model.Common;
using System.Text.RegularExpressions;

namespace CAF.Model.Fetch.Rule
{
    class HuaWeiRule : IDataFetcher
    {
        HuaWeiWebClient huaweiClient = new HuaWeiWebClient();

        public async Task InitFetcherAsync()
        {
            huaweiClient.SetCookies();

            huaweiClient.UpdateCSRFToken();
        }

        public async Task<string> FetchCallRecordJsonAsync(UInt32 pageIndex)
        {
            string callPageUrl = "https://cloud.huawei.com/call";

            string re = await huaweiClient.client.GetStringAsync(callPageUrl);
            string pattern = @"CSRFToken = ""(\S+)""";
            string CSRFToken = Regex.Match(re, pattern).Result("$1");
            huaweiClient.UpdateCSRFToken(CSRFToken);

            string currentTimeStamp = TimeConverter.GetTimeStamp();
            string url = String.Format("https://cloud.huawei.com/call/getCallRecord.action?dt={0}", currentTimeStamp);

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

            var token = response.Headers.GetValues("CSRFToken");
            huaweiClient.UpdateCSRFToken(token.First());

            return contactsJson;
        }

        public async Task<string> FetchContactsJsonAsync()
        {
            string currentTimeStamp = TimeConverter.GetTimeStamp();
            string url = String.Format("https://www.hicloud.com/contact/getAllContacts.action?{0}", currentTimeStamp);

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("SoftDel", "0"),
            });

            HttpResponseMessage response = await huaweiClient.client.PostAsync(url, content);
            string contactsJson = await response.Content.ReadAsStringAsync();

            huaweiClient.UpdateCSRFToken();

            return contactsJson;
        }

        public async Task<string> FetchMessageJsonAsync(UInt32 pageNum)
        {
            string messagePageUrl = "https://cloud.huawei.com/message";

            string re = await huaweiClient.client.GetStringAsync(messagePageUrl);
            string pattern = @"CSRFToken = ""(\S+)""";
            string CSRFToken = Regex.Match(re, pattern).Result("$1");
            huaweiClient.UpdateCSRFToken(CSRFToken);

            string currentTimeStamp = TimeConverter.GetTimeStamp();
            string url = String.Format("https://cloud.huawei.com/message/getAllMessage.action?dt={0}", currentTimeStamp);

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("pageSize", "256"),
                new KeyValuePair<string, string>("direction", "0"),
                new KeyValuePair<string, string>("pageNum", pageNum.ToString()),
                new KeyValuePair<string, string>("deviceId", ""),
            });

            HttpResponseMessage response = await huaweiClient.client.PostAsync(url, content);
            string contactsJson = await response.Content.ReadAsStringAsync();

            var token = response.Headers.GetValues("CSRFToken");
            huaweiClient.UpdateCSRFToken(token.First());
            return contactsJson;
        }
    }
}
