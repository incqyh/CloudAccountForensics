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
    public class BinderManager : INotifyPropertyChanged  
    {
        string status;
        public string Status
        {
            get { return status; }
            set
            {
                status = value;
                if (PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Status"));
                }
            }
        }

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
