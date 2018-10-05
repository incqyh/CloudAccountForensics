using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CAF.Model;
using CAF.Model.Database;
using CAF.Model.DataObject;
using CAF.Model.Fetch;
using CAF.Model.Parser;
using System.ComponentModel;

using CAF.Model.Common;

namespace CAF.View.Common
{
    class VMManager : INotifyPropertyChanged 
    {
        public FetchHelper fh;
        public DBHelper dbh;
        public ParserHelper ph;

        public string MainUrl { get; set; } = "";
        public bool IsLogin { get; set; }
        bool UpdateDBMutex { get; set; } = false;
        bool ReadFromDBMutex { get; set; } = false;
        bool ReadFromWebMutex { get; set; } = false;

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
                    MainUrl = "https://cloud.huawei.com";
                    break;
                case "小米":
                    Setting.Provider = ServiceProvider.XiaoMi;
                    MainUrl = "https://i.mi.com";
                    break;
            }

            DataManager.Contacts.Clear();
            DataManager.CallRecord.Clear();
            DataManager.Message.Clear();

            fh = new FetchHelper();
            dbh = new DBHelper();
            ph = new ParserHelper();
        }

        public void UpdateDB()
        {
            if (Setting.Provider == ServiceProvider.Invalid)
            {
                Status = "请先选择服务商";
                return;
            }
            
            if (UpdateDBMutex)
            {
                Status = "上次保存尚未完成，请稍后再试";
                return;
            }

            Status = "正在更新本地数据库...";

            UpdateDBMutex = true;
            Task.Run(() =>
            {
                try
                {
                    dbh.CallRecordDBUpdate();
                    Status = "保存通话记录成功";
                }
                catch (Exception e)
                {
                    Status = "保存通话记录失败，原因：" + e.Message;
                }

                try
                {
                    dbh.MessageDBUpdate();
                    Status = "保存短信成功";
                }
                catch (Exception e)
                {
                    Status = "保存短信失败，原因：" + e.Message;
                }

                try
                {
                    dbh.ContactsDBUpdate();
                    Status = "保存联系人成功";
                }
                catch (Exception e)
                {
                    Status = "保存联系人失败，原因：" + e.Message;
                }
            }).ContinueWith(t => { UpdateDBMutex = false; });
        }

        public void ReadFromDB()
        {
            if (Setting.Provider == ServiceProvider.Invalid)
            {
                Status = "请先选择服务商";
                return;
            }

            if (ReadFromDBMutex)
            {
                Status = "上次读取尚未完成，请稍后再试";
                return;
            }

            Status = "正在从本地数据库读取数据";

            ReadFromDBMutex = true;
            Task.Run(() =>
            {
                try
                {
                    dbh.CallRecordRead();
                    Status = "读取通话记录成功";
                }
                catch (Exception e)
                {
                    Status = "读取通话记录失败";
                }

                try
                {
                    dbh.MessageRead();
                    Status = "读取短信成功";
                }
                catch (Exception e)
                {
                    Status = "读取短信失败";
                }

                try
                {
                    dbh.ContactsRead();
                    Status = "读取联系人成功";
                }
                catch (Exception e)
                {
                    Status = "读取联系人失败";
                }
            }).ContinueWith(t => { ReadFromDBMutex = false; });
        }

        public void ReadFromWeb()
        {
            if (Setting.Provider == ServiceProvider.Invalid)
            {
                Status = "请先选择服务商";
                return;
            }

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
                    await fh.fetcher.InitFetcherAsync();
                    Status = "初始化网络爬虫成功";
                }
                catch (Exception)
                {
                    Status = "爬虫初始化失败，请检查是否登陆完成，并执行网页身份验证";
                    return;
                }

                try
                {
                    string rawJson = await fh.fetcher.FetchCallRecordJsonAsync();
                    Status = "获取通话记录完成";

                    ph.parser.CallRecordParser(rawJson);
                    Status = "解析通话记录完成";
                }
                catch (Exception e)
                {
                    Status = "获取/解析通话记录失败，原因:" + e.Message;
                }

                try
                {
                    string rawJson = await fh.fetcher.FetchMessageJsonAsync();
                    Status = "获取短信完成";

                    ph.parser.MessageParser(rawJson);
                    Status = "解析短信数据完成";
                }
                catch (Exception e)
                {
                    Status = "获取/解析短信失败，原因:" + e.Message;
                }

                try
                {
                    string rawJson = await fh.fetcher.FetchContactsJsonAsync();
                    Status = "获取联系人完成";

                    ph.parser.ContactsParser(rawJson);
                    Status = "解析联系人完成";
                }
                catch (Exception e)
                {
                    Status = "获取/解析联系人失败，原因:" + e.Message;
                }
            }).ContinueWith(t => {ReadFromWebMutex = false;});
            
        }
    }
}
