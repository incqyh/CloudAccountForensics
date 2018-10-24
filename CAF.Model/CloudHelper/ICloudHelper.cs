using CAF.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Model.CloudHelper
{
    interface ICloudHelper
    {
        void InitHelper();
        bool IsLogIn();

        Task<List<Contact>> SyncContactAsync();
        Task<List<Message>> SyncMessageAsync();
        Task<List<CallRecord>> SyncCallRecordAsync();
        Task<List<Picture>> SyncPictureAsync();
        Task<List<Note>> SyncNoteAsync();
        Task<List<Record>> SyncRecordAsync();
        Task<List<File>> SyncFileAsync(File file);
        Task<List<Gps>> SyncLocationAsync();

        Task DownloadPictureAsync(Picture picture);
        Task DownloadRecordAsync(Record record);
        Task DownloadFileAsync(File file);
        Task DownloadNoteAsync(Note note);
        Task DownloadThumbnailAsync(Picture picture);
    }
}
