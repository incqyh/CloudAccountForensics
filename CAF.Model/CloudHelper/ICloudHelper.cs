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

        Task SyncContactsAsync();
        Task SyncMessageAsync();
        Task SyncCallRecordAsync();
        Task SyncPictureAsync();
        Task SyncNoteAsync();
        Task SyncRecordAsync();
        Task SyncFileAsync();
        Task SyncLocationAsync();

        Task DownloadPictureAsync(int Index);
        Task DownloadRecordAsync(int Index);
        Task DownloadFileAsync(int Index);
        Task DownloadNoteAsync(int index);
    }
}
