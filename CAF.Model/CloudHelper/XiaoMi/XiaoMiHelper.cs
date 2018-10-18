using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Model.CloudHelper.XiaoMi
{
    class RuntimeData
    {
        public bool isFirstTime;
        public string syncIgnoreTag;
        public string syncTag;
        public string syncThreadingTag;
        public bool lastPage;
    }

    partial class XiaoMiHelper : ICloudHelper
    {
        RuntimeData runtimeData = null;

        public void InitHelper()
        {
            InitFetcher();
        }

        public async Task SyncCallRecordAsync()
        {
            runtimeData = new RuntimeData
            {
                isFirstTime = true,
                syncTag = "",
                lastPage = false
            };
            while (!runtimeData.lastPage)
            {
                string data = await FetchCallRecordAsync();
                CallRecordParser(data);
            }
        }

        public async Task SyncContactsAsync()
        {
            runtimeData = new RuntimeData
            {
                isFirstTime = true,
                syncTag = "0",
                syncIgnoreTag = "0",
                lastPage = false
            };
            while (!runtimeData.lastPage)
            {
                string data = await FetchContactsAsync();
                ContactsParser(data);
            }
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
            // runtimeData = new RuntimeData
            // {
            //     isFirstTime = true,
            //     syncTag = "0",
            //     syncThreadingTag = "",
            //     lastPage = "false"
            // };
            // while (runtimeData.lastPage != "true")
            // {
                string data = await FetchMessageAsync();
                MessageParser(data);
            // }
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
