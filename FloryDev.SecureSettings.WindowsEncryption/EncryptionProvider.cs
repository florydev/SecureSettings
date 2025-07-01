using FloryDev.SecureSettings.Interfaces;
using Microsoft.Extensions.Options;
using System.Runtime.Versioning;
using System.Security.Cryptography;

namespace FloryDev.SecureSettings.WindowsEncryption
{
    [SupportedOSPlatform("windows")]
    public class EncryptionProvider : IEncryptDecryptService
    {
        private WindowsEncryptionSettings Settings { get; set; }
        public EncryptionProvider(IOptions<WindowsEncryptionSettings> settings)
        {
            Settings = settings.Value;
        }

        public string DecryptString(string encryptedString)
        {
            var bytesCypherText = Convert.FromBase64String(encryptedString);

            RSACryptoServiceProvider csp = GetProviderFromContainerKey(Settings.KeyContainerName);

            //decrypt and strip pkcs#1.5 padding
            var bytesPlainTextData = csp.Decrypt(bytesCypherText, false);

            //get our original plainText back...
            String plainTextData = System.Text.Encoding.Unicode.GetString(bytesPlainTextData);

            return plainTextData;
        }

        public string EncryptString(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.Unicode.GetBytes(plainText);

            RSACryptoServiceProvider csp = GetProviderFromContainerKey(Settings.KeyContainerName);

            var encryptedBytes = csp.Encrypt(plainTextBytes, false);
            String cypherText = Convert.ToBase64String(encryptedBytes);

            return cypherText;
        }

        public static RSACryptoServiceProvider GetProviderFromContainerKey(string ContainerName)
        {
            CspParameters cp = new()
            {
                KeyContainerName = ContainerName
            };
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cp);
            return rsa;
        }

        /// <summary>
        /// The intention of this method is to determine if a value is encrypted or not so that perhaps
        /// we can provide a shortcut to determining that rather than decrypting the value. This implementation 
        /// is not what I would consider a good implementation. It is a placeholder for a better implementation.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ValueIsEncrypted(string value)
        {
            try
            {
                DecryptString(value);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}
