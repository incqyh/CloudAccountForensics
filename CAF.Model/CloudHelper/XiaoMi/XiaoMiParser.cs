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
        public void ParseCallRecord(string rawJson)
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

                    DataManager.CallRecords.Add(callRecord);
                }
            }
            catch (Exception)
            {
                throw new Exception("通话记录数据格式出错，可能云服务网页有更新");
            }
        }

        public void ParseContacts(string rawJson)
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

                    if (content["phoneNumbers"] != null)
                        foreach (var i in content["phoneNumbers"])
                            contact.PhoneNumber.Add(new KeyValuePair<string, string>((string)i["type"], (string)i["value"]));

                    if (content["emails"] != null)
                        foreach (var i in content["emails"])
                            contact.Email.Add(new KeyValuePair<string, string>((string)i["type"], (string)i["value"]));

                    if (content["address"] != null)
                        foreach (var i in content["addresses"])
                            contact.Address.Add(new KeyValuePair<string, string>((string)i["type"], (string)i["value"]));

                    if (content["ims"] != null)
                        foreach (var i in content["ims"])
                            contact.ImAccount.Add(new KeyValuePair<string, string>((string)i["type"], (string)i["value"]));

                    DataManager.Contacts.Add(contact);
                }
            }
            catch (Exception)
            {
                throw new Exception("通讯录数据格式出错，可能云服务网页有更新");
            }
        }

        public void ParseMessage(string rawJson)
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
                DataManager.Messages.Clear();
            //     runtimeData.isFirstTime = false;
            // }
            try
            {
                // runtimeData.lastPage = messageJson.data.watermark.lastPage;
                // runtimeData.syncIgnoreTag = messageJson.data.watermark.syncIgnoreTag;
                // runtimeData.syncTag = messageJson.data.watermark.syncTag;
                foreach (var item in messageJson.data.entries)
                {
                    Message message = new Message();
                    message.PhoneNumber = item.entry.recipients;

                    message.Direction = "";
                    if (item.entry.folder == "0")
                        message.Direction = "接收";
                    else if (item.entry.folder == "1")
                        message.Direction = "发送";

                    message.Content = item.entry.snippet;
                    UInt64 timeStamp = item.entry.localTime;
                    message.MessageTime = TimeConverter.UInt64ToDateTime(timeStamp);

                    DataManager.Messages.Add(message);
                }
            }
            catch (Exception)
            {
                throw new Exception("短信记录数据格式出错，可能云服务网站有更新");
            }
        }

        public void ParsePicture(string rawJson)
        {
            dynamic pictureJson;
            try
            {
                pictureJson = Newtonsoft.Json.Linq.JToken.Parse(rawJson) as dynamic;
            }
            catch (Exception)
            {
                throw new Exception("图片信息录解析出错，请尝试重新获取数据");
            }

            DataManager.Pictures.Clear();
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

                    DataManager.Pictures.Add(picture);
                }
            }
            catch (Exception)
            {
                throw new Exception("图片信息格式出错，可能云服务网页有更新");
            }
            
        }

        public void ParseNote(string rawJson)
        {
            dynamic noteJson;
            try
            {
                noteJson = Newtonsoft.Json.Linq.JToken.Parse(rawJson) as dynamic;
            }
            catch (Exception)
            {
                throw new Exception("备忘录解析出错，请尝试重新获取数据");
            }

            DataManager.Notes.Clear();
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

                    DataManager.Notes.Add(note);
                }
            }
            catch (Exception)
            {
                throw new Exception("备忘录格式出错，可能云服务网页有更新");
            }
        }

        public void ParseRecord(string rawJson)
        {
            dynamic recordJson;
            try
            {
                recordJson = Newtonsoft.Json.Linq.JToken.Parse(rawJson) as dynamic;
            }
            catch (Exception)
            {
                throw new Exception("录音信息解析出错，请尝试重新获取数据");
            }

            DataManager.Records.Clear();
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

                    DataManager.Records.Add(record);
                }
            }
            catch (Exception)
            {
                throw new Exception("录音信息格式出错，可能云服务网页有更新");
            }
        }

        public void ParseFile(string rawJson)
        {
            dynamic fileJson;
            try
            {
                fileJson = Newtonsoft.Json.Linq.JToken.Parse(rawJson) as dynamic;
            }
            catch (Exception)
            {
                throw new Exception("文件信息解析出错，请尝试重新获取数据");
            }

            DataManager.Files.Clear();
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

                    DataManager.Files.Add(file);
                }
            }
            catch (Exception)
            {
                throw new Exception("文件信息格式出错，可能云服务网页有更新");
            }
        }

        public void ParseGps(string rawJson)
        {
            dynamic gpsJson;
            try
            {
                gpsJson = Newtonsoft.Json.Linq.JToken.Parse(rawJson) as dynamic;
            }
            catch (Exception)
            {
                throw new Exception("gps信息解析出错，请尝试重新获取数据");
            }

            DataManager.Gpses.Clear();
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

                    DataManager.Gpses.Add(gps);
                }
            }
            catch (Exception)
            {
                throw new Exception("gps信息格式出错，可能云服务网页有更新");
            }
        }
    }
}
