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

    /// <summary>
    /// 在调用任何接口之前先设置
    /// </summary>
    public class Setting
    {
        public static ServiceProvider Provider
        {
            get; set;
        }
    }
}
