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

        public PackageItem(string id, string title, string description = null, string icon = "/sensenet.ico")
        {
            Id = id;
            Title = title;
            Description = description;
            IconUrl = icon;

            Versions = new List<string> { "7.0.0-beta1", "7.0.0-beta2" };
        }
                
        public string GetSelectedPackage()
        {
            //UNDONE: get selected package: id+version
            throw new NotImplementedException();
        }
    }
}
