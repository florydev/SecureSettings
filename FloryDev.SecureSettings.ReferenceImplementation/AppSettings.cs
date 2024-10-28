using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloryDev.SecureSettings.ReferenceImplementation
{
    public class AppSettings
    {
        public static string SectionName = "AppSettings";
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
