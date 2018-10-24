using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Windows.Data;
using CAF.Model.Common;
using System.Collections.ObjectModel;

namespace CAF.View.Common
{
    public class BinderManager
    {
        public static ContactsBinder contactsBinder = new ContactsBinder();
        public static MessageBinder messageBinder = new MessageBinder();
        public static CallRecordBinder callRecordBinder = new CallRecordBinder();
        public static PictureBinder pictureBinder = new PictureBinder();
        public static NoteBinder noteBinder = new NoteBinder();
        public static RecordBinder recordBinder = new RecordBinder();
        public static FileBinder fileBinder = new FileBinder();
        public static GpsBinder gpsBinder = new GpsBinder();
    }

    public class ContactsBinder : INotifyPropertyChanged  
    {
        ObservableCollection<Contact> contacts = new ObservableCollection<Contact>();
        public ObservableCollection<Contact> Contacts
        {
            get { return contacts; }
            set  
            {
                contacts = value;
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
        ObservableCollection<Message> messages = new ObservableCollection<Message>();
        public ObservableCollection<Message> Messages
        {  
            get { return messages; }  
            set  
            {  
                messages = value;  
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
        ObservableCollection<CallRecord> callRecords = new ObservableCollection<CallRecord>();
        public ObservableCollection<CallRecord> CallRecords
        {
            get { return callRecords; }  
            set  
            {
                callRecords = value;  
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
        public int loadedCount = 0;

        ObservableCollection<Picture> pictures = new ObservableCollection<Picture>();
        public ObservableCollection<Picture> Pictures
        {
            get { return pictures; }  
            set  
            {
                pictures = value;  
                // Pictures.CollectionChanged -= OnCollectionChanged;
                // Pictures.CollectionChanged += OnCollectionChanged;
                if (PropertyChanged != null)  
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Pictures"));  
                }
            }
        }

        public void OnCollectionChanged()
        {
            this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Pictures"));  
        }

        public event PropertyChangedEventHandler PropertyChanged;  
    }

    public class NoteBinder : INotifyPropertyChanged  
    {
        ObservableCollection<Note> notes = new ObservableCollection<Note>();
        public ObservableCollection<Note> Notes
        {
            get { return notes; }  
            set  
            {
                notes = value;  
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
        ObservableCollection<Record> records = new ObservableCollection<Record>();
        public ObservableCollection<Record> Records
        {
            get { return records; }
            set
            {
                records = value;
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
        ObservableCollection<File> files = new ObservableCollection<File>();
        public ObservableCollection<File> Files
        {
            get { return files; }  
            set  
            {
                files = value;  
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
        ObservableCollection<Gps> gpses = new ObservableCollection<Gps>();
        public ObservableCollection<Gps> Gpses
        {
            get { return gpses; }  
            set  
            {
                gpses = value;  
                if (PropertyChanged != null)  
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Gpses"));  
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;  
    }
}
