using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Win32;
using SenseNet.Installer.ViewModels;
using System.Windows.Forms;

namespace SenseNet.Installer.Views
{
    /// <summary>
    /// Interaction logic for SelectSiteView.xaml
    /// </summary>
    public partial class CreateWebsiteView
    {
        public CreateWebsiteView()
        {
            InitializeComponent();
        }

        private void SelectWebFolderDialog_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result != DialogResult.OK && result != DialogResult.Yes)
                return;

            var vm = this.DataContext as InstallerViewModel;
            if (vm != null)
                vm.WebFolderPath = dialog.SelectedPath;
        }

        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            ((dynamic) this.DataContext).IdentityPassword = ((PasswordBox) sender).SecurePassword;
        }
    }
}
