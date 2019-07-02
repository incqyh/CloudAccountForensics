using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CAF.Model.Common;
using System.Text.RegularExpressions;

namespace CAF.Model.CloudHelper.HuaWei
{
    partial class HuaWeiHelper
    {
        Dictionary<int, string> phoneNumberMap;
        Dictionary<int, string> emailMap;
        Dictionary<int, string> addressMap;
        Dictionary<int, string> imAccountMap;

        /// <summary>
        /// 初始化一些解析时候使用的参数
        /// 华为使用一些编号来指代不同类型的数据
        /// </summary>
        void InitParser()
        {
            phoneNumberMap = new Dictionary<int, string>
            {
                { 0, "自定义" },
                { 1, "手机" },
                { 2, "住宅" },
                { 3, "单位" },
                { 14, "总机" },
                { 5, "住宅传真" },
                { 6, "单位传真" },
                { 7, "寻呼机" },
                { 8, "其他" }
            };

            emailMap = new Dictionary<int, string>
            {
                { 0, "自定义" },
                { 1, "私人" },
                { 2, "单位" },
                { 3, "其他" },
            };

            addressMap = new Dictionary<int, string>
            {
                { 0, "自定义" },
                { 1, "住宅" },
                { 2, "单位" },
                { 3, "其他" },
            };

            imAccountMap = new Dictionary<int, string>
            {
                { 0, "自定义" },
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

        public List<CallRecord> ParseCallRecord(string rawJson)
        {
            List<CallRecord> callRecords = new List<CallRecord>();

            dynamic callRecordJson;
            try
            {
                callRecordJson = Newtonsoft.Json.Linq.JToken.Parse(rawJson) as dynamic;
            }
            catch (Exception)
            {
                throw new Exception("通话记录解析出错，请尝试重新获取数据，请检查登陆是否失效");
            }

            try
            {
                // 从当前解析出的数据可以找到数据是否已全部获取完毕
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

                    callRecords.Add(callRecord);
                }
            }
            catch (Exception)
            {
                throw new Exception("通话记录数据格式出错，可能云服务网页有更新");
            }

            return callRecords;
        }

        public List<Contact> ParseContact(List<string> rawJson)
        {
            List<Contact> contacts = new List<Contact>();

            byte[] raw = Convert.FromBase64String(rawJson[0]);
            AllGroupsRespVo groupInfo;
            try
            {
                // 这是由google protobuf库自动生成的一个类
                groupInfo = AllGroupsRespVo.Parser.ParseFrom(raw);
            }
            catch (Exception)
            {
                throw new Exception("通讯录解析出错，请尝试重新获取数据，请检查登陆是否失效");
            }

            Dictionary<string, string> groupMap = new Dictionary<string, string>();
            try
            {
                foreach (var group in groupInfo.GroupList)
                {
                    groupMap.Add(group.GroupId, group.GroupName);
                }
            }
            catch(Exception)
            {
                throw new Exception("通讯录数据格式出错，可能云服务网页有更新");
            }

            raw = Convert.FromBase64String(rawJson[1]);
            AllContactsRespVo contactsInfo;

            try
            {
                // 这是由google protobuf库自动生成的一个类
                contactsInfo = AllContactsRespVo.Parser.ParseFrom(raw);
            }
            catch (Exception)
            {
                throw new Exception("通讯录解析出错，请尝试重新获取数据，请检查登陆是否失效");
            }

            try
            {
                foreach (var item in contactsInfo.ContactList)
                {
                    Contact contact = new Contact();
                    contact.Name = item.FName;
                    contact.Birthday = item.BDay;

                    contact.Company = "";
                    contact.Title = "";
                    foreach (var i in item.OrganizeList)
                    {
                        contact.Company = i.Org;
                        contact.Title = i.Title;
                    }

                    foreach (var i in item.GroupIdList)
                    {
                        if (groupMap.TryGetValue(i, out string value))
                        {
                            contact.Group.Add(value);
                        }
                    }

                    foreach (var i in item.TelList)
                    {
                        if (phoneNumberMap.TryGetValue(i.Type, out string type))
                        {
                            string value = i.Value;
                            contact.PhoneNumber.Add(new KeyValuePair<string, string>(type, value));
                        }
                    }

                    foreach (var i in item.EmailList)
                    {
                        if (emailMap.TryGetValue(i.Type, out string type))
                        {
                            string value = i.Value;
                            contact.Email.Add(new KeyValuePair<string, string>(type, value));
                        }
                    }

                    foreach (var i in item.AddressList)
                    {
                        if (addressMap.TryGetValue(i.Type, out string type))
                        {
                            string value = i.Country + i.Province + i.City + i.Street + i.PostalCode;
                            contact.Address.Add(new KeyValuePair<string, string>(type, value));
                        }
                    }

                    foreach (var i in item.MsgList)
                    {
                        if (imAccountMap.TryGetValue(i.Type, out string type))
                        {
                            string value = i.Value;
                            contact.ImAccount.Add(new KeyValuePair<string, string>(type, value));
                        }
                    }

                    contacts.Add(contact);
                }
            }
            catch(Exception)
            {
                throw new Exception("通讯录数据格式出错，可能云服务网页有更新");
            }

            return contacts;
        }

        public List<Message> ParseMessage(string rawJson)
        {
            List<Message> messages = new List<Message>();

            dynamic messageJson;
            try
            {
                messageJson = Newtonsoft.Json.Linq.JToken.Parse(rawJson) as dynamic;
            }
            catch(Exception)
            {
                throw new Exception("短信记录解析出错，请尝试重新获取数据，请检查登陆是否失效");
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

                    messages.Add(message);
                }
            }
            catch(Exception)
            {
                throw new Exception("短信记录数据格式出错，可能云服务网站有更新");
            }

            return messages;
        }

        public List<Note> ParseNote(List<string> rawJson)
        {
            List<Note> notes = new List<Note>();
            string ctagNoteTag;
            string ctagNoteInfo;

            dynamic noteJson;
            try
            {
                noteJson = Newtonsoft.Json.Linq.JToken.Parse(rawJson[0]) as dynamic;
                ctagNoteTag = noteJson.ctagNoteTag;
                noteJson = Newtonsoft.Json.Linq.JToken.Parse(rawJson[1]) as dynamic;
                ctagNoteInfo = noteJson.ctagNoteInfo;
            }
            catch (Exception)
            {
                throw new Exception("备忘录解析出错，请尝试重新获取数据");
            }

            try
            {
                foreach (var item in noteJson.rspInfo)
                {
                    Note note = new Note();

                    string pattern = @"({.*})";
                    string tmp = item["data"];
                    string data = Regex.Match(tmp, pattern).Result("$1");
                    var json = Newtonsoft.Json.Linq.JToken.Parse(data) as dynamic;
                    note.Snippet = json.title;
                    note.Id = json.guid;
                    note.ctagNoteInfo = ctagNoteInfo;
                    note.ctagNoteTag = ctagNoteTag;

                    ulong timeStamp = json["modified"];
                    note.ModifyTime = TimeConverter.UInt64ToDateTime(timeStamp);
                    note.LocalUrl = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(),
                        Setting.NoteFolder, note.Id + ".txt");

                    notes.Add(note);
                }
            }
            catch (Exception)
            {
                throw new Exception("备忘录格式出错，可能云服务网页有更新");
            }

            return notes;
        }

        public List<Record> ParseRecord(string rawJson)
        {
            List<Record> records = new List<Record>();

            dynamic recordJson;
            try
            {
                recordJson = Newtonsoft.Json.Linq.JToken.Parse(rawJson) as dynamic;
            }
            catch (Exception)
            {
                throw new Exception("录音信息解析出错，请尝试重新获取数据");
            }

            try
            {
                foreach (var item in recordJson.fileList)
                {
                    Record record = new Record();
                    record.Name = item["name"];
                    record.ModifyTime = item["modifyTime"];
                    record.CreateTime = item["createTime"];
                    record.LocalUrl = System.IO.Path.Combine(Environment.CurrentDirectory, 
                        Setting.RecordFolder, record.Name.Replace("/", "_"));

                    records.Add(record);
                }
            }
            catch (Exception)
            {
                throw new Exception("录音信息格式出错，可能云服务网页有更新");
            }

            return records;
        }

        public List<Gps> ParseGps(string rawJson)
        {
            List<Gps> gpses = new List<Gps>();
            dynamic gpsJson;
            try
            {
                gpsJson = Newtonsoft.Json.Linq.JToken.Parse(rawJson) as dynamic;
            }
            catch (Exception)
            {
                throw new Exception("gps信息解析出错，请尝试重新获取数据");
            }

            try
            {
                Gps gps = new Gps();
                gps.Latitude = gpsJson["info"]["latitude"];
                gps.Longitude = gpsJson["info"]["longitude"];
                gps.Accuracy = gpsJson["info"]["accuracy"];
                ulong timeStamp = gpsJson["executeTime"];
                gps.Time = TimeConverter.UInt64ToDateTime(timeStamp);

                gpses.Add(gps);
            }
            catch (Exception)
            {
                throw new Exception("gps信息格式出错，可能云服务网页有更新");
            }

            return gpses;
        }

        List<Picture> ParsePicture(string rawJson)
        {
            List<Picture> pictures = new List<Picture>();
            dynamic pictureJson;
            try
            {
                pictureJson = Newtonsoft.Json.Linq.JToken.Parse(rawJson) as dynamic;
            }
            catch (Exception)
            {
                throw new Exception("图片信息解析出错，请尝试重新获取数据");
            }

            try
            {
                foreach (var i in pictureJson.urlList)
                {
                    Picture picture = new Picture();
                    picture.AlbumId = i["albumId"];
                    picture.BigThumbnailUrl = i["url"];
                    picture.UniqueId = i["uniqueId"];
                    pictures.Add(picture);
                }
            }
            catch (Exception)
            {
                throw new Exception("图片信息格式出错，可能云服务网页有更新");
            }
            return pictures;
        }
    }
}
