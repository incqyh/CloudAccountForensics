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
using CAF.Model.Common;
// using System.Windows.Forms;

using CefSharp;

namespace CAF.View.Pages
{
    /// <summary>
    /// Browser.xaml 的交互逻辑
    /// </summary>
    public partial class CefBrowser : Page
    {
        public CefBrowser()
        {
            InitializeComponent();

            var tmp = new RunJSEventManager.RunJSEventHandler(RunJS);
            Model.Common.EventManager.runJSEventManager.RunJSEvent += tmp;
        }

        public void SwitchWebsite()
        {
            Browser.Address = Setting.MainUrl[Setting.Provider];
        }

        private void RunJS(object sender, EventArgs e)
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
                WebHelper.TraceID = EvaluateJavaScriptResult;
                WebHelper.GetTraceIDDone = true;
            });
        }
    }
}
