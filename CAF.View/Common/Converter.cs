using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CAF.View.Common
{
    public class ContactsConverter : IValueConverter
    {
        /// <summary>
        /// 主要在dataview中加入几项
        /// 存储在combox中被选中的值
        /// 否则显示时会出现奇怪的异常
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DataTable contacts = new DataTable();
            contacts.Columns.Add("name", typeof(string));
            contacts.Columns.Add("birthday", typeof(string));

            contacts.Columns.Add("phoneNumber", typeof(List<string>));
            contacts.Columns.Add("email", typeof(List<string>));
            contacts.Columns.Add("address", typeof(List<string>));
            contacts.Columns.Add("imAccount", typeof(List<string>));

            contacts.Columns.Add("selectedPhoneNumber", typeof(string));
            contacts.Columns.Add("selectedEmail", typeof(string));
            contacts.Columns.Add("selectedAddress", typeof(string));
            contacts.Columns.Add("selectedIMAccount", typeof(string));

            foreach (DataRowView drv in (DataView)value)
            {
                string name = drv[0].ToString();
                string birthday = drv[1].ToString();

                List<List<string>> row = new List<List<string>>();
                row.Add(new List<string>());
                row.Add(new List<string>());
                row.Add(new List<string>());
                row.Add(new List<string>());

                for (int i = 2; i < drv.Row.ItemArray.Length; i++)
                {
                    List<KeyValuePair<string, string>> x = (List<KeyValuePair<string, string>>)drv[i];
                    foreach (var item in x)
                    {
                        row[i - 2].Add(item.Key + ":" + item.Value);
                    }
                }
                contacts.Rows.Add(name, birthday, row[0], row[1], row[2], row[3], 
                    row[0].FirstOrDefault(), row[1].FirstOrDefault(), row[2].FirstOrDefault(), row[3].FirstOrDefault());
            }
            return new DataView(contacts);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
