using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace SenseNet.Installer.Views
{
    /// <summary>
    /// Interaction logic for InstallView.xaml
    /// </summary>
    public partial class InstallView : UserControl
    {
        public InstallView()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;

            //tb.SelectionStart = int.MaxValue;
            //tb.SelectionLength = 0;
            tb?.ScrollToEnd();
        }

        private void LogFolder_Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            }
            catch (Exception ex)
            {
                // cannot do much at this point
                Logger.WriteLine(ex.ToString());
            }

            e.Handled = true;
        }

        private void Website_Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            }
            catch (Exception ex)
            {
                // cannot do much at this point
                Logger.WriteLine(ex.ToString());
            }

            e.Handled = true;
        }
    }
}
