using CAF.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Model.CloudHelper
{
    /// <summary>
    /// 云服务商数据获取接口
    /// </summary>
    public interface ICloudHelper
    {
        /// <summary>
        /// 初始化爬虫
        /// </summary>
        void InitHelper();

        /// <summary>
        /// 判断是否处于登录状态
        /// </summary>
        /// <returns></returns>
        bool IsLogIn();

        /// <summary>
        /// 获取通讯录数据
        /// </summary>
        /// <returns></returns>
        Task<List<Contact>> SyncContactAsync();

        /// <summary>
        /// 获取短信数据
        /// </summary>
        /// <returns></returns>
        Task<List<Message>> SyncMessageAsync();

        /// <summary>
        /// 获取通话记录数据
        /// </summary>
        /// <returns></returns>
        Task<List<CallRecord>> SyncCallRecordAsync();

        /// <summary>
        /// 获取图片数据
        /// </summary>
        /// <returns></returns>
        Task<List<Picture>> SyncPictureAsync();

        /// <summary>
        /// 获取备忘录数据
        /// </summary>
        /// <returns></returns>
        Task<List<Note>> SyncNoteAsync();

        /// <summary>
        /// 获取录音数据
        /// </summary>
        /// <returns></returns>
        Task<List<Record>> SyncRecordAsync();

        /// <summary>
        /// 获取文件数据
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        Task<List<File>> SyncFileAsync();

        /// <summary>
        /// 获取手机所处地址
        /// </summary>
        /// <returns></returns>
        Task<List<Gps>> SyncLocationAsync();

        /// <summary>
        /// 下载指定图片
        /// </summary>
        /// <param name="picture"></param>
        /// <returns></returns>
        Task DownloadPictureAsync(Picture picture);

        /// <summary>
        /// 下载指定录音
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        Task DownloadRecordAsync(Record record);

        /// <summary>
        /// 下载指定文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        Task DownloadFileAsync(File file);

        /// <summary>
        /// 下载指定备忘录
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        Task DownloadNoteAsync(Note note);

        /// <summary>
        /// 下载图片缩略图
        /// </summary>
        /// <param name="picture"></param>
        /// <returns></returns>
        Task DownloadThumbnailAsync(Picture picture);
    }
}
