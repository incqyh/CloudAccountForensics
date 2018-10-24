using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Model.Common
{
    public class RunJSEventArgs : EventArgs
    {
        public string script;
        public RunJSEventArgs(string script)
        {
            this.script = script;
        }
    }

    public class RunJSEventManager
    {
        public delegate void RunJSEventHandler(object sender, RunJSEventArgs e);
        public event RunJSEventHandler GetUniqueIDEvent = delegate { };
        public event RunJSEventHandler GetTraceIDEvent = delegate { };

        public void GetTraceID(RunJSEventArgs e)
        {
            GetTraceIDEvent?.Invoke(this, e);
        }

        public void GetUniqueID(RunJSEventArgs e)
        {
            GetUniqueIDEvent?.Invoke(this, e);
        }
    }

    public class GetHuaWeiPictureEventManager
    {
        public delegate void GetHuaWeiPictureEventHandler(object sender, EventArgs e);
        public event GetHuaWeiPictureEventHandler GetHuaWeiPictureEvent = delegate { };

        public void RaiseEvent()
        {
            GetHuaWeiPictureEvent?.Invoke(this, new EventArgs());
        }
    }

    public class LogOutEventManager
    {
        public delegate void LogOutEventHandler(object sender, EventArgs e);
        public event LogOutEventHandler RunJSEvent = delegate { };

        public void RaiseEvent()
        {
            EventArgs e = new EventArgs();
            RunJSEvent?.Invoke(this, e);
        }
    }

    public class DownloadPictureEventArgs : EventArgs
    {
        public Picture picture;
        public DownloadPictureEventArgs(Picture picture)
        {
            this.picture = picture;
        }
    }

    public class DownloadPictureEventManager
    {
        public delegate void DownloadPictureEventHandler(object sender, DownloadPictureEventArgs e);
        public event DownloadPictureEventHandler DownloadPictureEvent = delegate { };

        public void RaiseEvent(Picture picture)
        {
            DownloadPictureEventArgs e = new DownloadPictureEventArgs(picture);
            DownloadPictureEvent?.Invoke(this, e);
        }
    }

    public class EventManager
    { 
        public static RunJSEventManager runJSEventManager = new RunJSEventManager();
        public static LogOutEventManager logOutEventManager = new LogOutEventManager();
        public static DownloadPictureEventManager downloadPictureEventManager = new DownloadPictureEventManager();
        public static GetHuaWeiPictureEventManager getHuaWeiPictureEventManager = new GetHuaWeiPictureEventManager();
    }
}
