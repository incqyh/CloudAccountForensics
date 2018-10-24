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
            foreach (var contact in DataManager.Contacts)
            {
                Console.WriteLine(contact.Name);
                Console.WriteLine(contact.Birthday);

                foreach (var phoneNumber in (List<KeyValuePair<string, string>>)contact.PhoneNumber)
                {
                    Console.WriteLine(phoneNumber.Key);
                    Console.WriteLine(phoneNumber.Value);
                }

                foreach (var email in (List<KeyValuePair<string, string>>)contact.Email)
                {
                    Console.WriteLine(email.Key);
                    Console.WriteLine(email.Value);
                }

                foreach (var address in (List<KeyValuePair<string, string>>)contact.Address)
                {
                    Console.WriteLine(address.Key);
                    Console.WriteLine(address.Value);
                }

                foreach (var imAccount in (List<KeyValuePair<string, string>>)contact.ImAccount)
                {
                    Console.WriteLine(imAccount.Key);
                    Console.WriteLine(imAccount.Value);
                }
            }
        }

        public static void PrintCallRecord()
        {
            foreach (var callRecord in DataManager.CallRecords)
            {
                Console.WriteLine(callRecord.PhoneNumber);
                Console.WriteLine(callRecord.Direction);
                Console.WriteLine(callRecord.PhoneTime);
                Console.WriteLine(callRecord.LastTime);
            }
        }

        public static void PrintMessage()
        {
            foreach (var message in DataManager.Messages)
            {
                Console.WriteLine(message.PhoneNumber);
                Console.WriteLine(message.Direction);
                Console.WriteLine(message.Content);
                Console.WriteLine(message.MessageTime);
            }
        }

        public static void PrintPicture()
        {
            foreach (var picture in DataManager.Pictures)
            {
                Console.WriteLine(picture.Name);
                Console.WriteLine(picture.Address);
                Console.WriteLine(picture.Gps);
                Console.WriteLine(picture.IsAccurate);
                Console.WriteLine(picture.Time);
                Console.WriteLine(picture.BigThumbnailUrl);
                Console.WriteLine(picture.PhoneType);
                Console.WriteLine(picture.Id);
            }
        }

        public static void PrintNote()
        {
            foreach (var note in DataManager.Notes)
            {
                Console.WriteLine(note.Snippet);
                Console.WriteLine(note.ModifyTime);
                Console.WriteLine(note.CreateTime);
                Console.WriteLine(note.Id);
            }
        }

        public static void PrintRecord()
        {
            foreach (var record in DataManager.Records)
            {
                Console.WriteLine(record.Name);
                Console.WriteLine(record.ModifyTime);
                Console.WriteLine(record.CreateTime);
                Console.WriteLine(record.Id);
            }
        }

        public static void PrintFile()
        {
            foreach (var file in DataManager.Files)
            {
                Console.WriteLine(file.Name);
                Console.WriteLine(file.ModifyTime);
                Console.WriteLine(file.CreateTime);
                Console.WriteLine(file.Size);
                Console.WriteLine(file.Type);
                Console.WriteLine(file.Id);
            }
        }

        public static void PrintGps()
        {
            foreach (var gps in DataManager.Gpses)
            {
                Console.WriteLine(gps.Imei);
                Console.WriteLine(gps.Time);
                Console.WriteLine(gps.Latitude);
                Console.WriteLine(gps.Longitude);
                Console.WriteLine(gps.Accuracy);
            }
        }
    }
}
