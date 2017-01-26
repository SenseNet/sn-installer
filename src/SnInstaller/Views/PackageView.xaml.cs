using System.Windows;
using Microsoft.Win32;
using SenseNet.Installer.ViewModels;

namespace SenseNet.Installer.Views
{
    /// <summary>
    /// Interaction logic for PackageView.xaml
    /// </summary>
    public partial class PackageView
    {
        public PackageView()
        {
            InitializeComponent();
        }

        private void OpenPackageDialog_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = Properties.Resources.PackagePage_OpenFile_FilterText + "|*.zip"
                //InitialDirectory = 
            };

            // ReSharper disable once PossibleInvalidOperationException
            // This method always returns true or false
            if (openFileDialog.ShowDialog().Value)
            {
                var vm = this.DataContext as InstallerViewModel;
                if (vm != null)
                    vm.PackagePath = openFileDialog.FileName;
            }
        }
    }
}
