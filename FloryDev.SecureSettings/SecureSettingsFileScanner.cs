using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace FloryDev.SecureSettings
{
    internal static class SecureSettingsFileScanner
    {
        private record SectionRegistration(string SectionPath, PropertyInfo[] SecuredProperties);
        private static readonly List<SectionRegistration> _registrations = new();

        internal static void Register(string sectionPath, Type settingsType)
        {
            var props = settingsType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType == typeof(SecuredConfigSetting))
                .ToArray();

            if (props.Length > 0)
                _registrations.Add(new SectionRegistration(sectionPath, props));
        }

        internal static void EncryptInDirectory(string directory)
        {
            if (_registrations.Count == 0) return;

            foreach (var file in Directory.GetFiles(directory, "appsettings*.json"))
                EncryptInFile(file);
        }

        private static void EncryptInFile(string filePath)
        {
            var jObject = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(filePath));
            if (jObject == null) return;

            bool changed = false;
            foreach (var reg in _registrations)
            {
                var section = NavigateTo(jObject, reg.SectionPath.Split(':'));
                if (section == null) continue;

                foreach (var prop in reg.SecuredProperties)
                {
                    var match = section.Properties()
                        .FirstOrDefault(p => string.Equals(p.Name, prop.Name, StringComparison.OrdinalIgnoreCase));
                    if (match == null) continue;

                    var value = match.Value.ToString();
                    if (string.IsNullOrEmpty(value) || SecureValueEncoding.IsSecured(value)) continue;

                    section[match.Name] = SecureValueEncoding.Wrap(
                        SecuredConfigSetting.Securer.Secure(value));
                    changed = true;
                }
            }

            if (changed)
                File.WriteAllText(filePath, JsonConvert.SerializeObject(jObject, Formatting.Indented));
        }

        private static JObject? NavigateTo(JObject root, string[] segments)
        {
            JToken current = root;
            foreach (var segment in segments)
            {
                if (current is not JObject obj) return null;
                var prop = obj.Properties()
                    .FirstOrDefault(p => string.Equals(p.Name, segment, StringComparison.OrdinalIgnoreCase));
                if (prop == null) return null;
                current = prop.Value;
            }
            return current as JObject;
        }
    }
}
