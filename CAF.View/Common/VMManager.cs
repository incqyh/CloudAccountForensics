using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CAF.Model;
using System.ComponentModel;

using CAF.Model.Common;
using CAF.Model.CloudHelper;
using System.IO;

namespace CAF.View.Common
{
    class VMHelper
    {
        public static VMManager vmManager = new VMManager();
    }

    class VMManager : INotifyPropertyChanged 
    {
        CloudHelper ch;

        bool UpdateDBMutex { get; set; } = false;
        bool ReadFromDBMutex { get; set; } = false;
        bool ReadFromWebMutex { get; set; } = false;
        bool DownloadPictureMutex { get; set; } = false;
        bool DownloadRecordMutex { get; set; } = false;
        bool DownloadNoteMutex { get; set; } = false;
        bool DownloadFileMutex { get; set; } = false;

        string status;
        public string Status
        {
            get { return status; }
            set
            {
                status = value;
                if (PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Status"));
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public void Init(string provider)
        {
            switch (provider)
            {
                case "华为":
                    Setting.Provider = ServiceProvider.HuaWei;
                    break;
                case "小米":
                    Setting.Provider = ServiceProvider.XiaoMi;
                    break;
            }

            if (!Directory.Exists(Setting.DownloadFolder))
                Directory.CreateDirectory(Setting.DownloadFolder);
            if (!Directory.Exists(Setting.SaveFolder))
                Directory.CreateDirectory(Setting.SaveFolder);

            DataManager.Contacts.Clear();
            DataManager.CallRecords.Clear();
            DataManager.Messages.Clear();

            ch = new CloudHelper();
        }

        public void UpdateDB()
        {
            if (UpdateDBMutex)
            {
                Status = "上次保存尚未完成，请稍后再试";
                return;
            }

            Status = "正在保存到本地";

            UpdateDBMutex = true;
            Task.Run(() =>
            {
                Status = "保存完成";
            }).ContinueWith(t => { UpdateDBMutex = false; });
        }

        public void ReadFromWeb()
        {
            if (ReadFromWebMutex)
            {
                Status = "上次获取尚未完成，请稍后再试";
                return;
            }

            Status = "正在从网络获取数据";

            ReadFromWebMutex = true;
            Task.Run(async () =>
            {
                try
                {
                    ch.InitHelper();
                    Status = "初始化网络爬虫成功";
                }
                catch (Exception)
                {
                    Status = "爬虫初始化失败，请检查是否登陆完成，并执行网页身份验证";
                    return;
                }

                // try
                // {
                //     await ch.SyncCallRecordAsync();
                //     Status = "同步通话记录完成";
                // }
                // catch (Exception e)
                // {
                //     Status = "同步通话记录失败，原因:" + e.Message;
                // }

                // try
                // {
                //     await ch.SyncMessageAsync();
                //     Status = "同步短信完成";
                // }
                // catch (Exception e)
                // {
                //     Status = "同步短信失败，原因:" + e.Message;
                // }

                // try
                // {
                //     await ch.SyncContactsAsync();
                //     Status = "同步联系人完成";
                // }
                // catch (Exception e)
                // {
                //     Status = "同步联系人失败，原因:" + e.Message;
                // }

                try
                {
                    await ch.SyncPictureAsync();
                    Status = "同步图片信息完成";
                }
                catch (Exception e)
                {
                    Status = "同步图片信息失败，原因:" + e.Message;
                }

                // try
                // {
                //     await ch.SyncNoteAsync();
                //     Status = "同步备忘录完成";
                // }
                // catch (Exception e)
                // {
                //     Status = "同步备忘录失败，原因:" + e.Message;
                // }

                // try
                // {
                //     await ch.SyncRecordAsync();
                //     Status = "同步录音完成";
                // }
                // catch (Exception e)
                // {
                //     Status = "同步录音失败，原因:" + e.Message;
                // }

                // try
                // {
                //     await ch.SyncFileAsync();
                //     Status = "同步文件信息完成";
                // }
                // catch (Exception e)
                // {
                //     Status = "同步文件信息失败，原因:" + e.Message;
                // }

                // try
                // {
                //     await ch.SyncLocationAsync();
                //     Status = "同步地址信息完成";
                // }
                // catch (Exception e)
                // {
                //     Status = "同步地址信息失败，原因:" + e.Message;
                // }
            }).ContinueWith(t => {ReadFromWebMutex = false;});
        }

        public void DownloadPicture(int index)
        {
            if (DownloadPictureMutex)
            {
                Status = "上次下载尚未完成，请稍后再试";
                return;
            }

            Status = "正在下载图片";

            DownloadPictureMutex = true;
            Task.Run(async () =>
            {
                try
                {
                    string file = string.Format(@"{0}\{1}", Setting.DownloadFolder, DataManager.Pictures[index].Name);
                    if(!System.IO.File.Exists(file))
                        await ch.DownloadPicture(index);
                    Status = "下载完成";
                }
                catch (Exception e)
                {
                    Status = "下载失败：" + e.Message;
                }
            }).ContinueWith(t => {
                DownloadPictureMutex = false;
                string file = string.Format(@"{0}\{1}", Setting.DownloadFolder, DataManager.Pictures[index].Name);
                if(System.IO.File.Exists(file))
                    System.Diagnostics.Process.Start(file);
            });
        }

        public void DownloadRecord(int index)
        {
            if (DownloadRecordMutex)
            {
                Status = "上次下载尚未完成，请稍后再试";
                return;
            }

            Status = "正在下载通话记录";

            DownloadRecordMutex = true;
            Task.Run(async () =>
            {
                try
                {
                    string file = string.Format(@"{0}\{1}", Setting.DownloadFolder, DataManager.Records[index].Name);
                    if(!System.IO.File.Exists(file))
                        await ch.DownloadRecord(index);
                    Status = "下载完成";
                }
                catch (Exception e)
                {
                    Status = "下载失败：" + e.Message;
                }
            }).ContinueWith(t => {
                DownloadRecordMutex = false;
                string file = string.Format(@"{0}\{1}", Setting.DownloadFolder, DataManager.Records[index].Name);
                if(System.IO.File.Exists(file))
                    System.Diagnostics.Process.Start(file);
            });
        }

        public void DownloadFile(int index)
        {
            if (DownloadFileMutex)
            {
                Status = "上次下载尚未完成，请稍后再试";
                return;
            }

            Status = "正在下载文件";

            DownloadFileMutex = true;
            Task.Run(async () =>
            {
                try
                {
                    string file = string.Format(@"{0}\{1}", Setting.DownloadFolder, DataManager.Files[index].Name);
                    if(!System.IO.File.Exists(file))
                        await ch.DownloadFile(index);
                    Status = "下载完成";
                }
                catch (Exception e)
                {
                    Status = "下载失败：" + e.Message;
                }
            }).ContinueWith(t => {
                DownloadFileMutex = false;
                string file = string.Format(@"{0}\{1}", Setting.DownloadFolder, DataManager.Files[index].Name);
                if(System.IO.File.Exists(file))
                    System.Diagnostics.Process.Start(file);
            });
        }

        public void DownloadNote(int index)
        {
            if (DownloadNoteMutex)
            {
                Status = "上次下载尚未完成，请稍后再试";
                return;
            }

            Status = "正在下载备忘录";

            DownloadNoteMutex = true;
            Task.Run(async () =>
            {
                try
                {
                    string file = string.Format(@"{0}\{1}.txt", Setting.DownloadFolder, DataManager.Notes[index].Id);
                    if(!System.IO.File.Exists(file))
                        await ch.DownloadNote(index);
                    Status = "下载完成";
                }
                catch (Exception e)
                {
                    Status = "下载失败：" + e.Message;
                }
            }).ContinueWith(t => {
                DownloadNoteMutex = false;
                string file = string.Format(@"{0}\{1}.txt", Setting.DownloadFolder, DataManager.Notes[index].Id);
                if(System.IO.File.Exists(file))
                    System.Diagnostics.Process.Start(file);
            });
        }
    }
}
