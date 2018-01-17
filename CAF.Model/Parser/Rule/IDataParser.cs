using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Model.Parser.Rule
{
    public interface IDataParser
    {
        void ContactsParser(string rawJson);
        void MessageParser(string rawJson);
        void CallRecordParser(string rawJson);

        // 暂时不实现，因为小米没有备忘录服务，华为的服务也无法正常使用
        // void MemoParser(string raw_data);
    }
}
