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
        private PictureBinder pictureBinder;

        public Picture()
        {
            InitializeComponent();

            pictureBinder = new PictureBinder();

            Binding bind = new Binding
            {
                Source = pictureBinder,
                Mode = BindingMode.OneWay,
                Path = new PropertyPath("Pictures")
            };
            PictureList.SetBinding(ItemsControl.ItemsSourceProperty, bind);
        }

        private void PictureList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = ((ListView)sender).SelectedIndex;
            VMHelper.vmManager.DownloadPicture(index);
        }
    }
}
