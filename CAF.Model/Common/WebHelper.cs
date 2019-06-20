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
    /// <summary>
    /// 该类是继承了cefsharp的cookies获取接口
    /// 在初始化爬虫的时候我们需要获取当前登录用户的cookie值
    /// 使用方式可参考cefsharp的官方文档
    /// </summary>
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
        /// <summary>
        /// 获取某给定的uri的cookies值并以返回一串字符
        /// 一般用在自定义构建request header时候的cookies条目
        /// </summary>
        /// <param name="uri">要获取cookie的uri</param>
        /// <returns></returns>
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

        /// <summary>
        /// 获取给定uri的cookies值并以一个collection的形式返回，
        /// 多用以HttpClient类的初始化
        /// </summary>
        /// <param name="uri">要获取cookie的uri</param>
        /// <returns></returns>
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

        /// <summary>
        /// 由于HttpClient类的get请求需要自己生成完整的请求url
        /// 所以我们封装一个使用基础url和get请求参数构造完整url的函数
        /// </summary>
        /// <param name="rawUrl">基础url</param>
        /// <param name="getParameters">get请求的参数</param>
        /// <returns></returns>
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
