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

        /// <summary>
        /// Returns the original plain-text value. Always async — implementations backed by
        /// remote stores (e.g. Key Vault) require I/O; local implementations return immediately.
        /// Call only when the value is actively needed to avoid holding it in memory.
        /// </summary>
        public async Task<string> GetUnsecuredValueAsync()
        {
            if (!string.IsNullOrEmpty(Value))
                return await Unsecurer.UnsecureAsync(SecureValueEncoding.Unwrap(Value));
            return string.Empty;
        }
    }
}
