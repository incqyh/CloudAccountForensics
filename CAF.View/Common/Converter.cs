using CAF.Model.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

            foreach (Contact contact in (ObservableCollection<Contact>)value)
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

    public class ByteToImageConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<Image> imgs = new List<Image>();
            // List<Button> buttons = new List<Button>();

            try
            {
                for (int i = 0; i < ((ObservableCollection<Picture>)value).Count; ++i)
                {
                    var rawByte = ((ObservableCollection<Picture>)value)[i].Thumbnail;
                    if (rawByte == null)
                        continue;
                    BitmapImage bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.UriSource = null;
                    bmp.StreamSource = new MemoryStream(rawByte);
                    bmp.EndInit();

                    // var brush = new ImageBrush
                    // {
                    //     Stretch = Stretch.UniformToFill,
                    //     AlignmentX = AlignmentX.Center,
                    //     AlignmentY = AlignmentY.Center,
                    //     ImageSource = bmp
                    // };
                    // // Style style = Application.Current.FindResource("ImageButtonStyle") as Style;
                    // Button button = new Button
                    // {
                    //     Width = 300,
                    //     Height = 300,
                    //     Background = brush,
                    //     Margin = new Thickness(10),
                    //     Uid = i.ToString()
                    // };
                    // button.Click += MouseButtonUpEvent;
                    // buttons.Add(button);

                    Image image = new Image
                    {
                        Width = 300,
                        Height = 300,
                        Source = bmp,
                        Margin = new System.Windows.Thickness(10),
                        Focusable = true,
                        Uid = i.ToString(),
                    };
                    image.MouseUp += MouseButtonUpEvent;
                    imgs.Add(image);
                }
            }
            catch (Exception e)
            {
                imgs = null;
                // buttons = null;
            }

            return imgs;
            // return buttons;
        }

        private void MouseButtonUpEvent(object sender, MouseButtonEventArgs e)
        // private void MouseButtonUpEvent(object sender, RoutedEventArgs e)
        {
            int index = int.Parse(((Image)sender).Uid);
            // int index = int.Parse(((Button)sender).Uid);
            Model.Common.EventManager.downloadPictureEventManager.RaiseEvent(BinderManager.pictureBinder.Pictures[index]);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
