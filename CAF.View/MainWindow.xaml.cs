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
using CAF.Model.Common;
using Microsoft.Win32;

namespace CAF.View
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>

    public class MainApp
    {
        [STAThread]
        public static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                Setting.IsMain = false;
                string basePath = args[0];
                Setting.LogFile = basePath + "/" + Setting.LogFile;
                Setting.DbFile = basePath + "/" + Setting.DbFile;
                Setting.XmlFolder = basePath + "/" + Setting.XmlFolder;
                Setting.NoteFolder = basePath + "/" + Setting.NoteFolder;
                Setting.PictureFolder = basePath + "/" + Setting.PictureFolder;
                Setting.RecordFolder = basePath + "/" + Setting.RecordFolder;
                Setting.FileFolder = basePath + "/" + Setting.FileFolder;
            }
            App app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }

    public partial class MainWindow : Window
    {
        CefBrowser browserPage = new CefBrowser();
        Pages.Picture PicturePage = new Pages.Picture();
        VMManager vmm = VMManager.GetInstance();

        public MainWindow()
        {
            InitializeComponent();

            Binding bind = new Binding
            {
                Source = vmm.BinderManager,
                Path = new PropertyPath("Status")
            };
            Status.SetBinding(TextBlock.TextProperty, bind);

            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();

            Display.Navigate(browserPage);
            browserPage.SwitchWebsite();

            if (!Setting.IsMain)
            {
                DisplayContacts.Visibility = Visibility.Hidden;
                DisplayCallRecord.Visibility = Visibility.Hidden;
                DisplayMessage.Visibility = Visibility.Hidden;
                DisplayNote.Visibility = Visibility.Hidden;
                DisplayPicture.Visibility = Visibility.Hidden;
                DisplayRecord.Visibility = Visibility.Hidden;
                DisplayFile.Visibility = Visibility.Hidden;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (vmm.DuringForensics())
            {
                MessageBoxResult re = MessageBox.Show("正在取证，是否关闭程序？", "", MessageBoxButton.YesNo);
                if (re == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }

            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            CefSharp.Cef.Shutdown();
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        private void ServiceProvider_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = ServiceProvider.SelectedItem as ComboBoxItem;
            vmm.Init(item.Content.ToString());
            if (IsInitialized)
            {
                Display.Navigate(browserPage);
                browserPage.SwitchWebsite();
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            Display.Navigate(browserPage);
        }

        private void Init_Click(object sender, RoutedEventArgs e)
        {
            vmm.InitCrawler();
        }

        private void DisplayContacts_Click(object sender, RoutedEventArgs e)
        {
            vmm.SyncContact();
            Display.Navigate(new Uri("Pages/Contact.xaml", UriKind.Relative));
        }

        private void DisplayCallRecord_Click(object sender, RoutedEventArgs e)
        {
            vmm.SyncCallRecord();
            Display.Navigate(new Uri("Pages/CallRecord.xaml", UriKind.Relative));  
        }

        private void DisplayMessage_Click(object sender, RoutedEventArgs e)
        {
            vmm.SyncMessage();
            Display.Navigate(new Uri("Pages/Message.xaml", UriKind.Relative));  
        }

        private void DisplayNote_Click(object sender, RoutedEventArgs e)
        {
            vmm.SyncNote();
            Display.Navigate(new Uri("Pages/Note.xaml", UriKind.Relative));  
        }

        private void DisplayPicture_Click(object sender, RoutedEventArgs e)
        {
            if (Setting.Provider == Model.Common.ServiceProvider.HuaWei)
                if (browserPage.IsInitialized)
                    browserPage.SwitchToPicture();
            vmm.SyncPicture();
            Display.Navigate(PicturePage);
            // Display.Navigate(new Uri("Pages/Picture.xaml", UriKind.Relative));  
        }

        private void DisplayRecord_Click(object sender, RoutedEventArgs e)
        {
            vmm.SyncRecord();
            Display.Navigate(new Uri("Pages/Record.xaml", UriKind.Relative));  
        }

        private void DisplayFile_Click(object sender, RoutedEventArgs e)
        {
            vmm.SyncFile();
            Display.Navigate(new Uri("Pages/File.xaml", UriKind.Relative));  
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            vmm.BinderManager.Pictures.Clear();
            vmm.BinderManager.loadedCount = 0;
        }

        private void StartForensics_Click(object sender, RoutedEventArgs e)
        {
            if (vmm.DoneForensics)
            {
                MessageBoxResult re = MessageBox.Show("已完成取证，若想重新取证，请先保存当前数据库文件", "重新取证？", MessageBoxButton.YesNo);
                if (re == MessageBoxResult.Yes)
                {
                    if (Setting.Provider == Model.Common.ServiceProvider.HuaWei)
                        browserPage.SwitchToPicture();

                    if (Setting.IsMain)
                    {
                        OpenFileDialog openFileDialog = new OpenFileDialog();
                        openFileDialog.Filter = "Text files (*.db)|*.db|All files (*.*)|*.*";
                        if (openFileDialog.ShowDialog() == true)
                            Setting.DbFile = openFileDialog.FileName;
                    }
                    vmm.StartForensics();
                }
            }
            else
            {
                if (Setting.Provider == Model.Common.ServiceProvider.HuaWei)
                    browserPage.SwitchToPicture();

                if (Setting.IsMain)
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Filter = "Text files (*.db)|*.db|All files (*.*)|*.*";
                    if (openFileDialog.ShowDialog() == true)
                        Setting.DbFile = openFileDialog.FileName;
                }
                vmm.StartForensics();
            }
        }
    }
}