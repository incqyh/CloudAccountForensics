using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using CAF.Model;
using System.ComponentModel;

using CAF.Model.Common;
using CAF.Model.CloudHelper;
using System.IO;
using System.Collections.ObjectModel;

namespace CAF.View.Common
{
    /// <summary>
    /// ViewModel层，管理Model层接口与与View层binding的数据
    /// </summary>
    class VMManager
    {
        static VMManager vmManager = new VMManager();
        static public VMManager GetInstance()
        {
            return vmManager;
        }

        public BinderManager BinderManager { get; set; } = new BinderManager();

        CloudHelper ch;

        private bool ForensicsMutex = false;
        private bool LoadThumbnailMutex = false;

        public bool DoneForensics { get; set; } = false;

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

            ch = new CloudHelper();
        }

        bool IsLogIn()
        {
            return ch.IsLogIn();
        }

        /// <summary>
        /// 每次在用户登录之后都需要调用此函数
        /// </summary>
        public void InitCrawler()
        {
            Task.Run(() =>
            {
                try
                {
                    ch.InitHelper();
                    BinderManager.Status = "初始化网络爬虫成功";
                }
                catch (Exception)
                {
                    BinderManager.Status = "爬虫初始化失败，请检查是否登陆完成，并执行网页身份验证";
                    return;
                }
            });
        }

        public void SyncContact()
        {
            List<Contact> contacts = new List<Contact>();
            Task.Run(async () =>
            {
                try
                {
                    BinderManager.Status = "正在同步联系人";
                    contacts = await ch.SyncContactAsync();
                    BinderManager.Status = "同步联系人完成";
                }
                catch (Exception e)
                {
                    BinderManager.Status = "同步联系人失败，原因:" + e.Message;
                }
            }).ContinueWith(t => 
            {
                BinderManager.Contacts = new ObservableCollection<Contact>(contacts);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void SyncCallRecord()
        {
            List<CallRecord> callRecords = new List<CallRecord>();
            Task.Run(async () =>
            {
                try
                {
                    BinderManager.Status = "正在同步通话记录";
                    callRecords = await ch.SyncCallRecordAsync();
                    BinderManager.Status = "同步通话记录完成";
                }
                catch (Exception e)
                {
                    BinderManager.Status = "同步通话记录失败，原因:" + e.Message;
                }
            }).ContinueWith(t => 
            {
                BinderManager.CallRecords = new ObservableCollection<CallRecord>(callRecords);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void SyncMessage()
        {
            List<Message> messages = new List<Message>();
            Task.Run(async () =>
            {
                try
                {
                    BinderManager.Status = "正在同步短信";
                    messages = await ch.SyncMessageAsync();
                    BinderManager.Status = "同步短信完成";
                }
                catch (Exception e)
                {
                    BinderManager.Status = "同步短信失败，原因:" + e.Message;
                }
            }).ContinueWith(t => 
            {
                BinderManager.Messages = new ObservableCollection<Message>(messages);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void SyncPicture()
        {
            List<Picture> pictures = new List<Picture>();
            Task.Run(async () =>
            {
                try
                {
                    BinderManager.Status = "正在同步图片信息";
                    pictures = await ch.SyncPictureAsync();
                    BinderManager.Status = "同步图片信息完成";
                    if (Setting.Provider == ServiceProvider.HuaWei)
                        ch.InitHelper();
                }
                catch (Exception e)
                {
                    BinderManager.Status = "同步图片信息失败，原因:" + e.Message;
                }
            }).ContinueWith(t => 
            {
                BinderManager.Pictures = new ObservableCollection<Picture>(pictures);
                BinderManager.loadedCount = 0;
                DownloadThumbnail();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void SyncNote()
        {
            List<Note> notes = new List<Note>();
            Task.Run(async () =>
            {
                try
                {
                    BinderManager.Status = "正在同步备忘录";
                    notes = await ch.SyncNoteAsync();
                    BinderManager.Status = "同步备忘录完成";
                }
                catch (Exception e)
                {
                    BinderManager.Status = "同步备忘录失败，原因:" + e.Message;
                }
            }).ContinueWith(t => 
            {
                BinderManager.Notes = new ObservableCollection<Note>(notes);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void SyncRecord()
        {
            List<Record> records = new List<Record>();
            Task.Run(async () =>
            {
                try
                {
                    BinderManager.Status = "正在同步录音";
                    records = await ch.SyncRecordAsync();
                    BinderManager.Status = "同步录音完成";
                }
                catch (Exception e)
                {
                    BinderManager.Status = "同步录音失败，原因:" + e.Message;
                }
            }).ContinueWith(t => 
            {
                BinderManager.Records = new ObservableCollection<Record>(records);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void SyncGps()
        {
            List<Gps> gpses = new List<Gps>();
            Task.Run(async () =>
            {
                try
                {
                    BinderManager.Status = "正在同步地址信息";
                    gpses = await ch.SyncLocationAsync();
                    BinderManager.Status = "同步地址信息完成";
                }
                catch (Exception e)
                {
                    BinderManager.Status = "同步地址信息失败，原因:" + e.Message;
                }
            }).ContinueWith(t => 
            {
                BinderManager.Gpses = new ObservableCollection<Gps>(gpses);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void SyncFile(Model.Common.File file = null)
        {
            List<Model.Common.File> files = new List<Model.Common.File>();
            Task.Run(async () =>
            {
                try
                {
                    BinderManager.Status = "正在同步文件信息";
                    files = await ch.SyncFileAsync();
                    BinderManager.Status = "同步文件信息完成";
                }
                catch (Exception e)
                {
                    BinderManager.Status = "同步文件信息失败，原因:" + e.Message;
                }
            }).ContinueWith(t =>
            {
                if (file == null)
                    BinderManager.Files = new ObservableCollection<Model.Common.File>(files);
                else
                {
                    BinderManager.Files.Remove(file);
                    foreach (var i in files)
                        BinderManager.Files.Add(i);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void DownloadPicture(Picture picture)
        {
            if (picture.DownloadMutex)
            {
                BinderManager.Status = "上次下载尚未完成，请稍后再试";
                return;
            }

            BinderManager.Status = "正在下载图片";

            picture.DownloadMutex = true;
            Task.Run(async () =>
            {
                try
                {
                    string file = picture.LocalUrl;
                    if(!System.IO.File.Exists(file))
                        await ch.DownloadPicture(picture);
                    BinderManager.Status = "下载完成";
                }
                catch (Exception e)
                {
                    BinderManager.Status = "下载失败：" + e.Message;
                }
            }).ContinueWith(t => {
                picture.DownloadMutex = false;
                string file = picture.LocalUrl;
                if(System.IO.File.Exists(file))
                    System.Diagnostics.Process.Start(file);
            });
        }

        public void DownloadRecord(Record record)
        {
            if (record.DownloadMutex)
            {
                BinderManager.Status = "上次下载尚未完成，请稍后再试";
                return;
            }

            BinderManager.Status = "正在下载通话录音";

            record.DownloadMutex = true;
            Task.Run(async () =>
            {
                try
                {
                    string file = record.LocalUrl;
                    if (!System.IO.File.Exists(file))
                        await ch.DownloadRecord(record);
                    BinderManager.Status = "下载完成";
                }
                catch (Exception e)
                {
                    BinderManager.Status = "下载失败：" + e.Message;
                }
            }).ContinueWith(t =>
            {
                record.DownloadMutex = false;
                string file = record.LocalUrl;
                if (System.IO.File.Exists(file))
                    System.Diagnostics.Process.Start(file);
            });
        }

        public void DownloadFile(Model.Common.File file)
        {
            if (file.DownloadMutex)
            {
                BinderManager.Status = "该文件上次下载尚未完成，请稍后再试";
                return;
            }

            BinderManager.Status = "正在下载文件";

            file.DownloadMutex = true;
            Task.Run(async () =>
            {
                try
                {
                    string fileName = file.LocalUrl;
                    if(!System.IO.File.Exists(fileName))
                        await ch.DownloadFile(file);
                    BinderManager.Status = "下载完成";
                }
                catch (Exception e)
                {
                    BinderManager.Status = "下载失败：" + e.Message;
                }
            }).ContinueWith(t => {
                file.DownloadMutex = false;
                string fileName = file.LocalUrl;
                if(System.IO.File.Exists(fileName))
                    System.Diagnostics.Process.Start(fileName);
            });
        }

        public void DownloadNote(Note note)
        {
            if (note.DownloadMutex)
            {
                BinderManager.Status = "上次下载尚未完成，请稍后再试";
                return;
            }

            BinderManager.Status = "正在下载备忘录";

            note.DownloadMutex = true;
            Task.Run(async () =>
            {
                try
                {
                    string file = note.LocalUrl;
                    if(!System.IO.File.Exists(file))
                        await ch.DownloadNote(note);
                    BinderManager.Status = "下载完成";
                }
                catch (Exception e)
                {
                    BinderManager.Status = "下载失败：" + e.Message;
                }
            }).ContinueWith(t => {
                note.DownloadMutex = false;
                string file = note.LocalUrl;
                if(System.IO.File.Exists(file))
                    System.Diagnostics.Process.Start(file);
            });
        }

        public void DownloadThumbnail()
        {
            int totalCount = BinderManager.Pictures.Count;
            int currentCount = BinderManager.loadedCount;
            if (currentCount >= totalCount)
                return;

            if (LoadThumbnailMutex)
            {
                return;
            }
            BinderManager.Status = "正在加载图片";
            LoadThumbnailMutex = true;

            var tasks = new List<Task>();
            Task.Run(() =>
            {
                for (int i = currentCount; i < currentCount + 5 && i < totalCount; ++i)
                {
                    byte[] Thumbnail = new byte[0];
                    tasks.Add(ch.DownloadThumbnailAsync(BinderManager.Pictures[i]));
                    BinderManager.loadedCount++;
                }
                Task.WaitAll(tasks.ToArray(), 20000);
            }).ContinueWith(t =>
            {
                BinderManager.OnCollectionChanged();
                LoadThumbnailMutex = false;
                BinderManager.Status = "图片加载完成";
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public bool DuringForensics()
        {
            return ForensicsMutex;
        }
        public void StartForensics()
        {
            BinderManager.Status = "开始取证";
            if (ForensicsMutex)
            {
                BinderManager.Status = "上次取证尚未完成，请稍后再试";
                return;
            }
            ForensicsMutex = true;

            Directory.CreateDirectory(Setting.XmlFolder);
            Directory.CreateDirectory(Setting.PictureFolder);
            Directory.CreateDirectory(Setting.NoteFolder);
            Directory.CreateDirectory(Setting.RecordFolder);
            Directory.CreateDirectory(Setting.FileFolder);

            Task.Run(async () =>
            {
                try
                {
                    BinderManager.Status = "正在初始化爬虫";
                    ch.InitHelper();
                }
                catch (Exception)
                {
                    BinderManager.Status = "爬虫初始化失败，请检查是否登陆完成，并执行网页身份验证";
                    return;
                }

                try
                {
                    BinderManager.Status = "正在获取通讯录";
                    var contacts = await ch.SyncContactAsync();

                    XmlHelper.SaveContact(contacts);
                }
                catch (Exception e)
                {
                }

                try
                {
                    BinderManager.Status = "正在获取通话记录";
                    var callRecords = await ch.SyncCallRecordAsync();
                    XmlHelper.SaveCallRecord(callRecords);
                }
                catch (Exception e)
                {
                }

                try
                {
                    BinderManager.Status = "正在获取短信";
                    var messages = await ch.SyncMessageAsync();
                    XmlHelper.SaveMessage(messages);
                }
                catch (Exception e)
                {
                }

                try
                {
                    var records = await ch.SyncRecordAsync();
                    XmlHelper.SaveRecord(records);
                    int cnt = 1;
                    foreach (Record record in records)
                    {
                        BinderManager.Status = string.Format("正在获取第{0}个录音，共{1}个", cnt, records.Count);
                        cnt += 1;
                        await ch.DownloadRecord(record);
                    }
                }
                catch (Exception e)
                {

                }

                try
                {
                    BinderManager.Status = "正在获取备忘录";
                    var notes = await ch.SyncNoteAsync();
                    XmlHelper.SaveNote(notes);
                    int cnt = 1;
                    foreach (Note note in notes)
                    {
                        BinderManager.Status = string.Format("正在获取第{0}条备忘录，共{1}个", cnt, notes.Count);
                        cnt += 1;
                        await ch.DownloadNote(note);
                    }
                }
                catch (Exception e)
                {

                }

                try
                {
                    BinderManager.Status = "正在获取文件";
                    var files = await ch.SyncFileAsync();
                    int cnt = 1;
                    foreach (var file in files)
                    {
                        BinderManager.Status = string.Format("正在获取第{0}个文件，共{1}个", cnt, files.Count);
                        cnt += 1;
                        await ch.DownloadFile(file);
                    }
                }
                catch (Exception e)
                {

                }

                try
                {
                    BinderManager.Status = "正在获取gps";
                    var gpses = await ch.SyncLocationAsync();
                    XmlHelper.SaveGps(gpses);
                }
                catch (Exception e)
                {

                }

                try
                {
                    BinderManager.Status = "正在获取图片";
                    var pictures = await ch.SyncPictureAsync();
                    XmlHelper.SavePicture(pictures);
                    int cnt = 1;
                    foreach (Picture picture in pictures)
                    {
                        BinderManager.Status = string.Format("正在获取第{0}张图片，共{1}张", cnt, pictures.Count);
                        cnt += 1;
                        await ch.DownloadPicture(picture);
                    }
                }
                catch (Exception e)
                {

                }

                try
                {
                    BinderManager.Status = "正在将xml导入到数据库";
                    XmlToDb xtd = new XmlToDb();
                    xtd.Convert(Setting.XmlFolder, Setting.DbFile);
                }
                catch
                { }

                BinderManager.Status = "取证完成";
            }).ContinueWith(t =>
            {
                DoneForensics = true;
                ForensicsMutex = false;
            });
        }
    }
}
