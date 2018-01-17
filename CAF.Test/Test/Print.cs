using CAF.Model.Common;
using CAF.Model.DataObject;
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
    }
}
