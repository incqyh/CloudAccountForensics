using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CAF.Model;
using CAF.Model.Common;
using CAF.Model.DataObject;
using Newtonsoft.Json;

namespace CAF.Model.CloudHelper.XiaoMi
{
    partial class XiaoMiHelper
    {
        public void CallRecordParser(string rawJson)
        {
            dynamic callRecordJson;
            try
            {
                callRecordJson = Newtonsoft.Json.Linq.JToken.Parse(rawJson) as dynamic;
            }
            catch (Exception)
            {
                throw new Exception("通话记录解析出错，请尝试重新获取数据，请检查登陆是否失效");
            }

            if (runtimeData.isFirstTime)
            {
                DataManager.CallRecord.Clear();
                runtimeData.isFirstTime = false;
            }
            try
            {
                runtimeData.lastPage = callRecordJson.data.lastPage;
                runtimeData.syncTag = callRecordJson.data.syncTag;

                foreach (var item in callRecordJson.data.entries)
                {
                    string phoneNumber = item["number"];

                    string direction = "";
                    if (item["type"] == "incoming")
                        direction = "来电";
                    else if (item["type"] == "outgoing")
                        direction = "去电";
                    else if (item["type"] == "missed")
                        direction = "未接";
                    else if (item["type"] == "newContact")
                        direction = "新建联系人";

                    UInt32 seconds = (Int32)item["duration"] >= 0 ? (UInt32)item["duration"] : 0;
                    TimeSpan lastTime = TimeConverter.ToTimeSpan(seconds);

                    UInt64 timeStamp = item["date"];
                    DateTime phoneTime = TimeConverter.UInt64ToDateTime(timeStamp);

                    DataManager.CallRecord.Rows.Add(phoneNumber, direction, phoneTime, lastTime);
                }
            }
            catch (Exception)
            {
                throw new Exception("通话记录数据格式出错，可能云服务网页有更新");
            }
        }

        public void ContactsParser(string rawJson)
        {
            dynamic contactsJson;
            try
            {
                contactsJson = Newtonsoft.Json.Linq.JToken.Parse(rawJson) as dynamic;
            }
            catch (Exception)
            {
                throw new Exception("通讯录解析出错，请尝试重新获取数据");
            }

            if (runtimeData.isFirstTime)
            {
                DataManager.Contacts.Clear();
                runtimeData.isFirstTime = false;
            }
            try
            {
                runtimeData.lastPage = contactsJson.data.lastPage;
                runtimeData.syncIgnoreTag = contactsJson.data.syncIgnoreTag;
                runtimeData.syncTag = contactsJson.data.syncTag;

                foreach (var item in contactsJson.data.content)
                {
                    var content = item.Value["content"];

                    string name = content["displayName"];

                    string birthday = "";
                    if (content["events"] != null)
                    {
                        foreach (var i in content["events"])
                        {
                            if ((string)i["type"] == "birthday")
                                birthday = (string)i["value"];
                        }
                    }

                    List<KeyValuePair<string, string>> phoneNumber = new List<KeyValuePair<string, string>>();
                    if (content["phoneNumbers"] != null)
                        foreach (var i in content["phoneNumbers"])
                        {
                            phoneNumber.Add(new KeyValuePair<string, string>((string)i["type"], (string)i["value"]));
                        }

                    List<KeyValuePair<string, string>> email = new List<KeyValuePair<string, string>>();
                    if (content["emails"] != null)
                        foreach (var i in content["emails"])
                        {
                            email.Add(new KeyValuePair<string, string>((string)i["type"], (string)i["value"]));
                        }

                    List<KeyValuePair<string, string>> address = new List<KeyValuePair<string, string>>();
                    if (content["address"] != null)
                        foreach (var i in content["addresses"])
                        {
                            address.Add(new KeyValuePair<string, string>((string)i["type"], (string)i["value"]));
                        }

                    List<KeyValuePair<string, string>> imAccount = new List<KeyValuePair<string, string>>();
                    if (content["ims"] != null)
                        foreach (var i in content["ims"])
                        {
                            imAccount.Add(new KeyValuePair<string, string>((string)i["type"], (string)i["value"]));
                        }

                    DataManager.Contacts.Rows.Add(name, birthday, phoneNumber, email, address, imAccount);
                }
            }
            catch (Exception)
            {
                throw new Exception("通讯录数据格式出错，可能云服务网页有更新");
            }
        }

        public void MemoParser(string rawJson)
        {
            throw new NotImplementedException();
        }

        public void MessageParser(string rawJson)
        {
            dynamic messageJson;
            try
            {
                messageJson = Newtonsoft.Json.Linq.JToken.Parse(rawJson) as dynamic;
            }
            catch (Exception)
            {
                throw new Exception("短信记录解析出错，请尝试重新获取数据");
            }

            // if (runtimeData.isFirstTime)
            // {
                DataManager.Message.Clear();
            //     runtimeData.isFirstTime = false;
            // }
            try
            {
                // runtimeData.lastPage = messageJson.data.watermark.lastPage;
                // runtimeData.syncIgnoreTag = messageJson.data.watermark.syncIgnoreTag;
                // runtimeData.syncTag = messageJson.data.watermark.syncTag;
                foreach (var item in messageJson.data.entries)
                {
                    string phoneNumber = item.entry.recipients;

                    string direction = "";
                    if (item.entry.folder == "0")
                        direction = "接收";
                    else if (item.entry.folder == "1")
                        direction = "发送";

                    string content = item.entry.snippet;
                    UInt64 timeStamp = item.entry.localTime;
                    DateTime messageTime = TimeConverter.UInt64ToDateTime(timeStamp);

                    DataManager.Message.Rows.Add(phoneNumber, direction, content, messageTime);
                }
            }
            catch (Exception)
            {
                throw new Exception("短信记录数据格式出错，可能云服务网站有更新");
            }
        }
    }
}
