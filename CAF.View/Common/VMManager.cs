using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CAF.Model;
using CAF.Model.DataObject;
using System.ComponentModel;

using CAF.Model.Common;
using CAF.Model.CloudHelper;

namespace CAF.View.Common
{
    class VMManager : INotifyPropertyChanged 
    {
        CloudHelper ch;

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
                    break;
                case "小米":
                    Setting.Provider = ServiceProvider.XiaoMi;
                    break;
            }

            DataManager.Contacts.Clear();
            DataManager.CallRecord.Clear();
            DataManager.Message.Clear();

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

                try
                {
                    await ch.SyncCallRecordAsync();
                    Status = "同步通话记录完成";
                }
                catch (Exception e)
                {
                    Status = "同步通话记录失败，原因:" + e.Message;
                }

                try
                {
                    await ch.SyncMessageAsync();
                    Status = "同步短信完成";
                }
                catch (Exception e)
                {
                    Status = "同步短信失败，原因:" + e.Message;
                }

                try
                {
                    await ch.SyncContactsAsync();
                    Status = "同步联系人完成";
                }
                catch (Exception e)
                {
                    Status = "同步联系人失败，原因:" + e.Message;
                }
            }).ContinueWith(t => {ReadFromWebMutex = false;});
            
        }
    }
}
