using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Model.Common
{
    public enum ServiceProvider
    {
        Invalid,
        HuaWei,
        XiaoMi
    };

    /// <summary>
    /// 在调用任何接口之前先设置
    /// </summary>
    public class Setting
    {
        public static ServiceProvider Provider
        {
            get; set;
        } = ServiceProvider.Invalid;

        public static string LogFile
        {
            get; set;
        } = "";
    }

    public class RunJSEventManager
    {
        public delegate void RunJSEventHandler(object sender, System.EventArgs e);
        public event RunJSEventHandler RunJSEvent = delegate { };

        public void RaiseEvent()
        {
            EventArgs e = new EventArgs();
            RunJSEvent?.Invoke(this, e);
        }
    }

    public class EventManager
    { 
        public static RunJSEventManager runJSEventManager = new RunJSEventManager();
    }
}
