using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CAF.Model.Common;
using Cloud;
using System.Text.RegularExpressions;

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

        public List<Contact> ParseContact(string rawJson)
        {
            List<Contact> contacts = new List<Contact>();

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

        public List<Picture> ParsePicture(string rawJson)
        {
            List<Picture> pictures = new List<Picture>();

            dynamic pictureJson;
            try
            {
                pictureJson = Newtonsoft.Json.Linq.JToken.Parse(rawJson) as dynamic;
            }
            catch (Exception)
            {
                throw new Exception("图片信息录解析出错，请尝试重新获取数据");
            }

            try
            {
                foreach (var item in pictureJson.data.galleries)
                {
                    Picture picture = new Picture();
                    picture.Name = item["fileName"];
                    if (item["geoInfo"] != null)
                    {
                        picture.Address = item["geoInfo"]["address"]["addressLines"][0];
                        picture.Gps = item["geoInfo"]["gps"];
                        picture.IsAccurate = item["geoInfo"]["isAccurate"];
                    }
                    if (item["exifInfo"]["dateTime"] != null)
                        picture.Time = item["exifInfo"]["dateTime"];
                    if (item["exifInfo"]["model"] != null)
                        picture.PhoneType = item["exifInfo"]["model"];
                    picture.BigThumbnailUrl = item["bigThumbnailInfo"]["data"];
                    picture.Id = item["id"];

                    pictures.Add(picture);
                }
            }
            catch (Exception)
            {
                throw new Exception("图片信息格式出错，可能云服务网页有更新");
            }

            return pictures;
        }

        public List<Note> ParseNote(string rawJson)
        {
            List<Note> notes = new List<Note>();

            dynamic noteJson;
            try
            {
                noteJson = Newtonsoft.Json.Linq.JToken.Parse(rawJson) as dynamic;
            }
            catch (Exception)
            {
                throw new Exception("备忘录解析出错，请尝试重新获取数据");
            }

            try
            {
                foreach (var item in noteJson.data.entries)
                {
                    Note note = new Note();
                    note.Snippet = item["snippet"];
                    ulong timeStamp = item["modifyDate"];
                    note.ModifyTime = TimeConverter.UInt64ToDateTime(timeStamp);
                    timeStamp = item["createDate"];
                    note.CreateTime = TimeConverter.UInt64ToDateTime(timeStamp);
                    note.Id = item["id"];

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
                foreach (var item in recordJson.data.list)
                {
                    Record record = new Record();

                    string name = item["name"];
                    string pattern = @".*\.mp3";
                    record.Name = Regex.Match(name, pattern).Result("$0");

                    ulong timeStamp = item["modify_time"];
                    record.ModifyTime = TimeConverter.UInt64ToDateTime(timeStamp);
                    timeStamp = item["create_time"];
                    record.CreateTime = TimeConverter.UInt64ToDateTime(timeStamp);
                    record.Id = item["id"];

                    records.Add(record);
                }
            }
            catch (Exception)
            {
                throw new Exception("录音信息格式出错，可能云服务网页有更新");
            }

            return records;
        }

        public List<File> ParseFile(string rawJson)
        {
            List<File> files = new List<File>();

            dynamic fileJson;
            try
            {
                fileJson = Newtonsoft.Json.Linq.JToken.Parse(rawJson) as dynamic;
            }
            catch (Exception)
            {
                throw new Exception("文件信息解析出错，请尝试重新获取数据");
            }

            try
            {
                foreach (var item in fileJson.data.list)
                {
                    File file = new File();
                    file.Name = item["name"];
                    ulong timeStamp = item["modifyTime"];
                    file.ModifyTime = TimeConverter.UInt64ToDateTime(timeStamp);
                    timeStamp = item["createTime"];
                    file.CreateTime = TimeConverter.UInt64ToDateTime(timeStamp);
                    file.Size = item["size"];
                    file.Type = item["type"];
                    file.Id = item["id"];

                    files.Add(file);
                }
            }
            catch (Exception)
            {
                throw new Exception("文件信息格式出错，可能云服务网页有更新");
            }

            return files;
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
                foreach (var item in gpsJson.data.devices)
                {
                    Gps gps = new Gps();
                    gps.Imei = item["imei"];
                    ulong timeStamp = item["lastLocationReceipt"]["infoTime"];
                    gps.Time = TimeConverter.UInt64ToDateTime(timeStamp);
                    gps.Latitude = item["lastLocationReceipt"]["gpsInfo"]["latitude"];
                    gps.Longitude = item["lastLocationReceipt"]["gpsInfo"]["longitude"];
                    gps.Accuracy = item["lastLocationReceipt"]["gpsInfo"]["accuracy"];

                    gpses.Add(gps);
                }
            }
            catch (Exception)
            {
                throw new Exception("gps信息格式出错，可能云服务网页有更新");
            }

            return gpses;
        }
    }
}
