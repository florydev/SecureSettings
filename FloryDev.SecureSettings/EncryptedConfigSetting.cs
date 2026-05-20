using FloryDev.SecureSettings.Interfaces;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace FloryDev.SecureSettings
{
    [TypeConverter(typeof(EncryptedConfigSettingConverter))]
    public class EncryptedConfigSetting
    {
        //As near as I can figure Dependency Injection doesn't work when using IOptions, something I need to figure out, right now I 
        //am setting it statically but I do need a better answer
        //public EncryptedConfigSetting(IEncrypter encrypter, IDecrypter decrypter)
        //{
        //    Encrypter = encrypter;
        //    Decrypter = decrypter;
        //}

        public static IDecryptionService Decrypter { get; set; }
        public static IEncryptionService Encrypter { get; set; }

        private string _value = string.Empty;

        public bool WasEncrypted { get; private set; }

        public string Value
        {
            get
            {
                return _value;
            }

            set
            {
                if (!SecureValueEncoding.IsEncrypted(value))
                {
                    _value = SecureValueEncoding.Wrap(Encrypter.EncryptString(value));
                    WasEncrypted = true;
                }
                else
                {
                    _value = value;
                }
            }

        }
        
        public static implicit operator string(EncryptedConfigSetting s) => s.Value;

        public string GetDecryptedValue()
        {
            if (!string.IsNullOrEmpty(Value))
                return Decrypter.DecryptString(SecureValueEncoding.Unwrap(Value));
            return string.Empty;
        }
    }
}
