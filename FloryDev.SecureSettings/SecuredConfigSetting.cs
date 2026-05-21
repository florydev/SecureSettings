using FloryDev.SecureSettings.Interfaces;
using System.ComponentModel;

namespace FloryDev.SecureSettings
{
    [TypeConverter(typeof(SecuredConfigSettingConverter))]
    public class SecuredConfigSetting
    {
        public static IUnsecureService Unsecurer { get; set; }
        public static ISecureService Securer { get; set; }

        private string _value = string.Empty;

        public bool WasSecured { get; private set; }

        public string Value
        {
            get => _value;
            set
            {
                if (!SecureValueEncoding.IsSecured(value))
                {
                    _value = SecureValueEncoding.Wrap(Securer.Secure(value));
                    WasSecured = true;
                }
                else
                {
                    _value = value;
                }
            }
        }

        public static implicit operator string(SecuredConfigSetting s) => s.Value;

        /// <summary>Returns the original plain-text value. Call only when the value is needed — avoids holding the unsecured form in memory.</summary>
        public string GetUnsecuredValue()
        {
            if (!string.IsNullOrEmpty(Value))
                return Unsecurer.Unsecure(SecureValueEncoding.Unwrap(Value));
            return string.Empty;
        }
    }
}
