using CAF.Model.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CAF.Model.CloudHelper.HuaWei
{
    class RuntimeData
    {
        public bool isEnd;
        public uint totalCount;
    }

    public partial class HuaWeiHelper : ICloudHelper
    {
        RuntimeData runtimeData = null;

        public Task DownloadFileAsync(File file)
        {
            throw new NotImplementedException();
        }

        public Task DownloadNoteAsync(Note note)
        {
            throw new NotImplementedException();
        }

        public Task DownloadPictureAsync(Picture picture)
        {
            throw new NotImplementedException();
        }

        public Task DownloadRecordAsync(Record record)
        {
            throw new NotImplementedException();
        }

        public void InitHelper()
        {
            InitFetcher();
            InitParser();
        }

        public async Task<List<CallRecord>> SyncCallRecordAsync()
        {
            runtimeData = new RuntimeData
            {
                isEnd = false,
            };

            return await FetchCallRecordAsync();
        }

        public async Task<List<Contact>> SyncContactAsync()
        {
            string data = await FetchContactAsync();
            return ParseContact(data);
        }

        public Task<List<File>> SyncFileAsync(File file)
        {
            throw new NotImplementedException();
        }

        public Task<List<Gps>> SyncLocationAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Message>> SyncMessageAsync()
        {
            runtimeData = new RuntimeData
            {
                isEnd = false,
                totalCount = 1000000
            };
            return await FetchMessageAsync();
        }

        public Task<List<Note>> SyncNoteAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<Picture>> SyncPictureAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<Record>> SyncRecordAsync()
        {
            throw new NotImplementedException();
        }
    }
}
