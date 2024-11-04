using FloryDev.SecureSettings.Interfaces;
using FloryDev.SecureSettings.Writer;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloryDev.SecureSettings
{
    public class SecureSettingsManager
    {
        private readonly IEncryptionService _encryptionService;
        private readonly IDecryptionService _decryptionService;

        public SecureSettingsManager(IEncryptionService encryptionService, IDecryptionService decryptionService)
        {
            _encryptionService = encryptionService;
            _decryptionService = decryptionService;
            EncryptedConfigSetting.Encrypter = encryptionService;
            EncryptedConfigSetting.Decrypter = decryptionService;
        }
    }
}
