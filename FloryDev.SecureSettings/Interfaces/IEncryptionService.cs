using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloryDev.SecureSettings.Interfaces
{
    /// <summary>
    ///  This is the interface for decryption of secure settings so you can provide your own implementation class.I expect that typically both encryption and decryption might be handled
    /// by one object in some scenarios you might not even want to have a decrypter object so that is primarily why they are seperated. An 
    /// example of this is where one system is the public key of another, it cannot decrypt any data but it can encrypt it for the other
    /// system's use.
    /// </summary>
    public interface IEncryptionService
    {
        /// <summary>
        /// Encrypts the given plain text string.
        /// </summary>
        /// <param name="plainText">The plain text string to encrypt.</param>
        /// <returns>The encrypted string.</returns>
        string EncryptString(string plainText);
    }
}
