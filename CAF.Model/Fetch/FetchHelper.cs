using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CAF.Model.Fetch.Rule;
using CAF.Model.Common;

namespace CAF.Model.Fetch
{
    public class FetchHelper
    {
        public IDataFetcher fetcher = null;
        public FetchHelper()
        {
            switch (Setting.Provider)
            {
                case ServiceProvider.HuaWei:
                    fetcher = new HuaWeiRule();
                    break;
                case ServiceProvider.XiaoMi:
                    fetcher = new XiaoMiRule();
                    break;
            }
        }
    }
}
