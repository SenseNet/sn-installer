using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using SenseNet.Installer.Models;

namespace SenseNet.Installer
{
    internal class PackageManager
    {
        public static async Task CreateEnvironment(string webfolderPath, string packagePath)
        {
            var adminPath = Path.Combine(webfolderPath, "Admin");
            var packageFileName = Path.GetFileName(packagePath) ?? string.Empty;
            var packageName = packageFileName.Substring(0, packageFileName.Length - 4); // remove '.zip'
            var packageFolderPath = Path.Combine(adminPath, packageName);

            Directory.CreateDirectory(packageFolderPath);

            await Task.Run(() =>
            {
                ZipFile.ExtractToDirectory(packagePath, packageFolderPath);

                var packageInstallFolder = Path.Combine(packageFolderPath, "Install");

                foreach (var dirPath in Directory.GetDirectories(packageInstallFolder, "*", SearchOption.AllDirectories))
                {
                    Directory.CreateDirectory(dirPath.Replace(packageInstallFolder, webfolderPath));
                }

                foreach (var newPath in Directory.GetFiles(packageInstallFolder, "*", SearchOption.AllDirectories))
                {
                    File.Copy(newPath, newPath.Replace(packageInstallFolder, webfolderPath), true);
                }
            });
        }

        public static async Task<int> ExecuteInstallPackage(string webfolderPath, string websiteBinding, string databaseServerName, string databaseName, bool recreateDbIfExists, string packageName, Action<string> consoleWriteLine)
        {
            var snAddminPath = Path.Combine(webfolderPath, "Admin\\bin\\SnAdmin.exe");
            var binding = websiteBinding
                .TrimStart("http://")
                .TrimStart("https://");

            var parameters = new[]
            {
                $"\"{packageName.Trim('"')}\"",
                $"DataSource:\"{databaseServerName}\"",
                $"DatabaseName:\"{databaseName}\"",
                $"RecreateDbIfExists:\"{recreateDbIfExists}\"",
                $"SiteUrl:\"{binding}\""
            };

            var result = 0;

            await Task.Run(() =>
            {
                result = ExecuteCommand(snAddminPath, parameters, consoleWriteLine);
            });

            // cleanup: delete extracted package
            var packageFolderPath = Path.Combine(webfolderPath, "Admin", packageName);
            if (Directory.Exists(packageFolderPath))
                Directory.Delete(packageFolderPath, true);

            return result;
        }
        public static async Task<int> ExecuteProductPackage(string webfolderPath, string packageName, Action<string> consoleWriteLine)
        {
            var snAddminPath = Path.Combine(webfolderPath, "Admin\\bin\\SnAdmin.exe");
            var parameters = new[]
            {
                $"\"{packageName.Trim('"')}\""
            };

            var result = 0;

            await Task.Run(() =>
            {
                result = ExecuteCommand(snAddminPath, parameters, consoleWriteLine);
            });

            // cleanup: delete extracted package
            //var packageFolderPath = Path.Combine(webfolderPath, "Admin", packageName);
            //if (Directory.Exists(packageFolderPath))
            //    Directory.Delete(packageFolderPath, true);

            return result;
        }

        private static int ExecuteCommand(string commandPath, string[] parameters, Action<string> consoleWriteLine)
        {
            var commandParams = string.Join(" ", parameters);

            Trace.WriteLine("##SnInstaller> Executing command " + commandPath + " " + commandParams);

            var startInfo = new ProcessStartInfo(commandPath, commandParams)
            {
                UseShellExecute = false,
                WorkingDirectory = Path.GetDirectoryName(commandPath),
                CreateNoWindow = true,
                ErrorDialog = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            var process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = startInfo,
            };

            process.Start();

            while (!process.StandardOutput.EndOfStream)
            {
                var line = process.StandardOutput.ReadLine();

                Trace.WriteLine("##SnInstaller> CMDOUTPUT: " + line);

                consoleWriteLine?.Invoke(line);
            }

            process.WaitForExit();

            var result = process.ExitCode;

            process.Dispose();

            return result;
        }


        private static PackageData[] _samplePackages = PackageData.SampleFeed;

        public static async Task<PackageData[]> LoadFeed()
        {
            await Task.Delay(2000);

            return _samplePackages;
        }                
    }
}
