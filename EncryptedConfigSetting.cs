﻿using FloryDev.SecureSettings.Interfaces;

namespace FloryDev.SecureSettings
{
    public class EncryptedConfigSetting
    {
        public EncryptedConfigSetting(IEncrypter encrypter, IDecrypter decrypter)
        {
            Encrypter = encrypter;
            Decrypter = decrypter;
        }

        public IDecrypter Decrypter { get; set; }
        public IEncrypter Encrypter { get; set; }

        private string _value = string.Empty;

        public string Value
        {
            get
            {
                return _value;
            }

            set
            {
                if (Decrypter.ValueIsEncrypted(value))
                {
                    _value = Decrypter.DecryptString(value);
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
            {
                var unencryptedValue = Decrypter.DecryptString(Value);
                return unencryptedValue;
            }
            return String.Empty;
        }
    }
}
