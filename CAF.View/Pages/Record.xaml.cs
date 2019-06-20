using CAF.Model.Common;
using CAF.View.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CAF.View.Pages
{
    /// <summary>
    /// Interaction logic for Record.xaml
    /// </summary>
    public partial class Record : Page
    {
        VMManager vmm = VMManager.GetInstance();

        public Record()
        {
            InitializeComponent();

            Binding bind = new Binding
            {
                Source = vmm.BinderManager,
                Mode = BindingMode.OneWay,
                Path = new PropertyPath("Records")
            };
            RecordList.SetBinding(ItemsControl.ItemsSourceProperty, bind);
        }

        private void RecordList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = ((ListView)sender).SelectedIndex;
            vmm.DownloadRecord(vmm.BinderManager.Records[index]);
        }
    }
}
