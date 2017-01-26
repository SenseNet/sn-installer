using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenseNet.Installer
{
    internal static class Logger
    {
        public static void WriteLine(string message)
        {
            Trace.WriteLine(message.Replace(Environment.NewLine, " *** "));
        }
    }
}
