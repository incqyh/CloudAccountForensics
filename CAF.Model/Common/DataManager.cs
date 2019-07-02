using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Model.Common
{
    public class Contact
    {
        public string Name{ get; set; }
        public string Birthday{ get; set; }
        public string Company{ get; set; }
        public string Title{ get; set; }
        public List<string> Group { get; set; } = new List<string>();
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
        public TimeSpan LastTime { get; set; }
    }

    public class Picture
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Gps { get; set; }
        public DateTime Time { get; set; }

        public string BigThumbnailUrl { get; set; }
        public string PhoneType { get; set; }
        public string Id { get; set; }

        public byte[] Thumbnail { get; set; }
        public string AlbumId { get; set; }
        public string UniqueId { get; set; }

        public bool DownloadMutex { get; set; } = false;
        public string LocalUrl { get; set; }
    }

    public class Note
    {
        public string Snippet{ get; set; }
        public DateTime ModifyTime{ get; set; }
        public DateTime CreateTime{ get; set; }

        public string Id{ get; set; }
        public string ctagNoteTag;
        public string ctagNoteInfo;

        public bool DownloadMutex { get; set; } = false;

        public string LocalUrl { get; set; }
    }

    public class Record
    {
        public string Name{ get; set; }
        public DateTime ModifyTime{ get; set; }
        public DateTime CreateTime{ get; set; }

        public string Id{ get; set; }

        public bool DownloadMutex { get; set; } = false;

        public string LocalUrl { get; set; }
    }

    public class File
    {
        public string Name{ get; set; }
        public DateTime ModifyTime{ get; set; }
        public DateTime CreateTime{ get; set; }
        public string Size{ get; set; }
        public string Type{ get; set; }

        public string Id{ get; set; }
        public int Father{ get; set; }

        public bool DownloadMutex { get; set; } = false;

        public string LocalUrl { get; set; }
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
