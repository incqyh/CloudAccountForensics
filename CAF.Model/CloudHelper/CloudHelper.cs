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

        public Task<List<Contact>> SyncContactAsync()
        {
            return cloudHelper.SyncContactAsync();
        }

        public Task<List<Message>> SyncMessageAsync()
        {
            return cloudHelper.SyncMessageAsync();
        }

        public Task<List<CallRecord>> SyncCallRecordAsync()
        {
            return cloudHelper.SyncCallRecordAsync();
        }

        public Task<List<Picture>> SyncPictureAsync()
        {
            return cloudHelper.SyncPictureAsync();
        }

        public Task<List<Note>> SyncNoteAsync()
        {
            return cloudHelper.SyncNoteAsync();
        }

        public Task<List<Record>> SyncRecordAsync()
        {
            return cloudHelper.SyncRecordAsync();
        }

        public Task<List<File>> SyncFileAsync(File file = null)
        {
            return cloudHelper.SyncFileAsync(file);
        }

        public Task<List<Gps>> SyncLocationAsync()
        {
            return cloudHelper.SyncLocationAsync();
        }

        public Task DownloadPicture(Picture picture)
        {
            return cloudHelper.DownloadPictureAsync(picture);
        }

        public Task DownloadRecord(Record record)
        {
            return cloudHelper.DownloadRecordAsync(record);
        }

        public Task DownloadFile(File file)
        {
            return cloudHelper.DownloadFileAsync(file);
        }

        public Task DownloadNote(Note note)
        {
            return cloudHelper.DownloadNoteAsync(note);
        }
    }
}
