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

            var tmp = new RunJSEventManager.RunJSEventHandler(GetTraceID);
            Model.Common.EventManager.runJSEventManager.RunJSEvent += tmp;
        }

        public void SwitchWebsite()
        {
            Browser.Address = Setting.MainUrl[Setting.Provider];
        }

        private void GetTraceID(object sender, EventArgs e)
        {
            string script = "(function() { var trace_id = getTraceId(\"03111\"); return trace_id ; })();";
            RunJS(script);
        }

        private void RunJS(string script)
        {
            var task = Browser.EvaluateScriptAsync(script);

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
