using CAF.Model.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Test
{
    class Print
    {
        public static void PrintContacts()
        {
            foreach (DataRow row in DataManager.Contacts.Rows)
            {
                Console.WriteLine(row["name"]);
                Console.WriteLine(row["birthday"]);

                foreach (var phoneNumber in (List<KeyValuePair<string, string>>)row["phoneNumber"])
                {
                    Console.WriteLine(phoneNumber.Key);
                    Console.WriteLine(phoneNumber.Value);
                }

                foreach (var email in (List<KeyValuePair<string, string>>)row["email"])
                {
                    Console.WriteLine(email.Key);
                    Console.WriteLine(email.Value);
                }

                foreach (var address in (List<KeyValuePair<string, string>>)row["address"])
                {
                    Console.WriteLine(address.Key);
                    Console.WriteLine(address.Value);
                }

                foreach (var imAccount in (List<KeyValuePair<string, string>>)row["imAccount"])
                {
                    Console.WriteLine(imAccount.Key);
                    Console.WriteLine(imAccount.Value);
                }
            }
        }

        public static void PrintCallRecord()
        {
            foreach (DataRow row in DataManager.CallRecord.Rows)
            {
                Console.WriteLine(row["phoneNumber"]);
                Console.WriteLine(row["direction"]);
                Console.WriteLine(row["phoneTime"]);
                Console.WriteLine(row["lastTime"]);
            }
        }

        public static void PrintMessage()
        {
            foreach (DataRow row in DataManager.Message.Rows)
            {
                Console.WriteLine(row["phoneNumber"]);
                Console.WriteLine(row["direction"]);
                Console.WriteLine(row["content"]);
                Console.WriteLine(row["messageTime"]);
            }
        }

        public static void PrintPicture()
        {
            foreach (DataRow row in DataManager.Picture.Rows)
            {
                Console.WriteLine(row["name"]);
                Console.WriteLine(row["address"]);
                Console.WriteLine(row["gps"]);
                Console.WriteLine(row["isAccurate"]);
                Console.WriteLine(row["time"]);
                Console.WriteLine(row["bigThumbnailUrl"]);
                Console.WriteLine(row["width"]);
                Console.WriteLine(row["length"]);
                Console.WriteLine(row["phoneType"]);
                Console.WriteLine(row["id"]);
            }
        }

        public static void PrintNote()
        {
            foreach (DataRow row in DataManager.Note.Rows)
            {
                Console.WriteLine(row["snippet"]);
                Console.WriteLine(row["modifyTime"]);
                Console.WriteLine(row["createTime"]);
                Console.WriteLine(row["id"]);
            }
        }

        public static void PrintRecord()
        {
            foreach (DataRow row in DataManager.Record.Rows)
            {
                Console.WriteLine(row["name"]);
                Console.WriteLine(row["modifyTime"]);
                Console.WriteLine(row["createTime"]);
                Console.WriteLine(row["id"]);
            }
        }

        public static void PrintFile()
        {
            foreach (DataRow row in DataManager.File.Rows)
            {
                Console.WriteLine(row["name"]);
                Console.WriteLine(row["modifyTime"]);
                Console.WriteLine(row["createTime"]);
                Console.WriteLine(row["size"]);
                Console.WriteLine(row["type"]);
                Console.WriteLine(row["id"]);
            }
        }

        public static void PrintGps()
        {
            foreach (DataRow row in DataManager.Gps.Rows)
            {
                Console.WriteLine(row["imei"]);
                Console.WriteLine(row["time"]);
                Console.WriteLine(row["latitude"]);
                Console.WriteLine(row["longitude"]);
                Console.WriteLine(row["accuracy"]);
            }
        }
    }
}
