using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Model.Common
{
    public class DataManager
    {
        static public List<Contact> Contacts { get; set; } = new List<Contact>();
        static public List<Message> Messages { get; set; } = new List<Message>();
        static public List<CallRecord> CallRecords { get; set; } = new List<CallRecord>();
        static public List<Picture> Pictures { get; set; } = new List<Picture>();
        static public List<Note> Notes { get; set; } = new List<Note>();
        static public List<Record> Records { get; set; } = new List<Record>();
        static public List<File> Files { get; set; } = new List<File>();
        static public List<Gps> Gpses { get; set; } = new List<Gps>();
    }

    public class Contact
    {
        public string Name{ get; set; }
        public string Birthday{ get; set; }
        public List<KeyValuePair<string, string>> PhoneNumber { get; set; } = new List<KeyValuePair<string, string>>();
        public List<KeyValuePair<string, string>> Email { get; set; } = new List<KeyValuePair<string, string>>();
        public List<KeyValuePair<string, string>> Address { get; set; } = new List<KeyValuePair<string, string>>();
        public List<KeyValuePair<string, string>> ImAccount { get; set; } = new List<KeyValuePair<string, string>>();
    }
    
    public class Message
    {
        public string PhoneNumber{ get; set; }
        public string Direction{ get; set; }
        public string Content{ get; set; }
        public DateTime MessageTime{ get; set; }
    }

    public class CallRecord
    {
        public string PhoneNumber{ get; set; }
        public string Direction{ get; set; }
        public DateTime PhoneTime{ get; set; }
        public TimeSpan LastTime{ get; set; }
    }

    public class Picture
    {
        public string Name{ get; set; }
        public string Address{ get; set; }
        public string Gps{ get; set; }
        public bool IsAccurate{ get; set; }
        public string Time{ get; set; }
        public string BigThumbnailUrl{ get; set; }
        public string PhoneType{ get; set; }
        public string Id{ get; set; }
    }

    public class Note
    {
        public string Snippet{ get; set; }
        public DateTime ModifyTime{ get; set; }
        public DateTime CreateTime{ get; set; }
        public string Id{ get; set; }
    }

    public class Record
    {
        public string Name{ get; set; }
        public DateTime ModifyTime{ get; set; }
        public DateTime CreateTime{ get; set; }
        public string Id{ get; set; }
    }

    public class File
    {
        public string Name{ get; set; }
        public DateTime ModifyTime{ get; set; }
        public DateTime CreateTime{ get; set; }
        public string Size{ get; set; }
        public string Type{ get; set; }
        public string Id{ get; set; }
    }

    public class Gps
    {
        public string Imei{ get; set; }
        public DateTime Time{ get; set; }
        public double Latitude{ get; set; }
        public double Longitude{ get; set; }
        public double Accuracy{ get; set; }
    }
}
