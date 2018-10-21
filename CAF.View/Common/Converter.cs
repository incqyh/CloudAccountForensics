using CAF.Model.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CAF.View.Common
{
    public class ContactsConverter : IValueConverter
    {
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

            foreach (Contact contact in (List<Contact>)value)
            {
                List<string> phoneNumber = new List<string>();
                List<string> email = new List<string>();
                List<string> address = new List<string>();
                List<string> imAccount = new List<string>();
                foreach (var i in contact.PhoneNumber)
                    phoneNumber.Add(i.Key + ":" + i.Value);
                foreach (var i in contact.Email)
                    email.Add(i.Key + ":" + i.Value);
                foreach (var i in contact.Address)
                    address.Add(i.Key + ":" + i.Value);
                foreach (var i in contact.ImAccount)
                    imAccount.Add(i.Key + ":" + i.Value);

                contacts.Rows.Add(contact.Name, contact.Birthday, 
                    phoneNumber, email, address, imAccount,
                    phoneNumber.FirstOrDefault(), email.FirstOrDefault(), 
                    address.FirstOrDefault(), imAccount.FirstOrDefault());
            }
            return new DataView(contacts);
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TitleConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "CAF - " + (value ?? "No Title Specified");
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
