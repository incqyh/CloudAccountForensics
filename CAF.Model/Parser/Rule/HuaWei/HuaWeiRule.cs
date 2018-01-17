using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CAF.Model.DataObject;
using CAF.Model.Common;

namespace CAF.Model.Parser.Rule
{
    public class HuaWeiRule : IDataParser
    {
        Dictionary<int, string> phoneNumberTypeMap;
        Dictionary<int, string> emailTypeMap;
        Dictionary<int, string> addressTypeMap;
        Dictionary<int, string> imAccountTypeMap;

        public HuaWeiRule()
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

        public void CallRecordParser(string rawJson)
        {
            dynamic CallRecordJson = Newtonsoft.Json.Linq.JToken.Parse(rawJson) as dynamic;

            DataManager.CallRecord.Clear();

            foreach (var item in CallRecordJson.CallLogInfos)
            {
                string phoneNumber = item.PeerAddress;

                string direction = "";
                if (item.Direction == "1")
                    direction = "来电";
                else if (item.Direction == "2")
                    direction = "去电";
                else if (item.Direction == "3")
                    direction = "未接";

                UInt32 seconds = item.Duration;
                TimeSpan lastTime = TimeConverter.ToTimeSpan(seconds);

                UInt64 timeStamp = item.StartTime;
                DateTime phoneTime = TimeConverter.UInt64ToDateTime(timeStamp);

                DataManager.CallRecord.Rows.Add(phoneNumber, direction, phoneTime, lastTime);
            }
        }

        public void ContactsParser(string rawJson)
        {  
            // ContactsJsonObject contactsJson = JsonConvert.DeserializeObject<ContactsJsonObject>(rawJson);
            dynamic contactsJson = Newtonsoft.Json.Linq.JToken.Parse(rawJson) as dynamic;
            // var contacts = contactsJson.contactList;

            DataManager.Contacts.Clear();
            foreach (var item in contactsJson.contactList)
            {
                string name = item.fName;
                string birthday = item.bDay;

                List<KeyValuePair<string, string>> phoneNumber = new List<KeyValuePair<string, string>>();
                foreach (var i in item.telList)
                {
                    phoneNumber.Add(new KeyValuePair<string, string>(phoneNumberTypeMap[(int)i.type], (string)i.value));
                }

                List<KeyValuePair<string, string>> email = new List<KeyValuePair<string, string>>();
                foreach (var i in item.emailList)
                {
                    email.Add(new KeyValuePair<string, string>(emailTypeMap[(int)i.type], (string)i.value));
                }

                List<KeyValuePair<string, string>> address = new List<KeyValuePair<string, string>>();
                foreach (var i in item.addressList)
                {
                    address.Add(new KeyValuePair<string, string>(addressTypeMap[(int)i.type], (string)i.city));
                }

                List<KeyValuePair<string, string>> imAccount = new List<KeyValuePair<string, string>>();
                foreach (var i in item.msgList)
                {
                    imAccount.Add(new KeyValuePair<string, string>(imAccountTypeMap[(int)i.type], (string)i.value));
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
            foreach (var item in messageJson.SMSWithIDs)
            {
                string phoneNumber = item.SMSInfo.PeerAddress;

                string direction = "";
                if (item.SMSInfo.Direction == "1")
                    direction = "接收";
                else if (item.SMSInfo.Direction == "2")
                    direction = "发送";

                string content = item.SMSInfo.Text;
                UInt64 timeStamp = item.SMSInfo.ActivityTime;
                DateTime messageTime = TimeConverter.UInt64ToDateTime(timeStamp);

                DataManager.Message.Rows.Add(phoneNumber, direction, content, messageTime);
            }
        }
    }
}
