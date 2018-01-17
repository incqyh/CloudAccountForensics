using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CAF.Model.Parser.Rule;
using CAF.Model.Common;

namespace CAF.Model.Parser
{
    public class ParserHelper
    {
        public IDataParser parser = null;

        public ParserHelper()
        {
            switch (Setting.Provider)
            {
                case ServiceProvider.HuaWei:
                    parser = new HuaWeiRule();
                    break;
                case ServiceProvider.XiaoMi:
                    parser = new XiaoMiRule();
                    break;
            }
        }

    }
}
