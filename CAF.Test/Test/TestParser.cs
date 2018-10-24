using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

using CAF.Model.CloudHelper.XiaoMi;
using CAF.Model.CloudHelper.HuaWei;

namespace CAF.Test
{
    class TestParser
    {
        HuaWeiHelper ph = new HuaWeiHelper();

        public void TestMain()
        {
            // TestContactsParser();
            // TestCallRecordParser();
            // TestMessageParser();
            TestPictureParser();
            TestNoteParser();
            TestRecordParser();
            TestFileParser();
            TestGpsParser();
        }

        void TestContactsParser()
        {
            string path = @"D:/Documents/contacts.json";
            string text = File.ReadAllText(path);
            ph.ParseContact(text);

            Console.WriteLine("解析json之后的通讯录数据如下");
            Print.PrintContacts();
        }

        void TestCallRecordParser()
        {
            string path = @"D:/Documents/callrecord.json";
            string text = File.ReadAllText(path);
            ph.ParseCallRecord(text);

            Console.WriteLine("解析json之后的通话记录");
            Print.PrintCallRecord();
        }

        void TestMessageParser()
        {
            string path = @"D:/Documents/message.json";
            string text = File.ReadAllText(path);
            ph.ParseMessage(text);

            Console.WriteLine("解析json之后的短信数据");
            Print.PrintMessage();
        }

        void TestPictureParser()
        {
            string path = @"D:/Documents/picture.json";
            string text = File.ReadAllText(path);
            ph.ParsePicture(text);

            Console.WriteLine("解析json之后的图片数据");
            Print.PrintPicture();
        }

        void TestNoteParser()
        {
            string path = @"D:/Documents/note.json";
            string text = File.ReadAllText(path);
            ph.ParseNote(text);

            Console.WriteLine("解析json之后的图片数据");
            Print.PrintNote();
        }

        void TestRecordParser()
        {
            string path = @"D:/Documents/record.json";
            string text = File.ReadAllText(path);
            ph.ParseRecord(text);

            Console.WriteLine("解析json之后的图片数据");
            Print.PrintNote();
        }

        void TestFileParser()
        {
            string path = @"D:/Documents/file.json";
            string text = File.ReadAllText(path);
            ph.ParseFile(text);

            Console.WriteLine("解析json之后的图片数据");
            Print.PrintFile();
        }

        void TestGpsParser()
        {
            string path = @"D:/Documents/gps.json";
            string text = File.ReadAllText(path);
            ph.ParseGps(text);

            Console.WriteLine("解析json之后的图片数据");
            Print.PrintGps();
        }
    }
}
