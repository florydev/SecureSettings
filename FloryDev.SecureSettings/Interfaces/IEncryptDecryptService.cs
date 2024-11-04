using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloryDev.SecureSettings.Interfaces
{
    /// <summary>
    /// Interface for both encrypting and decrypting strings.
    /// Combines the functionalities of <see cref="IEncryptionService"/> and <see cref="IDecryptionService"/>.
    /// </summary>
    public interface IEncryptDecryptService : IEncryptionService, IDecryptionService
    {
    }
}
