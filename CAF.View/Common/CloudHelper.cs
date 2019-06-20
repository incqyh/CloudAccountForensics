using CAF.Model.CloudHelper;
using CAF.Model.CloudHelper.HuaWei;
using CAF.Model.CloudHelper.XiaoMi;
using CAF.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.View.Common
{
    /// <summary>
    /// 在该类中对ICloudHelper封装一下，这样就在VMManager中调用就非常方便
    /// </summary>
    public class CloudHelper
    {
        readonly ICloudHelper ich;
        public CloudHelper()
        {
            switch (Setting.Provider)
            {
                case ServiceProvider.HuaWei:
                    ich = new HuaWeiHelper();
                    break;
                case ServiceProvider.XiaoMi:
                    ich = new XiaoMiHelper();
                    break;
            }
        }

        public void InitHelper()
        {
            ich.InitHelper();
        }

        public bool IsLogIn()
        {
            return ich.IsLogIn();
        }

        public Task<List<Contact>> SyncContactAsync()
        {
            return ich.SyncContactAsync();
        }

        public Task<List<Message>> SyncMessageAsync()
        {
            return ich.SyncMessageAsync();
        }

        public Task<List<CallRecord>> SyncCallRecordAsync()
        {
            return ich.SyncCallRecordAsync();
        }

        public Task<List<Picture>> SyncPictureAsync()
        {
            return ich.SyncPictureAsync();
        }

        public Task<List<Note>> SyncNoteAsync()
        {
            return ich.SyncNoteAsync();
        }

        public Task<List<Record>> SyncRecordAsync()
        {
            return ich.SyncRecordAsync();
        }

        public Task<List<File>> SyncFileAsync()
        {
            return ich.SyncFileAsync();
        }

        public Task<List<Gps>> SyncLocationAsync()
        {
            return ich.SyncLocationAsync();
        }

        public Task DownloadPicture(Picture picture)
        {
            return ich.DownloadPictureAsync(picture);
        }

        public Task DownloadRecord(Record record)
        {
            return ich.DownloadRecordAsync(record);
        }

        public Task DownloadFile(File file)
        {
            return ich.DownloadFileAsync(file);
        }

        public Task DownloadNote(Note note)
        {
            return ich.DownloadNoteAsync(note);
        }

        public Task DownloadThumbnailAsync(Picture picture)
        {
            return ich.DownloadThumbnailAsync(picture);
        }
    }
}
