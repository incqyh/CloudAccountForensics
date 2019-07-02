using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using CAF.Model;
using CAF.Model.Common;
using Newtonsoft.Json;

namespace CAF.Model.CloudHelper.XiaoMi
{
    partial class XiaoMiHelper
    {
        Dictionary<string, string> phoneNumberMap;
        Dictionary<string, string> emailMap;
        Dictionary<string, string> addressMap;

        void InitParser()
        {
            phoneNumberMap = new Dictionary<string, string>
                {
                    { "mobile", "手机" },
                    { "work", "单位" },
                    { "home", "住宅" },
                    { "main", "总机" },
                    { "faxWork", "单位传真" },
                    { "faxHome", "家用传真" },
                    { "paper", "寻呼机" },
                    { "other", "其他" }
                };

            emailMap = new Dictionary<string, string>
                {
                    { "work", "单位" },
                    { "home", "家用" },
                    { "other", "其他" }
                };

            addressMap = new Dictionary<string, string>
                {
                    { "work", "单位" },
                    { "home", "家用" },
                    { "other", "其他" }
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
                runtimeData.lastPage = callRecordJson.data.lastPage;
                runtimeData.syncTag = callRecordJson.data.syncTag;

                foreach (var item in callRecordJson.data.entries)
                {
                    CallRecord callRecord = new CallRecord();
                    callRecord.PhoneNumber = item["number"];

                    callRecord.Direction = "";
                    if (item["type"] == "incoming")
                        callRecord.Direction = "来电";
                    else if (item["type"] == "outgoing")
                        callRecord.Direction = "去电";
                    else if (item["type"] == "missed")
                        callRecord.Direction = "未接";
                    else if (item["type"] == "newContact")
                        callRecord.Direction = "新建联系人";

                    UInt32 seconds = (Int32)item["duration"] >= 0 ? (UInt32)item["duration"] : 0;
                    callRecord.LastTime = TimeConverter.ToTimeSpan(seconds);

                    UInt64 timeStamp = item["date"];
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

            dynamic contactsJson;
            try
            {
                contactsJson = Newtonsoft.Json.Linq.JToken.Parse(rawJson) as dynamic;
            }
            catch (Exception)
            {
                throw new Exception("通讯录解析出错，请尝试重新获取数据");
            }

            try
            {
                runtimeData.lastPage = contactsJson.data.lastPage;
                runtimeData.syncIgnoreTag = contactsJson.data.syncIgnoreTag;
                runtimeData.syncTag = contactsJson.data.syncTag;

                Dictionary<string, string> groupMap = new Dictionary<string, string>();
                foreach (var item in contactsJson.data.group)
                {
                    var content = item["content"];
                    groupMap.Add((string)content["id"], (string)content["title"]);
                }

                foreach (var item in contactsJson.data.content)
                {
                    Contact contact = new Contact();
                    var content = item.Value["content"];

                    contact.Name = content["displayName"];

                    contact.Birthday = "";
                    if (content["events"] != null)
                    {
                        foreach (var i in content["events"])
                        {
                            if ((string)i["type"] == "birthday")
                                contact.Birthday = (string)i["value"];
                        }
                    }

                    contact.Company = "";
                    contact.Title = "";
                    if (content["organizations"] != null)
                    {
                        foreach (var i in content["organizations"])
                        {
                            contact.Company = (string)i["company"];
                            contact.Title = (string)i["title"];
                        }
                    }

                    if (content["groups"] != null)
                    {
                        foreach (var i in content["groups"])
                        {
                            groupMap.TryGetValue((string)i["value"], out string group);
                            contact.Group.Add(group);
                        }
                    }

                    if (content["phoneNumbers"] != null)
                    {
                        foreach (var i in content["phoneNumbers"])
                        {
                            if (phoneNumberMap.TryGetValue((string)i["type"], out string type))
                            {
                                string value = (string)i["value"];
                                contact.PhoneNumber.Add(new KeyValuePair<string, string>(type, value));
                            }
                        }
                    }

                    if (content["emails"] != null)
                    {
                        foreach (var i in content["emails"])
                        {
                            if (emailMap.TryGetValue((string)i["type"], out string type))
                            {
                                string value = (string)i["value"];
                                contact.Email.Add(new KeyValuePair<string, string>(type, value));
                            }
                        }
                    }

                    if (content["addresses"] != null)
                    {
                        foreach (var i in content["addresses"])
                        {
                            if (addressMap.TryGetValue((string)i["type"], out string type))
                            {
                                string value = (string)i["formatted"];
                                contact.Address.Add(new KeyValuePair<string, string>(type, value));
                            }
                        }
                    }

                    if (content["ims"] != null)
                    {
                        foreach (var i in content["ims"])
                        {
                            contact.ImAccount.Add(new KeyValuePair<string, string>((string)i["type"], (string)i["value"]));
                        }
                    }

                    contacts.Add(contact);
                }
            }
            catch (Exception)
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
            catch (Exception)
            {
                throw new Exception("短信记录解析出错，请尝试重新获取数据");
            }

            try
            {
                foreach (var item in messageJson.data.entries)
                {
                    Message message = new Message();
                    message.PhoneNumber = item.address;

                    message.Direction = "";
                    if (item.folder == "0")
                        message.Direction = "接收";
                    else if (item.folder == "1")
                        message.Direction = "发送";

                    message.Content = item.body;
                    UInt64 timeStamp = item.localTime;
                    message.MessageTime = TimeConverter.UInt64ToDateTime(timeStamp);

                    messages.Add(message);
                }
            }
            catch (Exception)
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
                    ulong timeStamp = item["dateTaken"];
                    picture.Time = TimeConverter.UInt64ToDateTime(timeStamp);
                    picture.LocalUrl = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), Setting.PictureFolder, picture.Name);

                    if (item["geoInfo"] != null)
                    {
                        picture.Address = item["geoInfo"]["address"]["addressLines"][0];
                        picture.Gps = item["geoInfo"]["gps"];
                    }
                    if (item["exifInfo"] != null)
                    {
                        // if (item["exifInfo"]["dateTime"] != null)
                        //     picture.Time = item["exifInfo"]["dateTime"];
                        if (item["exifInfo"]["model"] != null)
                            picture.PhoneType = item["exifInfo"]["model"];
                    }
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

                    record.LocalUrl = System.IO.Path.Combine(Environment.CurrentDirectory, 
                        Setting.RecordFolder, record.Name);

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
                    file.LocalUrl = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(),
                        Setting.FileFolder , file.Name);

                    files.Add(file);
                }
            }
            catch (Exception e)
            {
                throw new Exception("文件信息格式出错，可能云服务网页有更新" + e.Message);
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
