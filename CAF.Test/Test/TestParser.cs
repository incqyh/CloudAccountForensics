using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

using CAF.Model.Parser;
using CAF.Model.DataObject;
using CAF.Model.Common;

namespace CAF.Test
{
    class TestParser
    {
        ParserHelper ph = new ParserHelper();

        public void TestMain()
        {
            TestContactsParser();
            TestCallRecordParser();
            TestMessageParser();
        }

        void TestContactsParser()
        {
            string path = @"D:/文档/contacts.json";
            string text = File.ReadAllText(path);
            ph.parser.ContactsParser(text);

            Console.WriteLine("解析json之后的通讯录数据如下");
            Print.PrintContacts();
        }

        void TestCallRecordParser()
        {
            string path = @"D:/文档/callrecord.json";
            string text = File.ReadAllText(path);
            ph.parser.CallRecordParser(text);

            Console.WriteLine("解析json之后的通话记录");
            Print.PrintCallRecord();
        }

        void TestMessageParser()
        {
            string path = @"D:/文档/message.json";
            string text = File.ReadAllText(path);
            ph.parser.MessageParser(text);

            Console.WriteLine("解析json之后的短信数据");
            Print.PrintMessage();
        }
    }
}
