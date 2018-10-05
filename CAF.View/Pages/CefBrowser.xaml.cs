using System;
using System.Collections.Generic;
using System.Linq;
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
// using System.Windows.Forms;

using CefSharp;

namespace CAF.View.Pages
{
    /// <summary>
    /// Browser.xaml 的交互逻辑
    /// </summary>
    public partial class CefBrowser : Window
    {
        public CefBrowser()
        {
            InitializeComponent();

            var tmp = new CAF.Model.Common.RunJSEventManager.RunJSEventHandler(RunJS);
            CAF.Model.Common.EventManager.runJSEventManager.RunJSEvent += tmp;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }

        public void SwitchWebsite()
        {
            switch (CAF.Model.Common.Setting.Provider)
            {
                case Model.Common.ServiceProvider.HuaWei:
                    Browser.Address = "https://cloud.huawei.com";
                    break;
                case Model.Common.ServiceProvider.XiaoMi:
                    Browser.Address = "https://i.mi.com";
                    break;
            }
            //string url = App.Current.Properties["frame"].ToString();
            //Browser.Address = url;

            // Task.Run(() =>

            //     try
            //     {
            //         browser.Navigate(url);
            //     }
            //     catch (Exception ex)
            //     {

            //     }
            // });
        }

        private void RunJS(object sender, System.EventArgs e)
        {
            var task = Browser.EvaluateScriptAsync("(function() { var trace_id = getTraceId(\"03111\"); return trace_id ; })();");

            string EvaluateJavaScriptResult = null;
            task.ContinueWith(t =>
            {
                if (!t.IsFaulted)
                {
                    JavascriptResponse response = t.Result;
                    EvaluateJavaScriptResult = response.Success ? (response.Result.ToString() ?? "null") : response.Message.ToString();
                }
                CAF.Model.Fetch.Rule.WebHelper.TraceID = EvaluateJavaScriptResult;
                CAF.Model.Fetch.Rule.WebHelper.GetTraceIDDone = true;
            });
        }
    }
}
