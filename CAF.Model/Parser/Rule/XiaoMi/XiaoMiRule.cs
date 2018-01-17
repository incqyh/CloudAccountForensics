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

namespace CAF.Model.Parser.Rule
{
    public class XiaoMiRule : IDataParser
    {
        public void CallRecordParser(string raw_data)
        {
            throw new NotImplementedException();
        }

        public void ContactsParser(string rawJson)
        {
            dynamic contactsJson = Newtonsoft.Json.Linq.JToken.Parse(rawJson) as dynamic;

            DataManager.Contacts.Clear();
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

        public void MemoParser(string rawJson)
        {
            throw new NotImplementedException();
        }

        public void MessageParser(string rawJson)
        {
            dynamic messageJson = Newtonsoft.Json.Linq.JToken.Parse(rawJson) as dynamic;

            DataManager.Message.Clear();
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
    }
}
