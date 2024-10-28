using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloryDev.SecureSettings.Interfaces
{
    public interface IEncrypter
    {
        string EncryptString(string plainText);
    }
}
