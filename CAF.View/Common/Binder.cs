using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Windows.Data;
using CAF.Model.Common;

namespace CAF.View.Common
{
    public class ContactsBinder : INotifyPropertyChanged  
    {
        public List<Contact> Contacts
        {
            get { return DataManager.Contacts; }  
            set  
            {
                DataManager.Contacts = value;  
                if (PropertyChanged != null)  
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Contacts"));  
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;  
    }

    public class MessageBinder : INotifyPropertyChanged  
    {  
        public List<Message> Messages
        {  
            get { return DataManager.Messages; }  
            set  
            {  
                DataManager.Messages = value;  
                if (PropertyChanged != null)  
                {  
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Messages"));  
                }  
            }  
        }  

        public event PropertyChangedEventHandler PropertyChanged;  
    }

    public class CallRecordBinder : INotifyPropertyChanged  
    {
        public List<CallRecord> CallRecords
        {
            get { return DataManager.CallRecords; }  
            set  
            {
                DataManager.CallRecords = value;  
                if (PropertyChanged != null)  
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CallRecords"));  
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;  
    }

    public class PictureBinder : INotifyPropertyChanged  
    {
        public List<Picture> Pictures
        {
            get { return DataManager.Pictures; }  
            set  
            {
                DataManager.Pictures = value;  
                if (PropertyChanged != null)  
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Pictures"));  
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;  
    }

    public class NoteBinder : INotifyPropertyChanged  
    {
        public List<Note> Notes
        {
            get { return DataManager.Notes; }  
            set  
            {
                DataManager.Notes = value;  
                if (PropertyChanged != null)  
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Notes"));  
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;  
    }

    public class RecordBinder : INotifyPropertyChanged  
    {
        public List<Record> Records
        {
            get { return DataManager.Records; }  
            set  
            {
                DataManager.Records = value;  
                if (PropertyChanged != null)  
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Records"));  
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;  
    }

    public class FileBinder : INotifyPropertyChanged  
    {
        public List<File> Files
        {
            get { return DataManager.Files; }  
            set  
            {
                DataManager.Files = value;  
                if (PropertyChanged != null)  
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Files"));  
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;  
    }

    public class GpsBinder : INotifyPropertyChanged  
    {
        public List<Gps> Gpses
        {
            get { return DataManager.Gpses; }  
            set  
            {
                DataManager.Gpses = value;  
                if (PropertyChanged != null)  
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Gpses"));  
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;  
    }
}
