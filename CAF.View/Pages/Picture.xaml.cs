using CAF.Model.Common;
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
    /// Interaction logic for Picture.xaml
    /// </summary>
    public partial class Picture : Page
    {
        VMManager vmm = VMManager.GetInstance();

        public Picture()
        {
            InitializeComponent();

            Binding bind = new Binding
            {
                Source = vmm.BinderManager,
                Mode = BindingMode.OneWay,
                Converter = new ByteToImageConverter(),
                Path = new PropertyPath("Pictures")
            };
            gallery.SetBinding(ItemsControl.ItemsSourceProperty, bind);

            Model.Common.EventManager.downloadPictureEventManager.DownloadPictureEvent -= DownloadPictureEvent;
            Model.Common.EventManager.downloadPictureEventManager.DownloadPictureEvent += DownloadPictureEvent;
        }

        private void DownloadPictureEvent(object sender, DownloadPictureEventArgs e)
        {
            vmm.DownloadPicture(e.picture);
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
                vmm.DownloadThumbnail();
        }
    }
}
