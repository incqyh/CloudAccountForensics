using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using CefSharp;

namespace CAF.Model.Common
{
    class CookieCollector : ICookieVisitor
    {
        private readonly TaskCompletionSource<List<Cookie>> _source = new TaskCompletionSource<List<Cookie>>();

        public bool Visit(Cookie cookie, int count, int total, ref bool deleteCookie)
        {
            _cookies.Add(cookie);

            if (count == (total - 1))
            {
                _source.SetResult(_cookies);
            }

            return true;
        }

        public Task<List<Cookie>> Task => _source.Task;

        public static System.Net.CookieCollection GetCookies(string uri, List<Cookie> cookies)
        {
            System.Net.CookieCollection cc = new System.Net.CookieCollection();
            foreach (var cookie in cookies)
                cc.Add(new System.Net.Cookie(cookie.Name, cookie.Value));
            return cc;
        }

        public static string GetCookieHeader(List<Cookie> cookies)
        {

            StringBuilder cookieString = new StringBuilder();
            string delimiter = string.Empty;

            foreach (var cookie in cookies)
            {
                cookieString.Append(delimiter);
                cookieString.Append(cookie.Name);
                cookieString.Append('=');
                cookieString.Append(cookie.Value);
                delimiter = "; ";
            }

            return cookieString.ToString();
        }

        private readonly List<Cookie> _cookies = new List<Cookie>();
        public void Dispose()
        {
        }
    }

    public class WebHelper
    {
        public static List<string> HuaWeiPicture = new List<string>();
        public static bool GetPictureDone { get; set; } = false;
        public static string TraceID { get; set; } = null;
        public static bool GetTraceIDDone { get; set; } = false;

        public static string GetCookieHeader(string uri)
        {
            var cookieManager = Cef.GetGlobalCookieManager();
            var visitor = new CookieCollector();

            cookieManager.VisitUrlCookies(uri, true, visitor);

            var t = visitor.Task;
            t.Wait(1000);
            var cookieHeader = CookieCollector.GetCookieHeader(t.Result);

            return cookieHeader;
        }

        public static System.Net.CookieCollection GetCookies(string uri)
        {
            var cookieManager = Cef.GetGlobalCookieManager();
            var visitor = new CookieCollector();

            cookieManager.VisitUrlCookies(uri, true, visitor);

            var t = visitor.Task;
            bool isGet = t.Wait(1000);

            if (!isGet)
                throw new Exception("No cookies got, please check");
            return CookieCollector.GetCookies(uri, t.Result);
        }

        public static string MakeGetUrl(string rawUrl, Dictionary<string, string> getParameters)
        {
            StringBuilder url = new StringBuilder();
            url.Append(rawUrl+"?");
            foreach (var i in getParameters)
            {
                url.Append(i.Key);
                url.Append("=");
                url.Append(i.Value);
                url.Append("&");
            }
            url.Remove(url.Length - 1, 1);

            return url.ToString();
        }
    }
}
