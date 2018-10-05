using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using CefSharp;

namespace CAF.Model.Fetch.Rule
{
    /// <summary>
    /// CefSharp的cookies获取接口
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
        public static string TraceID { get; set; } = null;
        public static bool GetTraceIDDone { get; set; } = false;
        /// <summary>
        /// 使用CefSharp时候的cookies获取函数
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string GetCookieHeader(string uri)
        {
            var cookieManager = Cef.GetGlobalCookieManager();
            var visitor = new CookieCollector();

            cookieManager.VisitUrlCookies(uri, true, visitor);

            var t = visitor.Task;
            t.Wait();
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
                throw new Exception("请先登录");
            var cookieHeader = CookieCollector.GetCookies(uri, t.Result);

            return cookieHeader;
        }


        /// <summary>
        /// httpclient没有提供get的数据与url分离的接口，所以自己实现一下构造完整的url
        /// </summary>
        /// <param name="rawUrl"></parD:\Documents\Files\workspace\CloudAccountForensics\CAF.Model\Fetch\Rule\WebHelper.csam>
        /// <param name="getParameters"></param>
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

     //    public static List<Cookie> GetAllCookies(CookieContainer cc)
     //    {
     //        List<Cookie> lstCookies = new List<Cookie>();

     //        Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable",
     //            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
     //            System.Reflection.BindingFlags.Instance, null, cc, new object[] { });

     //        foreach (object pathList in table.Values)
     //        {
     //            SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
     //                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
     //                | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
     //            foreach (CookieCollection colCookies in lstCookieCol.Values)
     //                foreach (Cookie c in colCookies) lstCookies.Add(c);
     //        }

     //        return lstCookies;
     //    }

     //    [DllImport("wininet.dll", SetLastError = true)]
     //    public static extern bool InternetGetCookieEx(
     //            string url,
     //            string cookieName,
     //            StringBuilder cookieData,
     //            ref int size,
     //            Int32 dwFlags,
     //            IntPtr lpReserved);

     //    private const Int32 InternetCookieHttponly = 0x2000;

     //    /// <summary>
     //    /// Gets the URI cookie container.
     //    /// </summary>
     //    /// <param name="uri">The URI.</param>
     //    /// <returns></returns>
     //    public static CookieContainer GetUriCookieContainer(Uri uri)
     //    {
     //        CookieContainer cookies = null;
     //        // Determine the size of the cookie
     //        int datasize = 8192 * 16;
     //        StringBuilder cookieData = new StringBuilder(datasize);
     //        if (!InternetGetCookieEx(uri.ToString(), null, cookieData, ref datasize, InternetCookieHttponly, IntPtr.Zero))
     //        {
     //            if (datasize < 0)
     //                return null;
     //            // Allocate stringbuilder large enough to hold the cookie
     //            cookieData = new StringBuilder(datasize);
     //            if (!InternetGetCookieEx(
     //                uri.ToString(),
     //                null, cookieData,
     //                ref datasize,
     //                InternetCookieHttponly,
     //                IntPtr.Zero))
     //                return null;
     //        }
     //        if (cookieData.Length > 0)
     //        {
     //            cookies = new CookieContainer();
     //            cookies.SetCookies(uri, cookieData.ToString().Replace(';', ','));
     //        }
     //        return cookies;
     //    }

     //    /// <summary>
     //    /// 直接将cookies按照string的格式输出，用于生成请求头
     //    /// </summary>
     //    /// <param name="uri"></param>
     //    /// <returns></returns>
     //    public static string GetUriCookies(string uri)
     //    {
     //        // Determine the size of the cookie
     //        int datasize = 8192 * 16;
     //        StringBuilder cookieData = new StringBuilder(datasize);
     //        if (!InternetGetCookieEx(uri, null, cookieData, ref datasize, InternetCookieHttponly, IntPtr.Zero))
     //        {
     //            if (datasize < 0)
     //                return null;
     //            // Allocate stringbuilder large enough to hold the cookie
     //            cookieData = new StringBuilder(datasize);
     //            if (!InternetGetCookieEx(
     //                uri,
     //                null, cookieData,
     //                ref datasize,
     //                InternetCookieHttponly,
     //                IntPtr.Zero))
     //                return null;
     //        }
     //        return cookieData.ToString();
     //    }
    }
}
