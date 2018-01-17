using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.IO;

using CAF.Model.Common;

namespace CAF.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Setting.Provider = ServiceProvider.HuaWei;

            TestParser testParser = new TestParser();
            testParser.TestMain();
            // TestDB testDB = new TestDB();
            // testDB.TestMain();
            // TestWeb test = new TestWeb();
            // test.TestMain();

            Console.ReadKey();
        }
    }
}
