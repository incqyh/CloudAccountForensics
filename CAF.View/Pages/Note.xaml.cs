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
    /// Interaction logic for Note.xaml
    /// </summary>
    public partial class Note : Page
    {
        private NoteBinder noteBinder;

        public Note()
        {
            InitializeComponent();
            
            noteBinder = new NoteBinder();

            Binding bind = new Binding
            {
                Source = noteBinder,
                Mode = BindingMode.OneWay,
                Path = new PropertyPath("Notes")
            };
            NoteList.SetBinding(ItemsControl.ItemsSourceProperty, bind);
        }

        private void NoteList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = ((ListView)sender).SelectedIndex;
            VMHelper.vmManager.DownloadNote(index);
        }
    }
}