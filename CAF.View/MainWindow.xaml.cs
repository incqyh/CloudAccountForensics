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
        VMManager vmManager = new VMManager();

        public MainWindow()
        {
            InitializeComponent();

            Binding bind = new Binding();
            bind.Source = vmManager;
            // bind.Mode = BindingMode.OneWay;
            bind.Path = new PropertyPath("Status");
            this.Status.SetBinding(TextBlock.TextProperty, bind);

            Display.Navigate((new Uri("Pages/Guide.xaml", UriKind.Relative)), vmManager.MainUrl);

            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            Display.Navigate((new Uri("Pages/Browser.xaml", UriKind.Relative)), vmManager.MainUrl);
            // Display.Navigate(new Browser(vmManager.MainUrl));
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DisplayContacts_Click(object sender, RoutedEventArgs e)
        {
            Display.Navigate(new Uri("Pages/Contacts.xaml", UriKind.Relative));  
        }

        private void DisplayCallRecord_Click(object sender, RoutedEventArgs e)
        {
            Display.Navigate(new Uri("Pages/CallRecord.xaml", UriKind.Relative));  
        }

        private void DisplayMessage_Click(object sender, RoutedEventArgs e)
        {
            Display.Navigate(new Uri("Pages/Message.xaml", UriKind.Relative));  
        }

        private void UpdateData_Click(object sender, RoutedEventArgs e)
        {
            vmManager.ReadFromWeb();
        }

        private void SaveData_Click(object sender, RoutedEventArgs e)
        {
            vmManager.UpdateDB();
        }

        private void ServiceProvider_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = ServiceProvider.SelectedItem as ComboBoxItem;
            vmManager.Init(item.Content.ToString());
            vmManager.ReadFromDB();
            Display.Navigate((new Uri("Pages/Guide.xaml", UriKind.Relative)), vmManager.MainUrl);
        }

        private void Display_Navigated(object sender, NavigationEventArgs e)
        {
            App.Current.Properties["frame"] = e.ExtraData;
        }
    }
}