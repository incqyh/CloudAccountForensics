using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
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
using System.Text.RegularExpressions;

using CAF.View.Pages;
using CAF.View.Common;
using System.ComponentModel;

namespace CAF.View
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>

    public partial class MainWindow : Window
    {
        CefBrowser browserPage = new CefBrowser();

        public MainWindow()
        {
            InitializeComponent();

            Binding bind = new Binding
            {
                Source = VMHelper.vmManager,
                Path = new PropertyPath("Status")
            };
            Status.SetBinding(TextBlock.TextProperty, bind);

            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();

        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            Display.Navigate(browserPage);
            browserPage.SwitchWebsite();
        }

        private void DisplayContacts_Click(object sender, RoutedEventArgs e)
        {
            Display.Navigate(new Uri("Pages/Contact.xaml", UriKind.Relative));
        }

        private void DisplayCallRecord_Click(object sender, RoutedEventArgs e)
        {
            Display.Navigate(new Uri("Pages/CallRecord.xaml", UriKind.Relative));  
        }

        private void DisplayMessage_Click(object sender, RoutedEventArgs e)
        {
            Display.Navigate(new Uri("Pages/Message.xaml", UriKind.Relative));  
        }

        private void SyncData_Click(object sender, RoutedEventArgs e)
        {
            VMHelper.vmManager.ReadFromWeb();
        }

        private void SaveData_Click(object sender, RoutedEventArgs e)
        {
            VMHelper.vmManager.UpdateDB();
        }

        private void ServiceProvider_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = ServiceProvider.SelectedItem as ComboBoxItem;
            VMHelper.vmManager.Init(item.Content.ToString());
        }

        private void DisplayNote_Click(object sender, RoutedEventArgs e)
        {
            Display.Navigate(new Uri("Pages/Note.xaml", UriKind.Relative));  
        }

        private void DisplayPicture_Click(object sender, RoutedEventArgs e)
        {
            Display.Navigate(new Uri("Pages/Picture.xaml", UriKind.Relative));  
        }

        private void DisplayRecord_Click(object sender, RoutedEventArgs e)
        {
            Display.Navigate(new Uri("Pages/Record.xaml", UriKind.Relative));  
        }

        private void DisplayFile_Click(object sender, RoutedEventArgs e)
        {
            Display.Navigate(new Uri("Pages/File.xaml", UriKind.Relative));  
        }
    }
}