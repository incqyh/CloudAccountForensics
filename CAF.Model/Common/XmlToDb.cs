using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Model.Common
{
    /// <summary>
    /// 该类可使xml文件中的数据导出到一个数据库中
    /// </summary>
    public class XmlToDb
    {
        PMFProgressCallBack callback;
        public XmlToDb()
        {
            callback = PMF_ProgressCallBack;
        }

        public delegate int PMFProgressCallBack([MarshalAs(UnmanagedType.LPWStr)]string taskId,
           [MarshalAs(UnmanagedType.LPWStr)]string appId,
           [MarshalAs(UnmanagedType.LPWStr)]string txtStatus,
           int total,
           int cur,
           IntPtr pData);

        [DllImport("DBWriter.dll", EntryPoint = "MF_XMLFileToDB", CallingConvention = CallingConvention.Cdecl)]
        public static extern int MF_XMLFileToDB(PMFProgressCallBack progressCallBack, [MarshalAs(UnmanagedType.LPWStr)]string taskId, [MarshalAs(UnmanagedType.LPWStr)]string appId, [MarshalAs(UnmanagedType.LPWStr)]string path, [MarshalAs(UnmanagedType.LPWStr)]string file);
       
        [DllImport("DBWriter.dll", EntryPoint = "MF_DataSummary", CallingConvention = CallingConvention.Cdecl)]
        public static extern int MF_DataSummary(PMFProgressCallBack progressCallBack, [MarshalAs(UnmanagedType.LPWStr)]string taskId, [MarshalAs(UnmanagedType.LPWStr)]string file);

        public int PMF_ProgressCallBack(string taskId, string appId, string txtStatus, int total, int cur, IntPtr pData)
        {
            return 0;
        }

        public void Convert(string xmlPath, string dbPath)
        {          
            MF_XMLFileToDB(callback, "1", "2", xmlPath, dbPath);
            MF_DataSummary(callback, "1", dbPath);
        }
    }
}
