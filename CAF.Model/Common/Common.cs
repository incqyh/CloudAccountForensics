using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Model.Common
{
    /// <summary>
    /// 全局的选项，选择当前正在使用的服务商
    /// 该文将内的设置可以写成文本的形式存放在文件中
    /// 即一个setting.txt的文件
    /// 若以后设置很多可以考虑
    /// </summary>
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

        public static bool IsMain { get; set; } = true;
        public static string LogFile { get; set; } = "CAF.log";
        public static string DbFile { get; set; } = @"EVIDENCE.db";
        public static string XmlFolder { get; set; } = @"Save\Xml\";
        public static string NoteFolder { get; set; } = @"Save\Note\";
        public static string PictureFolder { get; set; } = @"Save\Picture\";
        public static string RecordFolder { get; set; } = @"Save\Record\";
        public static string FileFolder { get; set; } = @"Save\File\";
    }
}
