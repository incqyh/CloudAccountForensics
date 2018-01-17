using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Model.Fetch.Rule
{
    class XiaoMiWebClient
    {
        HttpClientHandler handler = new HttpClientHandler() { UseCookies = false, AutomaticDecompression = DecompressionMethods.GZip };
        public HttpClient client;
        string cookies;
        public string Cookies { get { return cookies; } set { cookies = value; } }

        public XiaoMiWebClient()
        {
            client = new HttpClient(handler);// { BaseAddress = baseAddress };
            client.Timeout = TimeSpan.FromMilliseconds(30000); 

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

        public void SetCookies()
        {
            string uri = "https://i.mi.com";
            StringBuilder cookieBuilder = new StringBuilder();
            cookieBuilder.Append(WebHelper.GetCookieHeader(uri));
            uri = "https://mi.com";
            cookieBuilder.Append(WebHelper.GetCookieHeader(uri));

            cookies = cookieBuilder.ToString();
            client.DefaultRequestHeaders.Remove("Cookie");
            client.DefaultRequestHeaders.Add("Cookie", cookies);
        }
    }
}
