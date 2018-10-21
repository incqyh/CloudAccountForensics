using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CAF.Model.Common;
using Cloud;

namespace CAF.Model.CloudHelper.HuaWei
{
    partial class HuaWeiHelper
    {
        Dictionary<int, string> phoneNumberTypeMap;
        Dictionary<int, string> emailTypeMap;
        Dictionary<int, string> addressTypeMap;
        Dictionary<int, string> imAccountTypeMap;

        void InitParser()
        {
            phoneNumberTypeMap = new Dictionary<int, string>
            {
                { 1, "手机" },
                { 2, "住宅" },
                { 3, "单位" },
                { 14, "总机" },
                { 5, "住宅传真" },
                { 6, "单位传真" },
                { 7, "寻呼机" },
                { 8, "其他" }
            };

            emailTypeMap = new Dictionary<int, string>
            {
                { 1, "私人" },
                { 2, "单位" },
                { 3, "其他" },
            };

            addressTypeMap = new Dictionary<int, string>
            {
                { 1, "住宅" },
                { 2, "单位" },
                { 3, "其他" },
            };

            imAccountTypeMap = new Dictionary<int, string>
            {
                { 1, "aim" },
                { 2, "windows live" },
                { 3, "yahoo" },
                { 4, "skype" },
                { 5, "qq"},
                { 6, "环聊" },
                { 7, "icq" },
                { 8, "jabber" },
            };
        }

        void ParseCallRecord(string rawJson)
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
                DataManager.CallRecords.Clear();
                runtimeData.isFirstTime = false;
            }

            try
            {
                if (!callRecordJson.CallLogInfos.HasValues)
                    runtimeData.isEnd = true;
                foreach (var item in callRecordJson.CallLogInfos)
                {
                    CallRecord callRecord = new CallRecord();
                    callRecord.PhoneNumber = item.PeerAddress;

                    callRecord.Direction = "";
                    if (item.Direction == "1")
                        callRecord.Direction = "来电";
                    else if (item.Direction == "2")
                        callRecord.Direction = "去电";
                    else if (item.Direction == "3")
                        callRecord.Direction = "未接";

                    UInt32 seconds = item.Duration;
                    callRecord.LastTime = TimeConverter.ToTimeSpan(seconds);

                    UInt64 timeStamp = item.StartTime;
                    callRecord.PhoneTime = TimeConverter.UInt64ToDateTime(timeStamp);

                    DataManager.CallRecords.Add(callRecord);
                }
            }
            catch (Exception)
            {
                throw new Exception("通话记录数据格式出错，可能云服务网页有更新");
            }
        }

        void ParseContacts(string rawJson)
        {
            byte[] raw = Convert.FromBase64String(rawJson);
            AllContactsRespVo contactsJson;

            try
            {
                contactsJson = AllContactsRespVo.Parser.ParseFrom(raw);
            }
            catch (Exception)
            {
                throw new Exception("通讯录解析出错，请尝试重新获取数据，请检查登陆是否失效");
            }

            DataManager.Contacts.Clear();
            try
            {
                foreach (var item in contactsJson.ContactList)
                {
                    Contact contact = new Contact();
                    contact.Name = item.FName;
                    contact.Birthday = item.BDay;

                    foreach (var i in item.TelList)
                        contact.PhoneNumber.Add(new KeyValuePair<string, string>(phoneNumberTypeMap[i.Type], i.Value));

                    foreach (var i in item.EmailList)
                        contact.Email.Add(new KeyValuePair<string, string>(emailTypeMap[i.Type], i.Value));

                    foreach (var i in item.AddressList)
                        contact.Address.Add(new KeyValuePair<string, string>(addressTypeMap[i.Type], i.City));

                    foreach (var i in item.MsgList)
                        contact.ImAccount.Add(new KeyValuePair<string, string>(imAccountTypeMap[i.Type], i.Value));

                    DataManager.Contacts.Add(contact);
                }
            }
            catch(Exception)
            {
                throw new Exception("通讯录数据格式出错，可能云服务网页有更新");
            }
        }

        void ParseMessage(string rawJson)
        {
            dynamic messageJson;
            try
            {
                messageJson = Newtonsoft.Json.Linq.JToken.Parse(rawJson) as dynamic;
            }
            catch(Exception)
            {
                throw new Exception("短信记录解析出错，请尝试重新获取数据，请检查登陆是否失效");
            }

            if (runtimeData.isFirstTime)
            {
                DataManager.Messages.Clear();
                runtimeData.isFirstTime = false;
            }
            try
            {
                runtimeData.totalCount = messageJson.TotalCount;
                foreach (var item in messageJson.SMSWithIDs)
                {
                    Message message = new Message();
                    message.PhoneNumber = item.SMSInfo.PeerAddress;

                    message.Direction = "";
                    if (item.SMSInfo.Direction == "1")
                        message.Direction = "接收";
                    else if (item.SMSInfo.Direction == "2")
                        message.Direction = "发送";

                    message.Content = item.SMSInfo.Text;
                    UInt64 timeStamp = item.SMSInfo.ActivityTime;
                    message.MessageTime = TimeConverter.UInt64ToDateTime(timeStamp);

                    DataManager.Messages.Add(message);
                }
            }
            catch(Exception)
            {
                throw new Exception("短信记录数据格式出错，可能云服务网站有更新");
            }
        }
    }
}
