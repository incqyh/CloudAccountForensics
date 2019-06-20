using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CAF.Model.CloudHelper.HuaWei;
using CAF.Model.Common;
using CAF.View.Common;
// using System.Windows.Forms;

using CefSharp;
using CefSharp.Handler;

namespace CAF.View.Pages
{
    public class MemoryStreamResponseFilter : IResponseFilter
    {
        private MemoryStream memoryStream;

        bool IResponseFilter.InitFilter()
        {
            //NOTE: We could initialize this earlier, just one possible use of InitFilter
            memoryStream = new MemoryStream();

            return true;
        }

        FilterStatus IResponseFilter.Filter(Stream dataIn, out long dataInRead, Stream dataOut, out long dataOutWritten)
        {
            if (dataIn == null)
            {
                dataInRead = 0;
                dataOutWritten = 0;

                return FilterStatus.Done;
            }

            dataInRead = dataIn.Length;
            dataOutWritten = Math.Min(dataInRead, dataOut.Length);

            if(dataIn.Length > dataOut.Length)
            {
                var data = new byte[dataOut.Length];
                dataIn.Seek(0, SeekOrigin.Begin);
                dataIn.Read(data, 0, data.Length);
                dataOut.Write(data, 0, data.Length);
            
                dataInRead = dataOut.Length;
                dataOutWritten = dataOut.Length;
                return FilterStatus.NeedMoreData;
            }

            dataIn.CopyTo(dataOut);

            //Copy data to stream
            dataIn.Position = 0;
            dataIn.CopyTo(memoryStream);

            return FilterStatus.Done;
        }

        void IDisposable.Dispose()
        {
            memoryStream.Dispose();
            memoryStream = null;
        }

        public byte[] Data
        {
            get { return memoryStream.ToArray(); }
        }
    }

    public class RequestHandler : DefaultRequestHandler
    {
        private Dictionary<ulong, MemoryStreamResponseFilter> responseDictionary = new Dictionary<ulong, MemoryStreamResponseFilter>();
        public override IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            var dataFilter = new MemoryStreamResponseFilter();
            responseDictionary.Add(request.Identifier, dataFilter);
            return dataFilter;
        }
        public override void OnResourceLoadComplete(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
        {      
            MemoryStreamResponseFilter filter;
            if (responseDictionary.TryGetValue(request.Identifier, out filter))
            {
                if (request.Url == "https://cloud.huawei.com/album/getSingleUrl")
                {
                    var data = filter.Data;
                    HuaWeiHelper.pictureInfo.Add(Encoding.UTF8.GetString(data));
                    HuaWeiHelper.getPictureInfoDone = true;
                }
            }
        }
    }

    /// <summary>
    /// Browser.xaml 的交互逻辑
    /// </summary>
    public partial class CefBrowser : Page
    {
        public CefBrowser()
        {
            InitializeComponent();

            Browser.RequestHandler = new RequestHandler();

            if (Setting.Provider == ServiceProvider.HuaWei)
                Browser.LoadingStateChanged += BrowserLoadingStateChanged;
        }

        private void BrowserLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (!e.IsLoading)
            {
                Browser.LoadingStateChanged -= BrowserLoadingStateChanged;
            }
        }

        public void SwitchToPicture()
        {
            HuaWeiHelper.getPictureInfoDone = false;
            if (Browser.Address == "https://cloud.huawei.com/home#/album/photoList")
                Browser.Reload();
            else
                Browser.Address = "https://cloud.huawei.com/home#/album/photoList";
        }

        public void SwitchWebsite()
        {
            Browser.Address = Setting.MainUrl[Setting.Provider];
        }

        private async Task<string> RunJS(string script)
        {
            string EvaluateJavaScriptResult = null;

            try
            {
                var r = await Browser.EvaluateScriptAsync(script);
                JavascriptResponse response = r;
                EvaluateJavaScriptResult = response.Success ? (response.Result.ToString() ?? "null") : response.Message.ToString();
            }
            catch
            {
                EvaluateJavaScriptResult = "";
            }
            return EvaluateJavaScriptResult;
        }
    }
}
