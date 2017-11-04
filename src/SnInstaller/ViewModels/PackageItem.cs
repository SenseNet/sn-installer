using SenseNet.Installer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenseNet.Installer.ViewModels
{
    public class PackageItem
    {
        public bool Selected { get; set; }

        public string Id { get; private set; }
        public string IconUrl { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }

        public List<string> Versions { get; private set; } //UNDONE: handle when selected item is changed
        public int SelectedVersionIndex { get; set; }

        public static PackageItem ConvertFrom(PackageData packageData)
        {
            return new PackageItem
            {
                Id = packageData.Id,
                IconUrl = packageData.IconUrl,
                Title = packageData.Title,
                Description = packageData.Description,
                Versions = packageData.Versions.Select(v => v.Id).ToList()
            };
        }
                
        public string GetSelectedPackage()
        {
            //UNDONE: get selected package: id+version
            throw new NotImplementedException();
        }
    }
}
