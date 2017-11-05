using SenseNet.Installer.Models;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SenseNet.Installer.ViewModels
{
    public class PackageItem : INotifyPropertyChanged
    {
        private bool _selected;
        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                _viewModel.OnSelectedPackagesChanged();
            }
        }

        public string Id => _packageData.Id;
        public string IconUrl => _packageData.IconUrl;
        public string Title => _packageData.Title;
        public string Description => _packageData.Description;

        public string[] Versions => _packageData.Versions.Select(v => v.Id).ToArray();

        //UNDONE: handle when selected version is changed
        public int SelectedVersionIndex { get; set; }

        private int _downloadPercent;
        public int DownloadPercent
        {
            get
            {
                return _downloadPercent;
            }
            set
            {
                _downloadPercent = value;
                OnPropertyChanged();
            }
        }

        private PackageData _packageData;
        private InstallerViewModel _viewModel;

        public PackageItem(InstallerViewModel viewModel, PackageData data)
        {
            _viewModel = viewModel;
            _packageData = data;
        }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion        
    }
}
