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
using System.Windows.Forms;

namespace CAF.View.Pages
{
    /// <summary>
    /// Browser.xaml 的交互逻辑
    /// </summary>
    public partial class Browser : Page
    {
        public Browser()
        {
            InitializeComponent();
        }
        // public Browser(string url) : this()
        // {
        //     browser.ScriptErrorsSuppressed = true;
        //     browser.Navigate(url);
        // }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            string url = App.Current.Properties["frame"].ToString();
            browser.Address = url;
            // Task.Run(() =>
            // {
            //     try
            //     {
            //         browser.Navigate(url);
            //     }
            //     catch (Exception ex)
            //     {
            
            //     }
            // });
        }
        
    }
}
