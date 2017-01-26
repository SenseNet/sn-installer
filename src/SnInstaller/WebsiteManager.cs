using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Microsoft.Web.Administration;

namespace SenseNet.Installer
{
    internal class AppPoolIdentity
    {
        internal ProcessModelIdentityType IdentityType { get; set; } = ProcessModelIdentityType.ApplicationPoolIdentity;
        internal string Username { get; set; }
        internal SecureString Password { get; set; }
    }

    internal class WebsiteManager
    {
        public static async Task CreateSite(string webFolderPath, string siteName, Uri uri, AppPoolIdentity identity, bool addToHostsFile)
        {
            await CreateSite(webFolderPath, siteName, uri.Scheme, uri.Host, uri.Port, identity, addToHostsFile);
        }
        public static async Task CreateSite(string webFolderPath, string siteName, string scheme, string host, int port, AppPoolIdentity identity, bool addToHostsFile)
        {
            await Task.Run(async () =>
            {
                if (!Directory.Exists(webFolderPath))
                    Directory.CreateDirectory(webFolderPath);

                var server = new ServerManager();

                //create a new application pool if there is no existing one
                var myApplicationPool = server.ApplicationPools.FirstOrDefault(p => p.Name == siteName);
                if (myApplicationPool == null)
                {
                    myApplicationPool = server.ApplicationPools.Add(siteName);
                    myApplicationPool.ManagedRuntimeVersion = "v4.0";

                    myApplicationPool.ProcessModel.IdentityType = identity.IdentityType;
                    if (identity.IdentityType == ProcessModelIdentityType.SpecificUser)
                    {
                        myApplicationPool.ProcessModel.UserName = identity.Username;
                        myApplicationPool.ProcessModel.Password = identity.Password.ConvertToUnsecureString();
                    }

                    server.CommitChanges();

                    // Wait a bit for the changes above to settle...
                    // This is to avoid the System.Runtime.InteropServices.COMException
                    // 'The object identifier does not represent a valid object.'
                    await Task.Delay(2000);
                }

                // create site
                var bindingInfo = GetBindingInfo(host, port);

                var site = server.Sites.Add(siteName, scheme, bindingInfo, webFolderPath);
                site.ApplicationDefaults.ApplicationPoolName = myApplicationPool.Name;

                server.CommitChanges();

                // Wait a bit for the changes above to settle...
                // This is to avoid the System.Runtime.InteropServices.COMException
                // 'The object identifier does not represent a valid object.'
                await Task.Delay(2000);

                site.Stop();

                var url = $"{host}:{port}";

                // add entry to hosts file if necessary
                if (ShouldAddToHostsFile(url) && addToHostsFile)
                    await ModifyHostsFile(url);
            });
        }

        public static IEnumerable<Website> GetSites()
        {
            var server = new ServerManager();
            return server.Sites?.Select(s => new Website
            {
                Name = s.Name,
                Path = s.Applications?.FirstOrDefault()?.VirtualDirectories?.FirstOrDefault()?.PhysicalPath
            }).OrderBy(s => s.Name).ToArray() ?? new Website[0];
        }

        public static async Task<bool> WebsiteExists(string siteName)
        {
            var siteExists = false;

            await Task.Run(() =>
            {
                var server = new ServerManager();
                siteExists = server.Sites?.Any(s => s.Name == siteName) ?? false;
            });

            return siteExists;
        }
        public static async Task<bool> WebsiteBindingExists(string siteName, Uri uri)
        {
            return await Task.Run(() =>
            {
                var server = new ServerManager();
                if (server.Sites == null)
                    return false;

                var bindingInfo = GetBindingInfo(uri.Host, uri.Port);

                return server.Sites
                    .Where(site => site.Name != siteName)
                    .Any(site => site.Bindings.Any(b => b.BindingInformation == bindingInfo && b.Protocol == uri.Scheme));
            });
        }
        public static async Task StartWebsite(string siteName)
        {
            await Task.Run(() =>
            {
                var server = new ServerManager();
                var site = server.Sites?.FirstOrDefault(s => s.Name == siteName);

                GetAppPool(server, site)?.Start();
                site?.Start();
            });
        }
        public static async Task StopWebsite(string siteName)
        {
            await Task.Run(() =>
            {
                var server = new ServerManager();
                var site = server.Sites?.FirstOrDefault(s => s.Name == siteName);

                site?.Stop();
                GetAppPool(server, site)?.Stop();
            });
        }

        private const string LocalIp = "127.0.0.1";
        private const string LocalHost = "localhost";

        public static async Task ModifyHostsFile(string url)
        {
            url = SimplifyUrl(url);

            if (!ShouldAddToHostsFile(url))
                return;

            await Task.Run(async () =>
            {
                try
                {
                    var hostsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System),
                        @"drivers\etc\hosts");

                    if (!File.Exists(hostsPath))
                        return;
                    
                    // check if the entry is already in the hosts file
                    if (File.ReadAllLines(hostsPath)
                        .Where(line => line.StartsWith($"{LocalIp} "))
                        .Any(line => line.Substring(10).Trim() == url))
                        return;

                    var entry = $"{LocalIp} {url}";

                    using (var w = File.AppendText(hostsPath))
                    {
                        await w.WriteAsync(Environment.NewLine + entry);
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(ex.Message);
                }
            });
        }
        public static bool ShouldAddToHostsFile(string url)
        {
            url = SimplifyUrl(url);

            return url != LocalHost && !url.StartsWith($"{LocalHost}:");
        }
        private static string SimplifyUrl(string url)
        {
            if (url.Contains("://"))
                url = url.Substring(url.IndexOf("://", StringComparison.Ordinal) + 3);
            if (url.EndsWith(":80"))
                url = url.Remove(url.Length - 3);

            return url;
        }

        private static ApplicationPool GetAppPool(ServerManager server, Site site)
        {
            var appPoolName = site?.Applications?.FirstOrDefault()?.ApplicationPoolName;
            return string.IsNullOrEmpty(appPoolName) 
                ? null 
                : server?.ApplicationPools?.FirstOrDefault(p => p.Name == appPoolName);
        }

        private static string GetBindingInfo(string host, int port)
        {
            return $@"*:{port}:{host}";
        }
    }
}
