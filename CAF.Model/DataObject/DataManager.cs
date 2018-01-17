using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Model.DataObject
{
    public class DataManager
    {
        static DataModel userData = new DataModel();

        static public DataTable Contacts
        {
            get { return userData.contacts; }
            set
            {
                userData.contacts = value;
            }
        }
        static public DataTable Message
        {
            get { return userData.message; }
            set { userData.message = value; }
        }
        static public DataTable Memo
        {
            get { return userData.memo; }
            set { userData.memo = value; }
        }
        static public DataTable CallRecord
        {
            get { return userData.callRecord; }
            set { userData.callRecord = value; }
        }
    }
}
