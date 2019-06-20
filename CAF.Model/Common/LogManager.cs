using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Model.Common
{
    enum LogType
    {
        ERROR,
        NORMAL
    }

    /// <summary>
    /// 用来打log，当前程序还不算太大暂时不加上log功能，以后有必要会开启
    /// </summary>
    class LogManager
    {
        static public void Log(string msg, LogType type)
        {
            switch (type)
            {
                case LogType.ERROR:
                    LogError(msg);
                    break;
                case LogType.NORMAL:
                    LogNormal(msg);
                    break;
            }
        }

        static public void LogError(string msg)
        {
            using (FileStream fs = new FileStream(Setting.LogFile, FileMode.Append, FileAccess.Write))
            {
                StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding(936));
                sw.Write("[ ERROR ] ");
                sw.Write(DateTime.Now);
                sw.Write("  ");
                sw.Write(msg);
                sw.Close();
            }
        }

        static public void LogNormal(string msg)
        {
            using (FileStream fs = new FileStream(Setting.LogFile, FileMode.Append, FileAccess.Write))
            {
                StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding(936));
                sw.Write("[ NORMAL ] ");
                sw.Write(DateTime.Now);
                sw.Write("  ");
                sw.Write(msg);
                sw.Close();
            }
        }
    }
}
