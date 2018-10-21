using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Model.Common
{
    public enum ServiceProvider
    {
        HuaWei,
        XiaoMi
    };

    public class Setting
    {
        public static ServiceProvider Provider
        {
            get; set;
        } = ServiceProvider.HuaWei;

        public static Dictionary<ServiceProvider, string> MainUrl = new Dictionary<ServiceProvider, string>()
        {
            { ServiceProvider.HuaWei, "https://cloud.huawei.com"},
            { ServiceProvider.XiaoMi, "https://i.mi.com"},
        };

        public static string LogFile { get; set; } = "CAF.log";
        public static string DownloadFolder { get; set; } = "Download";
        public static string SaveFolder { get; set; } = "Save";
    }
}
