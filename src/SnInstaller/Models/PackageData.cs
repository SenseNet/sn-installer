using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenseNet.Installer.Models
{
    public class PackageData
    {
        public string Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string IconUrl { get; private set; }

        public PackageVersion[] Versions { get; private set; }
        

        //public PackageData(string id, string title, string description = null, string icon = "/sensenet.ico")
        //{
        //    Id = id;
        //    Title = title;
        //    Description = description;
        //    IconUrl = icon;

        //    Versions = new List<string> { "7.0.0-beta1", "7.0.0-beta2" };
        //}

        internal static PackageData[] SampleFeed = new PackageData[]
        {
            new PackageData
            {
                Id = "SenseNet.Services",
                Title = "sensenet ECM Services",
                Description = "Core layer",
                IconUrl = "https://raw.githubusercontent.com/SenseNet/sn-resources/master/images/sn-icon/sensenet-icon-64.ico",
                Versions = new PackageVersion[] 
                {
                    new PackageVersion
                    {
                        Id = "7.0.0-beta1",
                        Dependencies = new PackageDependency[0]
                    },
                    new PackageVersion
                    {
                        Id = "7.0.0-beta2",
                        Dependencies = new PackageDependency[0]
                    },
                    new PackageVersion
                    {
                        Id = "7.0.0-beta3",
                        Dependencies = new PackageDependency[0]
                    }
                }
            },
            new PackageData
            {
                Id = "SenseNet.WebPages",
                Title = "sensenet ECM WebPages",
                Description = "WebForms UI",
                Versions = new PackageVersion[]
                {
                    new PackageVersion
                    {
                        Id = "7.0.0-beta1",
                        Dependencies = new PackageDependency[]
                        {
                            new PackageDependency
                            {
                                Id = "SenseNet.Services",
                                MinVersion = "7.0.0-beta2"
                            }
                        }
                    },
                    new PackageVersion
                    {
                        Id = "7.0.0-beta2",
                        Dependencies = new PackageDependency[]
                        {
                            new PackageDependency
                            {
                                Id = "SenseNet.Services",
                                MinVersion = "7.0.0-beta2"
                            }
                        }
                    }
                }
            },
            new PackageData
            {
                Id = "SenseNet.Workspaces",
                Title = "sensenet ECM Workspaces",
                Description = "Workspace features",
                Versions = new PackageVersion[]
                {
                    new PackageVersion
                    {
                        Id = "7.0.0-beta1",
                        Dependencies = new PackageDependency[]
                        {
                            new PackageDependency
                            {
                                Id = "SenseNet.Services",
                                MinVersion = "7.0.0-beta2"
                            }
                        }
                    },
                    new PackageVersion
                    {
                        Id = "7.0.0-beta2",
                        Dependencies = new PackageDependency[]
                        {
                            new PackageDependency
                            {
                                Id = "SenseNet.Services",
                                MinVersion = "7.0.0-beta2"
                            }
                        }
                    }
                }
            }
        };
    }
}
