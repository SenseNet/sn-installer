using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Web.Administration;
using Prism.Commands;
using SenseNet.Installer.Properties;
using Application = System.Windows.Application;

namespace SenseNet.Installer.ViewModels
{
    public class InstallerViewModel: INotifyPropertyChanged
    {
        /// <summary>
        /// Represents all the pages in the install wizard. When you add a new page in the view, 
        /// you have to insert a new enum value here to the exact same location and provide 
        /// the previous and next page in the GetPreviousPage and GetNextPage methods.
        /// </summary>
        public enum PageType
        {
            Welcome,
            PackageList,
            GetPackages,
            CreateWebsite,
            SelectWebsite,
            Database,
            Package,
            Install
        }

        #region Install type properties
        public InstallType InstallType
        {
            get
            {
                if (IsInstallTypeNewInstanceChecked)
                    return InstallType.NewInstance;
                if (IsInstallTypeProductPackageChecked)
                    return InstallType.ProductPackage;

                throw new NotImplementedException("Unknown InstallType value on the UI.");
            }
        }

        private bool _isInstallTypeNewInstanceChecked = true;
        public bool IsInstallTypeNewInstanceChecked
        {
            get
            {
                return _isInstallTypeNewInstanceChecked;
            }
            set
            {
                _isInstallTypeNewInstanceChecked = value;
                OnPropertyChanged();
            }
        }

        private bool _isInstallTypeProductPackageChecked;
        public bool IsInstallTypeProductPackageChecked
        {
            get
            {
                return _isInstallTypeProductPackageChecked;
            }
            set
            {
                _isInstallTypeProductPackageChecked = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Website properties

        private string _websiteName = "sensenet";
        public string WebsiteName
        {
            get
            {
                return _websiteName;
            }
            set
            {
                _websiteName = value;
                OnPropertyChanged();
            }
        }
        
        private bool? UseExistingWebsite { get; set; }
        private bool? UseExistingWebsiteBinding { get; set; }

        private string _websiteBinding = "localhost";
        public string WebsiteBinding
        {
            get
            {
                return _websiteBinding;
            }
            set
            {
                _websiteBinding = value;
                OnPropertyChanged();
                OnPropertyChanged("ShouldAddUrlToHostsFile");

                if (!ShouldAddUrlToHostsFile)
                    _addUrlToHostsFile = null;

                OnPropertyChanged("AddUrlToHostsFile");
            }
        }

        public bool ShouldAddUrlToHostsFile => WebsiteManager.ShouldAddToHostsFile(WebsiteBinding);

        private bool? _addUrlToHostsFile;
        public bool AddUrlToHostsFile
        {
            get
            {
                return _addUrlToHostsFile.HasValue 
                    ? _addUrlToHostsFile.Value && ShouldAddUrlToHostsFile 
                    : ShouldAddUrlToHostsFile; 
            }
            set
            {
                _addUrlToHostsFile = value; 
                OnPropertyChanged();
            }
        }

        private string _websFolderPath;
        public string WebFolderPath
        {
            get
            {
                return _websFolderPath ?? string.Empty;
            }
            set
            {
                _websFolderPath = value;
                OnPropertyChanged();
                OnPropertyChanged("LogFolderPath");
            }
        }

        private bool _websiteExistsAlertVisible;
        public bool WebsiteExistsAlertVisible
        {
            get
            {
                return _websiteExistsAlertVisible;
            }
            set
            {
                _websiteExistsAlertVisible = value;
                OnPropertyChanged();
            }
        }

        private bool _bindingExistsAlertVisible;
        public bool WebsiteBindingExistsAlertVisible
        {
            get
            {
                return _bindingExistsAlertVisible;
            }
            set
            {
                _bindingExistsAlertVisible = value;
                OnPropertyChanged();
            }
        }

        private bool _warningMessageVisible;
        public bool WarningMessageVisible
        {
            get
            {
                return _warningMessageVisible;
            }
            set
            {
                _warningMessageVisible = value;
                OnPropertyChanged();
            }
        }
        private string _warningMessageText;
        public string WarningMessageText
        {
            get
            {
                return _warningMessageText;
            }
            set
            {
                _warningMessageText = value;

                // messagebox visibility depends on the message text
                WarningMessageVisible = !string.IsNullOrEmpty(value);

                OnPropertyChanged();
            }
        }

        private ObservableCollection<Website> _websites; 
        public ObservableCollection<Website> Websites
        {
            get { return _websites ?? (_websites = new ObservableCollection<Website>(WebsiteManager.GetSites())); }
            set
            {
                _websites = value;

                OnPropertyChanged();
            }
        }

        private Website _selectedWebsite;

        public Website SelectedWebsite
        {
            get { return _selectedWebsite; }
            set
            {
                _selectedWebsite = value;

                WebsiteName = value.Name;
                WebFolderPath = value.Path;

                OnPropertyChanged();
            }
        }

        private ProcessModelIdentityType _apppoolIdentity = ProcessModelIdentityType.SpecificUser;
        public ProcessModelIdentityType ApppoolIdentity
        {
            get { return _apppoolIdentity; }
            set
            {
                _apppoolIdentity = value; 
                OnPropertyChanged();
            }
        }

        private string _identityUsername = GetCurrentUsername();
        public string IdentityUsername
        {
            get { return _identityUsername; }
            set
            {
                _identityUsername = value;
                OnPropertyChanged();
            }
        }

        private SecureString _identityPassword;
        public SecureString IdentityPassword
        {
            get { return _identityPassword; }
            set
            {
                _identityPassword = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Database properties

        private string _databaseServerName = ".";
        public string DatabaseServerName
        {
            get
            {
                return _databaseServerName;
            }
            set
            {
                _databaseServerName = value;
                OnPropertyChanged();

                // clear flag to initiate re-validation
                DatabaseExists = false;
            }
        }

        private string _databaseName = "SenseNetContentRepository";
        public string DatabaseName
        {
            get
            {
                return _databaseName;
            }
            set
            {
                _databaseName = value;
                OnPropertyChanged();

                // clear flag to initiate re-validation
                DatabaseExists = false;
            }
        }

        private bool _recreateDatabase = true;
        public bool RecreateDatabase
        {
            get
            {
                return _recreateDatabase;
            }
            set
            {
                _recreateDatabase = value;
                OnPropertyChanged();
            }
        }

        private bool _databaseExists;
        public bool DatabaseExists
        {
            get
            {
                return _databaseExists;
            }
            set
            {
                _databaseExists = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Package properties

        private string _packagePath;
        public string PackagePath
        {
            get
            {
                return _packagePath;
            }
            set
            {
                _packagePath = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<PackageItem> _packageItems = new ObservableCollection<PackageItem>();

        public ObservableCollection<PackageItem> PackageItems
        {
            get
            {
                return _packageItems;
            }
            set
            {
                _packageItems = value;
                OnPropertyChanged();
                OnSelectedPackagesChanged();
            }
        }

        public IEnumerable<PackageItem> SelectedPackages
        {
            get
            {
                return PackageItems.Where(p => p.Selected);
            }
        }

        #endregion

        #region Install properties

        private bool _working;
        public bool Working
        {
            get
            {
                return _working;
            }
            set
            {
                _working = value;
                OnPropertyChanged();
                RaiseCommandExecutableEvents();
            }
        }
        private bool _installCompleted;
        public bool InstallCompleted
        {
            get { return _installCompleted; }
            set
            {
                _installCompleted = value;

                OnPropertyChanged();
                OnPropertyChanged("NewInstallCompleted");
                RaiseCommandVisibilityEvents();
                RaiseCommandExecutableEvents();
            }
        }

        public bool NewInstallCompleted
        {
            get { return InstallCompleted && InstallType == InstallType.NewInstance;}
        }

        // The console is a queue of string lines that are concatenated and displayed
        // on the UI. The queue has a limited capacity. When it reaches that maximum size,
        // the first lines will start to disappear, like in the real console window.
        private static readonly int MaxConsoleLineCount = 1000;
        private readonly Queue<string> _consoleLines = new Queue<string>(MaxConsoleLineCount); 
        public string ConsoleText => string.Join(Environment.NewLine, _consoleLines);

        private string _installStatusText = Resources.InstallStatus_ReadyToGo;
        public string InstallStatusText
        {
            get
            {
                return _installStatusText;
            }
            set
            {
                _installStatusText = value;
                OnPropertyChanged();
            }
        }
        public string LogFolderPath => Path.Combine(WebFolderPath ?? string.Empty, "Admin\\log");

        private bool _installFailed;
        public bool InstallFailed
        {
            get { return _installFailed; }
            set
            {
                _installFailed = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Navigation properties
        
        private int _currentPageIndex = 1;
        public int CurrentPageIndex
        {
            get
            {
                return _currentPageIndex;
            }
            set
            {
                _currentPageIndex = value;

                OnPropertyChanged();
                RaiseCommandVisibilityEvents();
                RaiseCommandExecutableEvents();
            }
        }

        private PageType CurrentPageType
        {
            get { return (PageType) CurrentPageIndex; }
            set { CurrentPageIndex = (int) value; }
        }

        #endregion

        #region Constructors

        public InstallerViewModel()
        {
            PreviousCommand = DelegateCommand.FromAsyncHandler(OnPreviousCommand, CanExecutePreviousCommand);
            NextCommand = DelegateCommand.FromAsyncHandler(OnNextCommand, CanExecuteNextCommand);
            InstallCommand = DelegateCommand.FromAsyncHandler(OnInstallCommand, CanExecuteInstallCommand);
            FinishCommand = new DelegateCommand(OnFinishCommand);

            UseExistingWebsiteCommand = DelegateCommand.FromAsyncHandler(OnUseExistingWebsiteCommand);
            UseExistingWebsiteBindingCommand = DelegateCommand.FromAsyncHandler(OnUseExistingWebsiteBindingCommand);

            Task.Run(async () =>
            {
                var pi = await GetPackageItems();

                this.PackageItems = new ObservableCollection<PackageItem>(pi);
            });
        }

        #endregion

        #region Commands (navigation)

        public DelegateCommand PreviousCommand { get; set; }
        public DelegateCommand NextCommand { get; set; }
        public DelegateCommand InstallCommand { get; set; }
        public DelegateCommand FinishCommand { get; set; }

        private Task OnPreviousCommand()
        {
            ClearWarnings();

            CurrentPageType = GetPreviousPage();
            
            return Task.FromResult(0);
        }
        private bool CanExecutePreviousCommand()
        {
            return CurrentPageType != PageType.Welcome &&
                CurrentPageType != PageType.PackageList &&
                !Working &&
                !InstallCompleted;
        }
        public Visibility PreviousCommandVisibility => CanExecutePreviousCommand() ? Visibility.Visible : Visibility.Collapsed;

        private async Task OnNextCommand()
        {
            if (!await NextPageIsAllowed())
                return;

            ClearWarnings();

            var nextPage = GetNextPage();

            // prepare page (e.g. fill install data) before displaying it
            PreparePage(nextPage);

            CurrentPageType = nextPage;
        }
        private bool CanExecuteNextCommand()
        {
            return !Working && CurrentPageType != PageType.Install;
        }
        public Visibility NextCommandVisibility => CanExecuteNextCommand() ? Visibility.Visible : Visibility.Collapsed;

        private PageType GetPreviousPage()
        {
            switch (CurrentPageType)
            {
                case PageType.Welcome: return PageType.Welcome;
                case PageType.PackageList: return PageType.PackageList;
                case PageType.GetPackages: return PageType.PackageList;
                case PageType.CreateWebsite: return PageType.GetPackages;
                case PageType.SelectWebsite: return PageType.Welcome;
                case PageType.Database: return PageType.CreateWebsite;
                case PageType.Package:
                    return InstallType == InstallType.NewInstance
                        ? PageType.Database
                        : PageType.SelectWebsite;

                case PageType.Install: return PageType.Package;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private PageType GetNextPage()
        {
            switch (CurrentPageType)
            {
                case PageType.Welcome:
                    return InstallType == InstallType.NewInstance 
                        ? PageType.CreateWebsite 
                        : PageType.SelectWebsite;

                case PageType.PackageList: return PageType.GetPackages;
                case PageType.GetPackages: return PageType.CreateWebsite; //UNDONE: create or select, if Services package is selected or not
                case PageType.CreateWebsite: return PageType.Database;
                case PageType.SelectWebsite: return PageType.Package;
                case PageType.Database: return PageType.Package;
                case PageType.Package: return PageType.Install;
                case PageType.Install: return PageType.Install;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private void PreparePage(PageType nextPage)
        {
            switch (nextPage)
            {
                case PageType.Install:
                    _consoleLines.Clear();
                    foreach (var dataLine in GetCollectedData())
                    {
                        ConsoleWriteLine(dataLine);
                    }
                    break;
                case PageType.GetPackages:
                    foreach (var packageItem in PackageItems)
                    {
                        packageItem.DownloadPercent = 0;
                    }

                    // start downloading packages on a background thread
                    Task.Run(DownloadPackages);
                    break;
            }
        }

        private async Task OnInstallCommand()
        {
            Working = true;
            RaiseCommandExecutableEvents();

            try
            {
                if (InstallType == InstallType.NewInstance)
                    await InstallNewInstance();
                else
                    await InstallProductPackage();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("##SnInstaller> " + ex.ToString().Replace(Environment.NewLine, " *** "));
                InstallStatusText = string.Empty;
                InstallFailed = true;
            }
            finally
            {
                Working = false;
            }

            InstallCompleted = true;

            RaiseCommandExecutableEvents();
        }

        private async Task InstallNewInstance()
        {
            if (!await WebsiteManager.WebsiteExists(WebsiteName))
            {
                InstallStatusText = Resources.InstallStatus_CreatingWebSite;

                var identity = new AppPoolIdentity
                {
                    IdentityType = ApppoolIdentity
                };
                if (ApppoolIdentity == ProcessModelIdentityType.SpecificUser)
                {
                    identity.Username = IdentityUsername;
                    identity.Password = IdentityPassword;
                }

                await WebsiteManager.CreateSite(WebFolderPath, WebsiteName, new Uri(WebsiteBinding), identity, AddUrlToHostsFile);
            }

            var packageName = Path.GetFileNameWithoutExtension(PackagePath);

            InstallStatusText = Resources.InstallStatus_PreparingWebfolder;
            await PackageManager.CreateEnvironment(WebFolderPath, PackagePath);

            InstallStatusText = Resources.InstallStatus_ExecutingPackage;
            var snAdminResult = await PackageManager.ExecuteInstallPackage(WebFolderPath, WebsiteBinding, DatabaseServerName, DatabaseName, RecreateDatabase, packageName, ConsoleWriteLine);

            await WebsiteManager.StartWebsite(WebsiteName);

            if (snAdminResult == 0)
            {
                InstallStatusText = Resources.InstallStatus_Finished;
                InstallFailed = false;
            }
            else
            {
                InstallStatusText = string.Empty;
                InstallFailed = true;
            }
        }

        private async Task InstallProductPackage()
        {
            if (string.IsNullOrEmpty(PackagePath))
                throw new InvalidOperationException("Package path cannot be null.");

            var targetPackage = Path.Combine(WebFolderPath, "Admin", Path.GetFileName(PackagePath));

            // if the package is not under the Admin folder, copy it there to execute it locally
            if (!PackagePath.StartsWith(Path.Combine(WebFolderPath, "Admin\\")))
            {
                InstallStatusText = Resources.InstallStatus_CopyingPackage;

                await Task.Run(() =>
                {
                    File.Copy(PackagePath, targetPackage, true);
                });
            }
            else
            {
                // to handle possible subfolders under Admin
                targetPackage = PackagePath;
            }

            await WebsiteManager.StopWebsite(SelectedWebsite.Name);

            InstallStatusText = Resources.InstallStatus_ExecutingPackage;
            var snAdminResult = await PackageManager.ExecuteProductPackage(WebFolderPath, targetPackage, ConsoleWriteLine);
            
            await WebsiteManager.StartWebsite(SelectedWebsite.Name);

            if (snAdminResult == 0)
            {
                InstallStatusText = Resources.InstallStatus_Finished;
                InstallFailed = false;
            }
            else
            {
                InstallStatusText = string.Empty;
                InstallFailed = true;
            }
        }

        private bool CanExecuteInstallCommand()
        {
            return CurrentPageType == PageType.Install && !Working;
        }
        public Visibility InstallCommandVisibility => CanExecuteInstallCommand() && !InstallCompleted ? Visibility.Visible : Visibility.Collapsed;

        private static void OnFinishCommand()
        {
            Application.Current.MainWindow.Close();
        }
        public Visibility FinishCommandVisibility => CurrentPageType == PageType.Install && InstallCompleted ? Visibility.Visible : Visibility.Collapsed;

        private void RaiseCommandVisibilityEvents()
        {
            OnPropertyChanged("PreviousCommandVisibility");
            OnPropertyChanged("NextCommandVisibility");
            OnPropertyChanged("InstallCommandVisibility");
            OnPropertyChanged("FinishCommandVisibility");
        }

        private void RaiseCommandExecutableEvents()
        {
            PreviousCommand.RaiseCanExecuteChanged();
            NextCommand.RaiseCanExecuteChanged();
            InstallCommand.RaiseCanExecuteChanged();
        }

        private void ClearWarnings()
        {
            // Set warning values to their empty value to hide all possible warning messages 
            // before navigating to a different wizard page.

            WarningMessageText = string.Empty;
            WebsiteExistsAlertVisible = false;
            WebsiteBindingExistsAlertVisible = false;
        }

        #endregion

        #region Commands (Website page)

        public DelegateCommand UseExistingWebsiteCommand { get; set; }
        public DelegateCommand UseExistingWebsiteBindingCommand { get; set; }

        private async Task OnUseExistingWebsiteCommand()
        {
            UseExistingWebsite = true;
            WebsiteExistsAlertVisible = false;

            await OnNextCommand();
        }
        private async Task OnUseExistingWebsiteBindingCommand()
        {
            UseExistingWebsiteBinding = true;
            WebsiteBindingExistsAlertVisible = false;

            await OnNextCommand();
        }

        #endregion

        #region Validation

        private async Task<bool> NextPageIsAllowed()
        {
            switch (CurrentPageType)
            {
                case PageType.CreateWebsite:
                    return await ValidateWebsitePage();
                case PageType.SelectWebsite:
                    return await ValidateSelectWebsitePage();
                case PageType.Database:
                    return await ValidateDatabasePage();
                case PageType.Package:
                    return await ValidatePackagePage();
                default:
                    return true;
            }
        }
        private async Task<bool> ValidateWebsitePage()
        {
            WebsiteExistsAlertVisible = false;
            WebsiteBindingExistsAlertVisible = false;
            WarningMessageText = string.Empty;

            //TODO: Maybe implement this as a regular field validation? (INotifyDataErrorInfo interface)
            if (string.IsNullOrEmpty(WebsiteName) || 
                string.IsNullOrEmpty(WebsiteBinding) || 
                string.IsNullOrEmpty(WebFolderPath) ||
                (ApppoolIdentity == ProcessModelIdentityType.SpecificUser && 
                (string.IsNullOrEmpty(IdentityUsername) || IdentityPassword == null || IdentityPassword.Length == 0)))
            {
                WarningMessageText = Resources.Form_Validation_FillAllValues;
                return false;
            }

            if (Directory.Exists(WebFolderPath))
            {
                if (Directory.EnumerateFileSystemEntries(WebFolderPath).Any())
                {
                    WarningMessageText = Resources.WebsitePage_Message_DirectoryNotEmpty;
                    return false;
                }
            }
            else
            {
                try
                {
                    Directory.CreateDirectory(WebFolderPath);
                }
                catch (Exception)
                {
                    WarningMessageText = Resources.WebsitePage_Message_Error_Webfolder;
                    return false;
                }
            }
            
            try
            {
                if (await WebsiteManager.WebsiteExists(WebsiteName) && (!UseExistingWebsite.HasValue || !UseExistingWebsite.Value))
                {
                    WebsiteExistsAlertVisible = true;
                    return false;
                }
            }
            catch (Exception)
            {
                WarningMessageText = Resources.WebsitePage_Message_Error_WebsiteAdmin;
                return false;
            }

            try
            {
                if (!WebsiteBinding.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
                    WebsiteBinding = "http://" + WebsiteBinding;

                // ReSharper disable once UnusedVariable
                var bindingUri = new Uri(WebsiteBinding);

                if (await WebsiteManager.WebsiteBindingExists(WebsiteName, bindingUri) && (!UseExistingWebsiteBinding.HasValue || !UseExistingWebsiteBinding.Value))
                {
                    WebsiteBindingExistsAlertVisible = true;
                    return false;
                }
            }
            catch (UriFormatException)
            {
                WarningMessageText = Resources.WebsitePage_Message_IncorrectUrl;
                return false;
            }

            return true;
        }

        private Task<bool> ValidateSelectWebsitePage()
        {
            WarningMessageText = string.Empty;

            if (SelectedWebsite == null)
            {
                WarningMessageText = Resources.SelectWebsitePage_Message_Error_SelectSite;
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private async Task<bool> ValidateDatabasePage()
        {
            WarningMessageText = string.Empty;

            //TODO: Maybe implement this as a regular field validation? (INotifyDataErrorInfo interface)
            if (string.IsNullOrEmpty(DatabaseServerName) || string.IsNullOrEmpty(DatabaseName))
            {
                WarningMessageText = Resources.Form_Validation_FillAllValues;
                return false;
            }

            Working = true;

            try
            {
                // check db existence only if we did not found a db with this name before
                if (!DatabaseExists && await DatabaseManager.DatabaseExists(DatabaseServerName, DatabaseName))
                {
                    DatabaseExists = true;
                    WarningMessageText = Resources.DatabasePage_Message_DbAlreadyExists;
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.ToString());
                WarningMessageText = Resources.DatabasePage_Message_Error_DbConnection;
                return false;
            }
            finally
            {
                Working = false;
            }

            return true;
        }
        private Task<bool> ValidatePackagePage()
        {
            WarningMessageText = string.Empty;

            //TODO: Maybe implement this as a regular field validation? (INotifyDataErrorInfo interface)
            if (string.IsNullOrEmpty(PackagePath))
            {
                WarningMessageText = Resources.Form_Validation_FillAllValues;
                return Task.FromResult(false);
            }

            var packageName = Path.GetFileName(PackagePath);
            if (string.IsNullOrEmpty(packageName) || !packageName.EndsWith(".zip", StringComparison.InvariantCultureIgnoreCase) || !File.Exists(PackagePath))
            {
                WarningMessageText = Resources.PackagePage_Message_InvalidPackageFile;
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        #endregion

        #region Helper methods

        private string[] GetCollectedData()
        {
            if (InstallType == InstallType.NewInstance)
            {
                return new[]
                {
                    $"Website\t\t: {WebsiteName}",
                    $"URL\t\t: {WebsiteBinding}",
                    $"Web folder\t: {WebFolderPath}",
                    $"Db server\t: {DatabaseServerName}",
                    $"Database name\t: {DatabaseName}",
                    $"Package\t\t: {PackagePath}",
                };
            }
            if (InstallType == InstallType.ProductPackage)
            {
                return new[]
                {
                    $"Website\t\t: {WebsiteName}",
                    $"Web folder\t: {WebFolderPath}",
                    $"Package\t\t: {PackagePath}",
                };
            }

            throw new NotImplementedException("Define collected data for install type " + InstallType);
        }

        private void ConsoleWriteLine(string line)
        {
            // remove the first line if we reached the limit
            if (_consoleLines.Count == MaxConsoleLineCount)
                _consoleLines.Dequeue();

            _consoleLines.Enqueue(line);

            OnPropertyChanged("ConsoleText");
        }

        private static string GetCurrentUsername()
        {
            try
            {
                var domain = Environment.UserDomainName;

                return string.IsNullOrEmpty(domain) 
                    ? Environment.UserName 
                    : $"{domain}\\{Environment.UserName}";
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        #endregion

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnSelectedPackagesChanged()
        {
            OnPropertyChanged("SelectedPackages");
        }

        #endregion






        private async Task<PackageItem[]> GetPackageItems()
        {
            var packages = await PackageManager.LoadFeed();

            return packages.Select(p => new PackageItem(this, p)).ToArray();
        }

        private async Task DownloadPackages()
        {
            Working = true;

            await Task.WhenAll(SelectedPackages.Select(p => DownloadPackage(p)));

            Working = false;
        }

        private Random rnd { get; } = new Random();

        private async Task DownloadPackage(PackageItem package)
        {
            //UNDONE: implement real download
            var delay = rnd.Next(200, 600);

            for(int i = 1; i <= 10; i++)
            {
                await Task.Delay(delay);
                package.DownloadPercent = i * 10;
            }
        }
    }
}
