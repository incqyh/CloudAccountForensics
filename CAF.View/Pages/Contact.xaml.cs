using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using CAF.View.Common;

namespace CAF.View.Pages
{
    /// <summary>
    /// Page1.xaml 的交互逻辑
    /// </summary>
    public partial class Contact : Page
    {
        ContactsBinder contactsBinder;

        public Contact()
        {
            InitializeComponent();

            contactsBinder = new ContactsBinder();

            Binding bind = new Binding
            {
                Source = contactsBinder,
                Mode = BindingMode.OneWay,
                Converter = new ContactsConverter(),
                Path = new PropertyPath("Contacts")
            };
            this.ContactsView.SetBinding(DataGrid.ItemsSourceProperty, bind);
        }
    }
}