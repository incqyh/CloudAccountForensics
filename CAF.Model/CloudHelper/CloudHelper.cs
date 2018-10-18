using CAF.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Model.CloudHelper
{
    public class CloudHelper
    {
        readonly ICloudHelper cloudHelper;
        public CloudHelper()
        {
            switch (Setting.Provider)
            {
                case ServiceProvider.HuaWei:
                    cloudHelper = new HuaWei.HuaWeiHelper();
                    break;
                case ServiceProvider.XiaoMi:
                    cloudHelper = new XiaoMi.XiaoMiHelper();
                    break;
            }
        }

        public void InitHelper()
        {
            cloudHelper.InitHelper();
        }

        public Task SyncContactsAsync()
        {
            return cloudHelper.SyncContactsAsync();
        }

        public Task SyncMessageAsync()
        {
            return cloudHelper.SyncMessageAsync();
        }

        public Task SyncCallRecordAsync()
        {
            return cloudHelper.SyncCallRecordAsync();
        }

        public Task SyncPictureAsync()
        {
            return cloudHelper.SyncPictureAsync();
        }

        public Task SyncNoteAsync()
        {
            return cloudHelper.SyncNoteAsync();
        }

        public Task SyncRecordAsync()
        {
            return cloudHelper.SyncRecordAsync();
        }

        public Task SyncFileAsync()
        {
            return cloudHelper.SyncFileAsync();
        }

        public Task SyncLocationAsync()
        {
            return cloudHelper.SyncLocationAsync();
        }

        public Task DownloadPicture(string Index)
        {
            return cloudHelper.DownloadPicture(Index);
        }

        public Task DownloadRecord(string Index)
        {
            return cloudHelper.DownloadRecord(Index);
        }

        public Task DownloadFile(string Index)
        {
            return cloudHelper.DownloadFile(Index);
        }
    }
}
