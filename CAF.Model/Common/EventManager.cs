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

    /// <summary>
    /// 运行js代码，由于在编写爬虫的时候有部分请求难以分析出具体的流程
    /// 所以在实在没办法的情况下可以使用前端的浏览器来辅助获取一些数据
    /// 该事件即通知浏览器运行某段js代码
    /// </summary>
    public class RunJSEventManager
    {
        public delegate void RunJSEventHandler(object sender, RunJSEventArgs e);
        public event RunJSEventHandler GetUniqueIDEvent = delegate { };

        public void GetUniqueID(RunJSEventArgs e)
        {
            GetUniqueIDEvent?.Invoke(this, e);
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

    /// <summary>
    /// 下载图片的事件，当前端界面显示图片的时候我们希望点击图片就可以直接下载图片
    /// 此事件可以通知调用下载图片的接口
    /// </summary>
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
        public static DownloadPictureEventManager downloadPictureEventManager = new DownloadPictureEventManager();
    }
}
