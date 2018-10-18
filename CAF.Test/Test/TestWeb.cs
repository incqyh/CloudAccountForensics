using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CAF.Model.Fetch;
using CAF.Model.Parser;

namespace CAF.Test
{
    public class TestWeb
    {
        FetchHelper fh = new FetchHelper();
        ParserHelper ph = new ParserHelper();

        public async void TestMain()
        {
            try
            {
                await fh.fetcher.InitFetcher();
                await TestFetchContactsAsync();
                await TestFetchMessageAsync();
                await TestFetchCallRecordAsync();
            }
            catch
            {
                
            }
        }

        async Task TestFetchContactsAsync()
        {
            string re = await fh.fetcher.FetchContactsJsonAsync();
            Console.WriteLine(re);
            ph.parser.ContactsParser(re);
            Print.PrintContacts();
        }

        async Task TestFetchMessageAsync()
        {
            string re = await fh.fetcher.FetchMessageJsonAsync();
            Console.WriteLine(re);
            ph.parser.MessageParser(re);
            Print.PrintMessage();
        }

        async Task TestFetchCallRecordAsync()
        {
            string re = await fh.fetcher.FetchCallRecordJsonAsync();
            Console.WriteLine(re);
            ph.parser.CallRecordParser(re);
            Print.PrintCallRecord();
        }
    }
}
