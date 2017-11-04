using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenseNet.Installer.Models
{
    public class PackageVersion
    {
        public string Id { get; set; }
        public PackageDependency[] Dependencies { get; set; } = new PackageDependency[0];
    }
}
