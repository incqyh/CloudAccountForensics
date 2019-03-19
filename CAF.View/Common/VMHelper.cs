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
    class VMHelper
    {
        public static VMManager vmManager = new VMManager();
    }

    class VMManager : INotifyPropertyChanged 
    {
        CloudHelper ch;

        private bool ForensicsMutex = false;
        private bool LoadThumbnailMutex = false;
        bool isCrawlerInit = false;

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

            if (!Directory.Exists(Setting.XmlFolder))
                Directory.CreateDirectory(Setting.XmlFolder);
            if (!Directory.Exists(Setting.PictureFolder))
                Directory.CreateDirectory(Setting.PictureFolder);
            if (!Directory.Exists(Setting.NoteFolder))
                Directory.CreateDirectory(Setting.NoteFolder);
            if (!Directory.Exists(Setting.RecordFolder))
                Directory.CreateDirectory(Setting.RecordFolder);
            if (!Directory.Exists(Setting.FileFolder))
                Directory.CreateDirectory(Setting.FileFolder);

            ch = new CloudHelper();
            isCrawlerInit = false;
        }

        bool IsLogIn()
        {
            return ch.IsLogIn();
        }

        public void InitCrawler()
        {
            Task.Run(() =>
            {
                try
                {
                    ch.InitHelper();
                    Status = "初始化网络爬虫成功";
                    isCrawlerInit = true;
                }
                catch (Exception)
                {
                    Status = "爬虫初始化失败，请检查是否登陆完成，并执行网页身份验证";
                    return;
                }
            });
        }

        public void SyncContact()
        {
            List<Contact> contacts = new List<Contact>();
            Task.Run(async () =>
            {
                if (!isCrawlerInit)
                {
                    ch.InitHelper();
                    isCrawlerInit = true;
                }
                try
                {
                    Status = "正在同步联系人";
                    contacts = await ch.SyncContactAsync();
                    Status = "同步联系人完成";
                }
                catch (Exception e)
                {
                    Status = "同步联系人失败，原因:" + e.Message;
                }
            }).ContinueWith(t => 
            {
                BinderManager.contactsBinder.Contacts = new ObservableCollection<Contact>(contacts);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void SyncCallRecord()
        {
            List<CallRecord> callRecords = new List<CallRecord>();
            Task.Run(async () =>
            {
                if (!isCrawlerInit)
                {
                    ch.InitHelper();
                    isCrawlerInit = true;
                }
                try
                {
                    Status = "正在同步通话记录";
                    callRecords = await ch.SyncCallRecordAsync();
                    Status = "同步通话记录完成";
                }
                catch (Exception e)
                {
                    Status = "同步通话记录失败，原因:" + e.Message;
                }
            }).ContinueWith(t => 
            {
                BinderManager.callRecordBinder.CallRecords = new ObservableCollection<CallRecord>(callRecords);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void SyncMessage()
        {
            List<Message> messages = new List<Message>();
            Task.Run(async () =>
            {
                if (!isCrawlerInit)
                {
                    ch.InitHelper();
                    isCrawlerInit = true;
                }
                try
                {
                    Status = "正在同步短信";
                    messages = await ch.SyncMessageAsync();
                    Status = "同步短信完成";
                }
                catch (Exception e)
                {
                    Status = "同步短信失败，原因:" + e.Message;
                }
            }).ContinueWith(t => 
            {
                BinderManager.messageBinder.Messages = new ObservableCollection<Message>(messages);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void SyncPicture()
        {
            List<Picture> pictures = new List<Picture>();
            Task.Run(async () =>
            {
                if (!isCrawlerInit)
                {
                    ch.InitHelper();
                    isCrawlerInit = true;
                }
                try
                {
                    Status = "正在同步图片信息";
                    pictures = await ch.SyncPictureAsync();
                    Status = "同步图片信息完成";
                    if (Setting.Provider == ServiceProvider.HuaWei)
                        ch.InitHelper();
                }
                catch (Exception e)
                {
                    Status = "同步图片信息失败，原因:" + e.Message;
                }
            }).ContinueWith(t => 
            {
                BinderManager.pictureBinder.Pictures = new ObservableCollection<Picture>(pictures);
                BinderManager.pictureBinder.loadedCount = 0;
                DownloadThumbnail();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void SyncNote()
        {
            List<Note> notes = new List<Note>();
            Task.Run(async () =>
            {
                if (!isCrawlerInit)
                {
                    ch.InitHelper();
                    isCrawlerInit = true;
                }
                try
                {
                    Status = "正在同步备忘录";
                    notes = await ch.SyncNoteAsync();
                    Status = "同步备忘录完成";
                }
                catch (Exception e)
                {
                    Status = "同步备忘录失败，原因:" + e.Message;
                }
            }).ContinueWith(t => 
            {
                BinderManager.noteBinder.Notes = new ObservableCollection<Note>(notes);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void SyncRecord()
        {
            List<Record> records = new List<Record>();
            Task.Run(async () =>
            {
                if (!isCrawlerInit)
                {
                    ch.InitHelper();
                    isCrawlerInit = true;
                }
                try
                {
                    Status = "正在同步录音";
                    records = await ch.SyncRecordAsync();
                    Status = "同步录音完成";
                }
                catch (Exception e)
                {
                    Status = "同步录音失败，原因:" + e.Message;
                }
            }).ContinueWith(t => 
            {
                BinderManager.recordBinder.Records = new ObservableCollection<Record>(records);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void SyncGps()
        {
            List<Gps> gpses = new List<Gps>();
            Task.Run(async () =>
            {
                if (!isCrawlerInit)
                {
                    ch.InitHelper();
                    isCrawlerInit = true;
                }
                try
                {
                    Status = "正在同步地址信息";
                    gpses = await ch.SyncLocationAsync();
                    Status = "同步地址信息完成";
                }
                catch (Exception e)
                {
                    Status = "同步地址信息失败，原因:" + e.Message;
                }
            }).ContinueWith(t => 
            {
                BinderManager.gpsBinder.Gpses = new ObservableCollection<Gps>(gpses);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void SyncFile(Model.Common.File file = null)
        {
            List<Model.Common.File> files = new List<Model.Common.File>();
            Task.Run(async () =>
            {
                if (!isCrawlerInit)
                {
                    ch.InitHelper();
                    isCrawlerInit = true;
                }
                try
                {
                    Status = "正在同步文件信息";
                    files = await ch.SyncFileAsync(file);
                    Status = "同步文件信息完成";
                }
                catch (Exception e)
                {
                    Status = "同步文件信息失败，原因:" + e.Message;
                }
            }).ContinueWith(t =>
            {
                if (file == null)
                    BinderManager.fileBinder.Files = new ObservableCollection<Model.Common.File>(files);
                else
                {
                    BinderManager.fileBinder.Files.Remove(file);
                    foreach (var i in files)
                        BinderManager.fileBinder.Files.Add(i);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void DownloadPicture(Picture picture)
        {
            if (picture.DownloadMutex)
            {
                Status = "上次下载尚未完成，请稍后再试";
                return;
            }

            Status = "正在下载图片";

            picture.DownloadMutex = true;
            Task.Run(async () =>
            {
                try
                {
                    string file = Setting.PictureFolder + picture.Name;
                    if(!System.IO.File.Exists(file))
                        await ch.DownloadPicture(picture);
                    Status = "下载完成";
                }
                catch (Exception e)
                {
                    Status = "下载失败：" + e.Message;
                }
            }).ContinueWith(t => {
                picture.DownloadMutex = false;
                string file = Setting.PictureFolder + picture.Name;
                if(System.IO.File.Exists(file))
                    System.Diagnostics.Process.Start(file);
            });
        }

        public void DownloadRecord(Record record)
        {
            if (record.DownloadMutex)
            {
                Status = "上次下载尚未完成，请稍后再试";
                return;
            }

            Status = "正在下载通话录音";

            record.DownloadMutex = true;
            Task.Run(async () =>
            {
                try
                {
                    string file = Setting.RecordFolder + record.Name.Replace("/", "_");
                    if (!System.IO.File.Exists(file))
                        await ch.DownloadRecord(record);
                    Status = "下载完成";
                }
                catch (Exception e)
                {
                    Status = "下载失败：" + e.Message;
                }
            }).ContinueWith(t =>
            {
                record.DownloadMutex = false;
                string file = Setting.RecordFolder + record.Name.Replace("/", "_");
                if (System.IO.File.Exists(file))
                    System.Diagnostics.Process.Start(file);
            });
        }

        public void DownloadFile(Model.Common.File file)
        {
            if (file.DownloadMutex)
            {
                Status = "该文件上次下载尚未完成，请稍后再试";
                return;
            }

            Status = "正在下载文件";

            file.DownloadMutex = true;
            Task.Run(async () =>
            {
                try
                {
                    string fileName = Setting.FileFolder + file.Name.Replace("/", "_");
                    if(!System.IO.File.Exists(fileName))
                        await ch.DownloadFile(file);
                    Status = "下载完成";
                }
                catch (Exception e)
                {
                    Status = "下载失败：" + e.Message;
                }
            }).ContinueWith(t => {
                file.DownloadMutex = false;
                string fileName = Setting.FileFolder + file.Name.Replace("/", "_");
                if(System.IO.File.Exists(fileName))
                    System.Diagnostics.Process.Start(fileName);
            });
        }

        public void DownloadNote(Note note)
        {
            if (note.DownloadMutex)
            {
                Status = "上次下载尚未完成，请稍后再试";
                return;
            }

            Status = "正在下载备忘录";

            note.DownloadMutex = true;
            Task.Run(async () =>
            {
                try
                {
                    string file = Setting.NoteFolder + note.Id + ".txt";
                    if(!System.IO.File.Exists(file))
                        await ch.DownloadNote(note);
                    Status = "下载完成";
                }
                catch (Exception e)
                {
                    Status = "下载失败：" + e.Message;
                }
            }).ContinueWith(t => {
                note.DownloadMutex = false;
                string file = Setting.NoteFolder + note.Id + ".txt";
                if(System.IO.File.Exists(file))
                    System.Diagnostics.Process.Start(file);
            });
        }

        public void DownloadThumbnail()
        {
            int totalCount = BinderManager.pictureBinder.Pictures.Count;
            int currentCount = BinderManager.pictureBinder.loadedCount;
            if (currentCount >= totalCount)
                return;

            if (LoadThumbnailMutex)
            {
                return;
            }
            Status = "正在加载图片";
            LoadThumbnailMutex = true;

            var tasks = new List<Task>();
            Task.Run(() =>
            {
                for (int i = currentCount; i < currentCount + 5 && i < totalCount; ++i)
                {
                    byte[] Thumbnail = new byte[0];
                    tasks.Add(ch.DownloadThumbnailAsync(BinderManager.pictureBinder.Pictures[i]));
                    BinderManager.pictureBinder.loadedCount++;
                }
                Task.WaitAll(tasks.ToArray(), 20000);
            }).ContinueWith(t =>
            {
                BinderManager.pictureBinder.OnCollectionChanged();
                LoadThumbnailMutex = false;
                Status = "图片加载完成";
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public bool DuringForensics()
        {
            return ForensicsMutex;
        }
        public void StartForensics()
        {
            Status = "开始取证";
            if (ForensicsMutex)
            {
                Status = "上次取证尚未完成，请稍后再试";
                return;
            }
            ForensicsMutex = true;

            Task.Run(async () =>
            {
                if (Setting.Provider == ServiceProvider.HuaWei)
                {
                    int cnt = 0;
                    while (!WebHelper.GetPictureDone)
                    {
                        Thread.Sleep(1000);
                        cnt += 1;
                        if (cnt == 30)
                        {
                            Status = "超时，确认是否登陆";
                            return;
                        }
                    }
                }
                try
                {
                    Status = "正在初始化爬虫";
                    ch.InitHelper();
                }
                catch (Exception)
                {
                    Status = "爬虫初始化失败，请检查是否登陆完成，并执行网页身份验证";
                    return;
                }

                try
                {
                    Status = "正在获取通讯录";
                    var contacts = await ch.SyncContactAsync();
                    XmlHelper.SaveContact(contacts);
                }
                catch (Exception e)
                {
                }

                try
                {
                    Status = "正在获取通话记录";
                    var callRecords = await ch.SyncCallRecordAsync();
                    XmlHelper.SaveCallRecord(callRecords);
                }
                catch (Exception e)
                {
                }

                try
                {
                    Status = "正在获取短信";
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
                        Status = string.Format("正在获取第{0}个录音，共{1}个", cnt, records.Count);
                        cnt += 1;
                        await ch.DownloadRecord(record);
                    }
                }
                catch (Exception e)
                {

                }

                try
                {
                    Status = "正在获取备忘录";
                    var notes = await ch.SyncNoteAsync();
                    XmlHelper.SaveNote(notes);
                    int cnt = 1;
                    foreach (Note note in notes)
                    {
                        Status = string.Format("正在获取第{0}条备忘录，共{1}个", cnt, notes.Count);
                        cnt += 1;
                        await ch.DownloadNote(note);
                    }
                }
                catch (Exception e)
                {

                }

                try
                {
                    Status = "正在获取文件";
                    var files = await ch.SyncFileAsync();
                    bool flag = false;
                    while (!flag)
                    {
                        flag = true;
                        for (int i = 0; i < files.Count; ++i)
                        {
                            var file = files[i];
                            if (file.Type == "folder")
                            {
                                files.AddRange(await ch.SyncFileAsync(file));
                                files.Remove(file);
                                flag = false;
                            }
                        }
                    }
                    int cnt = 1;
                    foreach (var file in files)
                    {
                        Status = string.Format("正在获取第{0}个文件，共{1}个", cnt, files.Count);
                        cnt += 1;
                        await ch.DownloadFile(file);
                    }
                }
                catch (Exception e)
                {

                }

                try
                {
                    Status = "正在获取gps";
                    var gpses = await ch.SyncLocationAsync();
                    XmlHelper.SaveGps(gpses);
                }
                catch (Exception e)
                {

                }

                try
                {
                    var pictures = await ch.SyncPictureAsync();
                    XmlHelper.SavePicture(pictures);
                    int cnt = 1;
                    foreach (Picture picture in pictures)
                    {
                        Status = string.Format("正在获取第{0}张图片，共{1}张", cnt, pictures.Count);
                        cnt += 1;
                        await ch.DownloadPicture(picture);
                    }
                }
                catch (Exception e)
                {

                }

                try
                {
                    Status = "正在将xml导入到数据库";
                    XmlToDb xtd = new XmlToDb();
                    xtd.Convert(Setting.XmlFolder, Setting.DbFile);
                }
                catch
                { }

                Status = "取证完成";
            }).ContinueWith(t =>
            {
                ForensicsMutex = false;
            });
        }
    }
}
