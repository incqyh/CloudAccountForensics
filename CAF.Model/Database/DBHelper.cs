using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

using CAF.Model.DataObject;
using CAF.Model.Common;
using System.Data;
using System.IO;

namespace CAF.Model.Database
{
    public class DBHelper
    {
        SQLiteConnection conn;
        SQLiteCommand cmd;

        public DBHelper()
        {
            string DBPath = "";
            switch (Setting.Provider)
            {
                case ServiceProvider.HuaWei:
                    DBPath = "Database/Huawei.db";
                    break;
                case ServiceProvider.XiaoMi:
                    DBPath = "Database/Xiaomi.db";
                    break;
            }

            if (!Directory.Exists("Database"))
            {
                 Directory.CreateDirectory("Database");
            }

            conn = new SQLiteConnection("data source=" + DBPath);
            cmd = new SQLiteCommand
            {
                Connection = conn
            };

            try
            {
                conn.Open();
            }
            catch
            {
                throw new Exception("Fail to link to database!");
            }
        }

        ~DBHelper()
        {
            try
            {
                conn.Close();
            }
            catch
            {

            }
        }

        public void ContactsDBUpdate()
        {
            ContactsDBCreate();
            SQLiteHelper sh = new SQLiteHelper(cmd);
            UInt32 id = 0;
            Dictionary<string, object> dic;

            sh.BeginTransaction();
            try
            {
                foreach (DataRow row in DataManager.Contacts.Rows)
                {
                    id += 1;

                    dic = new Dictionary<string, object>
                    {
                        ["id"] = id,
                        ["name"] = row["name"],
                        ["birthday"] = row["birthday"]
                    };

                    sh.Insert("contacts", dic);

                    foreach (var phoneNumber in (List<KeyValuePair<string, string>>)row["phoneNumber"])
                    {
                        dic = new Dictionary<string, object>
                        {
                            ["id"] = id,
                            ["type"] = phoneNumber.Key,
                            ["phoneNumber"] = phoneNumber.Value
                        };
                        sh.Insert("contactsPhoneNumber", dic);
                    }

                    foreach (var email in (List<KeyValuePair<string, string>>)row["email"])
                    {
                        dic = new Dictionary<string, object>
                        {
                            ["id"] = id,
                            ["type"] = email.Key,
                            ["email"] = email.Value
                        };
                        sh.Insert("contactsEmail", dic);
                    }

                    foreach (var address in (List<KeyValuePair<string, string>>)row["address"])
                    {
                        dic = new Dictionary<string, object>
                        {
                            ["id"] = id,
                            ["type"] = address.Key,
                            ["address"] = address.Value
                        };
                        sh.Insert("contactsAddress", dic);
                    }

                    foreach (var imAccount in (List<KeyValuePair<string, string>>)row["imAccount"])
                    {
                        dic = new Dictionary<string, object>
                        {
                            ["id"] = id,
                            ["type"] = imAccount.Key,
                            ["imAccount"] = imAccount.Value 
                        };
                        sh.Insert("contactsIMAccount", dic);
                    }
                }
                sh.Commit();
            }
            catch
            {
                sh.Rollback();
                throw new Exception("Fail to execute sql!");
            }
        }

        void ContactsDBCreate()
        {
            try
            {
                SQLiteHelper sh = new SQLiteHelper(cmd);
                sh.DropTable("contacts");
                sh.DropTable("contactsPhoneNumber");
                sh.DropTable("contactsEmail");
                sh.DropTable("contactsAddress");
                sh.DropTable("contactsIMAccount");

                SQLiteTable tb = new SQLiteTable("contacts");
                tb.Columns.Add(new SQLiteColumn("id", false));
                tb.Columns.Add(new SQLiteColumn("name"));
                tb.Columns.Add(new SQLiteColumn("birthday"));
                sh.CreateTable(tb);

                tb = new SQLiteTable("contactsPhoneNumber");
                tb.Columns.Add(new SQLiteColumn("id", false));
                tb.Columns.Add(new SQLiteColumn("type"));
                tb.Columns.Add(new SQLiteColumn("phoneNumber"));
                sh.CreateTable(tb);

                tb = new SQLiteTable("contactsEmail");
                tb.Columns.Add(new SQLiteColumn("id", false));
                tb.Columns.Add(new SQLiteColumn("type"));
                tb.Columns.Add(new SQLiteColumn("email"));
                sh.CreateTable(tb);

                tb = new SQLiteTable("contactsAddress");
                tb.Columns.Add(new SQLiteColumn("id", false));
                tb.Columns.Add(new SQLiteColumn("type"));
                tb.Columns.Add(new SQLiteColumn("address"));
                sh.CreateTable(tb);

                tb = new SQLiteTable("contactsIMAccount");
                tb.Columns.Add(new SQLiteColumn("id", false));
                tb.Columns.Add(new SQLiteColumn("type"));
                tb.Columns.Add(new SQLiteColumn("imAccount"));
                sh.CreateTable(tb);
            }
            catch
            {
                throw new Exception("Fail to create table");
            }

        }

        public void MessageDBUpdate()
        {
            MessageDBCreate();
            SQLiteHelper sh = new SQLiteHelper(cmd);
            UInt32 id = 0;
            Dictionary<string, object> dic;

            sh.BeginTransaction();
            try
            {
                foreach (DataRow row in DataManager.Message.Rows)
                {
                    id += 1;

                    dic = new Dictionary<string, object>
                    {
                        ["id"] = id,
                        ["phoneNumber"] = row["phoneNumber"],
                        ["direction"] = row["direction"],
                        ["content"] = row["content"],
                        ["messageTime"] = TimeConverter.DateTimeToString((DateTime)row["messageTime"]),
                    };
                    sh.Insert("message", dic);
                }
                sh.Commit();
            }
            catch
            {
                sh.Rollback();
                throw new Exception("Fail to execute sql!");
            }
        }

        void MessageDBCreate()
        {
            try
            {
                SQLiteHelper sh = new SQLiteHelper(cmd);
                sh.DropTable("message");

                SQLiteTable tb = new SQLiteTable("message");

                tb.Columns.Add(new SQLiteColumn("id", true));
                tb.Columns.Add(new SQLiteColumn("phoneNumber"));
                tb.Columns.Add(new SQLiteColumn("direction"));
                tb.Columns.Add(new SQLiteColumn("content"));
                tb.Columns.Add(new SQLiteColumn("messageTime"));

                sh.CreateTable(tb);
            }
            catch 
            {
                throw new Exception("Fail to create table");
            }
        }

        public void CallRecordDBUpdate()
        {
            CallRecordDBCreate();
            SQLiteHelper sh = new SQLiteHelper(cmd);
            UInt32 id = 0;
            Dictionary<string, object> dic;

            sh.BeginTransaction();
            try
            {
                foreach (DataRow row in DataManager.CallRecord.Rows)
                {
                    id += 1;

                    dic = new Dictionary<string, object>
                    {
                        ["id"] = id,
                        ["phoneNumber"] = row["phoneNumber"],
                        ["direction"] = row["direction"],
                        ["phoneTime"] = TimeConverter.DateTimeToString((DateTime)row["phoneTime"]),
                        ["lastTime"] = TimeConverter.TimeSpanToUInt32((TimeSpan)row["lastTime"])
                    };
                    sh.Insert("callRecord", dic);
                }
                sh.Commit();
            }
            catch
            {
                sh.Rollback();
                throw new Exception("Fail to execute sql!");
            }
        }

        void CallRecordDBCreate()
        {
            try
            {
                SQLiteHelper sh = new SQLiteHelper(cmd);
                sh.DropTable("callRecord");

                SQLiteTable tb = new SQLiteTable("callRecord");

                tb.Columns.Add(new SQLiteColumn("id", true));
                tb.Columns.Add(new SQLiteColumn("phoneNumber"));
                tb.Columns.Add(new SQLiteColumn("direction"));
                tb.Columns.Add(new SQLiteColumn("lastTime", ColType.Integer));
                tb.Columns.Add(new SQLiteColumn("phoneTime"));

                sh.CreateTable(tb);
            }
            catch
            {
                throw new Exception("Fail to create table");
            }
        }

        public void ContactsRead()
        {
            SQLiteHelper sh = new SQLiteHelper(cmd);
            DataTable dtContacts = sh.Select("select * from contacts order by id;");

            DataManager.Contacts.Rows.Clear();

            string selectTemplate = "select * from {0} where id = {1};";

            foreach (DataRow crow in dtContacts.Rows)
            {
                string name = (string)crow["name"];
                string birthday = (string)crow["birthday"];

                string id = (string)crow["id"];
                DataTable dtPhoneNumber = sh.Select(String.Format(selectTemplate, "contactsPhoneNumber", id));
                DataTable dtEmail = sh.Select(String.Format(selectTemplate, "contactsEmail", id));
                DataTable dtAddress = sh.Select(String.Format(selectTemplate, "contactsAddress", id));
                DataTable dtIMAccount = sh.Select(String.Format(selectTemplate, "contactsIMAccount", id));

                List<KeyValuePair<string, string>> phoneNumber = new List<KeyValuePair<string, string>>();
                foreach (DataRow prow in dtPhoneNumber.Rows)
                {
                    phoneNumber.Add(new KeyValuePair<string, string>(prow["type"].ToString(), prow["phoneNumber"].ToString()));
                }

                List<KeyValuePair<string, string>> email = new List<KeyValuePair<string, string>>();
                foreach (DataRow erow in dtEmail.Rows)
                {
                    email.Add(new KeyValuePair<string, string>(erow["type"].ToString(), erow["email"].ToString()));
                }

                List<KeyValuePair<string, string>> address = new List<KeyValuePair<string, string>>();
                foreach (DataRow arow in dtAddress.Rows)
                {
                    address.Add(new KeyValuePair<string, string>(arow["type"].ToString(), arow["address"].ToString()));
                }

                List<KeyValuePair<string, string>> imAccount = new List<KeyValuePair<string, string>>();
                foreach (DataRow irow in dtIMAccount.Rows)
                {
                    imAccount.Add(new KeyValuePair<string, string>(irow["type"].ToString(), irow["imAccount"].ToString()));
                }

                DataManager.Contacts.Rows.Add(name, birthday, phoneNumber, email, address, imAccount);
            }
        }

        public void MessageRead()
        {
            SQLiteHelper sh = new SQLiteHelper(cmd);
            DataTable dt = sh.Select("select * from message;");
            // Utility.PrintDataTable(dt);

            DataManager.Message.Rows.Clear();

            foreach (DataRow row in dt.Rows)
            {
                string phoneNumber = row["phoneNumber"].ToString();
                string direction = row["direction"].ToString();
                string content = row["content"].ToString();
                string timeStamp = row["messageTime"].ToString();
                //UInt64 timeStamp = Convert.ToUInt64((string)row["messageTime"]);
                DateTime messageTime = TimeConverter.UInt64ToDateTime(Convert.ToUInt64(timeStamp));

                DataManager.Message.Rows.Add(phoneNumber, direction, content, messageTime);
            }
        }

        public void CallRecordRead()
        {
            SQLiteHelper sh = new SQLiteHelper(cmd);
            DataTable dt = sh.Select("select * from callRecord;");

            DataManager.Message.Rows.Clear();

            foreach (DataRow row in dt.Rows)
            {
                string phoneNumber = (string)row["phoneNumber"];
                string direction = row["direction"].ToString();

                Int64 tmp = (Int64)row["lastTime"];
                TimeSpan lastTime = TimeConverter.ToTimeSpan((UInt64)tmp);

                string timeStamp = row["phoneTime"].ToString();
                DateTime phoneTime = TimeConverter.UInt64ToDateTime(Convert.ToUInt64(timeStamp));

                DataManager.CallRecord.Rows.Add(phoneNumber, direction, phoneTime, lastTime);
            }
        }
    }
}
