using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CAF.Model.DataObject;
using System.Data;
using System.Windows.Data;

namespace CAF.View.Common
{
    public class ContactsBinder : INotifyPropertyChanged  
    {  
        public DataTable Contacts
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
        public DataTable Message
        {  
            get { return DataManager.Message; }  
            set  
            {  
                DataManager.Message = value;  
                if (PropertyChanged != null)  
                {  
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Message"));  
                }  
            }  
        }  

        public event PropertyChangedEventHandler PropertyChanged;  
    }  

    public class CallRecordBinder : INotifyPropertyChanged  
    {  
        public DataTable CallRecord
        {  
            get { return DataManager.CallRecord; }  
            set  
            {  
                DataManager.CallRecord = value;  
                if (PropertyChanged != null)  
                {  
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CallRecord"));  
                }  
            }  
        }  

        public event PropertyChangedEventHandler PropertyChanged;  
    }   
}
