using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CAF.Model.Fetch.Rule
{
    class HuaWeiWebClient
    {
        HttpClientHandler handler;
        public HttpClient client;
        //string cookies;
        string CSRFToken;

        public HuaWeiWebClient()
        {

        }

        public void Init()
        {
            // string uri = "https://cloud.huawei.com";
            // handler.CookieContainer.Add(new Uri(uri), WebHelper.GetCookies(uri));

            string uri = "https://cloud.huawei.com";
            //cookies = WebHelper.GetCookieHeader(uri);
            CookieCollection cookies = WebHelper.GetCookies(uri);

            //if (!cookies.Contains("_pk_ref") && !cookies.Contains("verFlag"))
            //    cookies += "; verFlag=1;";
            //client.DefaultRequestHeaders.Remove("Cookie");
            //client.DefaultRequestHeaders.Add("Cookie", cookies);

            HttpClientHandler handler = new HttpClientHandler() { UseCookies = true, AutomaticDecompression = DecompressionMethods.GZip };
            handler.CookieContainer.Add(new Uri(uri), cookies);

            client = new HttpClient(handler);// { BaseAddress = baseAddress };
            client.Timeout = TimeSpan.FromMilliseconds(30000);

            client.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript, */*; q=0.01");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
            client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("Host", "cloud.huawei.com");
            client.DefaultRequestHeaders.Add("Pragma", "no-cache");
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.84 Safari/537.36");
            client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
        }

        /// <summary>
        /// 华为服务有csrf验证，每次获取数据之后都要重置csrf令牌
        /// </summary>
        public void UpdateCSRFToken(string token = "")
        {
            if (token.Length != 0)
            {
                CSRFToken = token;
            }
            //else
            //{
            //    string pattern = @"CSRFToken=(\S+)";
            //    // CSRFToken = Regex.Match(cookies, pattern).Result("$1");
            //    string uri = "https://cloud.huawei.com";
            //    // string tmp = handler.CookieContainer.GetCookieHeader(new Uri(uri));
            //    CSRFToken = Regex.Match(cookies, pattern).Result("$1");
            //}
            client.DefaultRequestHeaders.Remove("CSRFToken");
            client.DefaultRequestHeaders.Add("CSRFToken", CSRFToken);
        }
    }
}
