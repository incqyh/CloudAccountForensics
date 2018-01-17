using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Model.Fetch.Rule
{
    public interface IDataFetcher
    {
        Task InitFetcherAsync();
        Task<string> FetchContactsJsonAsync();
        Task<string> FetchMessageJsonAsync(UInt32 pageNum = 1);
        Task<string> FetchCallRecordJsonAsync(UInt32 pageIndex = 0);
    }
}
