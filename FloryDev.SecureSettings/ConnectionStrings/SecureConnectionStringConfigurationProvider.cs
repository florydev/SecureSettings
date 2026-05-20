using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FloryDev.SecureSettings.ConnectionStrings
{
    public class SecureConnectionStringConfigurationProvider : ConfigurationProvider
    {
        private const string Section = "ConnectionStrings";
        private static readonly List<SecureConnectionStringConfigurationProvider> _allProviders = new();

        private readonly string _physicalPath;

        public SecureConnectionStringConfigurationProvider(string physicalPath)
        {
            _physicalPath = physicalPath;
        }

        public override void Load()
        {
            Data.Clear();

            if (!File.Exists(_physicalPath))
                return;

            var jObject = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(_physicalPath));
            if (jObject?[Section] is JObject section)
            {
                foreach (var prop in section.Properties())
                    Data[ConfigurationPath.Combine(Section, prop.Name)] = prop.Value.ToString();
            }

            if (!_allProviders.Contains(this))
                _allProviders.Add(this);
        }

        public override bool TryGet(string key, out string? value)
        {
            if (!base.TryGet(key, out value))
                return false;

            if (!key.StartsWith(Section + ConfigurationPath.KeyDelimiter, StringComparison.OrdinalIgnoreCase))
                return true;

            if (EncryptedConfigSetting.Decrypter == null)
                return true;

            value = Decrypt(value!);
            return true;
        }

        // Called by SecureSettingsManager after encryption services are ready.
        // Encrypts any plain-text sensitive values in the file for all registered providers.
        public static void EncryptAll()
        {
            foreach (var provider in _allProviders)
                provider.EncryptFile();
        }

        private void EncryptFile()
        {
            if (!File.Exists(_physicalPath)) return;

            var jObject = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(_physicalPath));
            if (jObject?[Section] is not JObject section) return;

            bool changed = false;
            foreach (var prop in section.Properties().ToList())
            {
                var parts = SecureConnectionStringParser.Parse(prop.Value.ToString());
                var forFile = new List<(string Key, string Value, bool IsSensitive)>(parts.Count);
                bool needsUpdate = false;

                foreach (var (k, v, isSensitive) in parts)
                {
                    if (isSensitive && !SecureConnectionStringParser.IsEncryptedValue(v))
                    {
                        var encrypted = EncryptedConfigSetting.Encrypter.EncryptString(v);
                        forFile.Add((k, SecureConnectionStringParser.Wrap(encrypted), true));
                        needsUpdate = true;
                    }
                    else
                    {
                        forFile.Add((k, v, isSensitive));
                    }
                }

                if (needsUpdate)
                {
                    section[prop.Name] = SecureConnectionStringParser.Reconstruct(forFile);
                    changed = true;
                }
            }

            if (changed)
                File.WriteAllText(_physicalPath, JsonConvert.SerializeObject(jObject, Formatting.Indented));
        }

        private static string Decrypt(string raw)
        {
            var parts = SecureConnectionStringParser.Parse(raw);
            var result = new List<(string Key, string Value, bool IsSensitive)>(parts.Count);

            foreach (var (k, v, isSensitive) in parts)
            {
                if (isSensitive && SecureConnectionStringParser.IsEncryptedValue(v))
                {
                    var decrypted = EncryptedConfigSetting.Decrypter.DecryptString(
                        SecureConnectionStringParser.Unwrap(v));
                    result.Add((k, decrypted, true));
                }
                else
                {
                    result.Add((k, v, isSensitive));
                }
            }

            return SecureConnectionStringParser.Reconstruct(result);
        }
    }
}
