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
        public bool isFirstTime;
        public bool isEnd;
        public uint totalCount;
    }

    partial class HuaWeiHelper : ICloudHelper
    {
        RuntimeData runtimeData = null;
        public void InitHelper()
        {
            InitFetcher();
            InitParser();
        }

        public async Task SyncCallRecordAsync()
        {
            runtimeData = new RuntimeData
            {
                isFirstTime = true,
                isEnd = false,
            };

            await FetchCallRecordAsync();
        }

        public async Task SyncContactsAsync()
        {
            string data = await FetchContactsAsync();
            ContactsParser(data);
        }

        public Task SyncFileAsync()
        {
            throw new NotImplementedException();
        }

        public Task SyncLocationAsync()
        {
            throw new NotImplementedException();
        }

        public async Task SyncMessageAsync()
        {
            runtimeData = new RuntimeData
            {
                isFirstTime = true,
                isEnd = false,
                totalCount = 1000000
            };
            await FetchMessageAsync();
        }

        public Task SyncNoteAsync()
        {
            throw new NotImplementedException();
        }

        public Task SyncPictureAsync()
        {
            throw new NotImplementedException();
        }

        public Task SyncRecordAsync()
        {
            throw new NotImplementedException();
        }

        public Task DownloadFile(string Index)
        {
            throw new NotImplementedException();
        }

        public Task DownloadPicture(string Index)
        {
            throw new NotImplementedException();
        }

        public Task DownloadRecord(string Index)
        {
            throw new NotImplementedException();
        }

    }
}
