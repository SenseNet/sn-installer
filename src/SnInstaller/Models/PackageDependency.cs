using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenseNet.Installer.Models
{
    public class PackageDependency
    {
        public string Id { get; set; }
        public string MinVersion { get; set; }
        public string MaxVersion { get; set; }
    }
}
