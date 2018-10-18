using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CAF.Model.Common;

namespace CAF.Model.DataObject
{
    public class DataModel
    {
        public DataTable contacts;
        public DataTable message;
        public DataTable callRecord;
        public DataTable memo;

        public DataModel()
        {
            contacts = new DataTable();
            contacts.Columns.Add("name", typeof(string));
            contacts.Columns.Add("birthday", typeof(string));
            contacts.Columns.Add("phoneNumber", typeof(List<KeyValuePair<string, string>>));
            contacts.Columns.Add("email", typeof(List<KeyValuePair<string, string>>));
            contacts.Columns.Add("address", typeof(List<KeyValuePair<string, string>>));
            contacts.Columns.Add("imAccount", typeof(List<KeyValuePair<string, string>>));

            message = new DataTable();
            message.Columns.Add("phoneNumber", typeof(string));
            message.Columns.Add("direction", typeof(string));
            message.Columns.Add("content", typeof(string));
            message.Columns.Add("messageTime", typeof(DateTime));

            callRecord = new DataTable();
            callRecord.Columns.Add("phoneNumber", typeof(string));
            callRecord.Columns.Add("direction", typeof(string));
            callRecord.Columns.Add("phoneTime", typeof(DateTime));
            callRecord.Columns.Add("lastTime", typeof(TimeSpan));

            memo = new DataTable();
            memo.Columns.Add("title", typeof(string));
            memo.Columns.Add("content", typeof(string));
            memo.Columns.Add("recordTime", typeof(DateTime));
        }
    }
}
