using CAF.View.Common;
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

namespace CAF.View.Pages
{
    /// <summary>
    /// Page3.xaml 的交互逻辑
    /// </summary>
    public partial class Message : Page
    {
        VMManager vmm = VMManager.GetInstance();

        public Message()
        {
            InitializeComponent();

            Binding bind = new Binding
            {
                Source = vmm.BinderManager,
                Mode = BindingMode.OneWay,
                Path = new PropertyPath("Messages")
            };
            MessageList.SetBinding(ItemsControl.ItemsSourceProperty, bind);
        }
    }
}
