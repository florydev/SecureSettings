using FloryDev.SecureSettings.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace FloryDev.SecureSettings.Writer
{
    public class WritableOptions<T> : ISecuredOptions<T> where T : class, new()
    {
        private static readonly PropertyInfo[] _securedProperties =
            typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                     .Where(p => p.PropertyType == typeof(SecuredConfigSetting))
                     .ToArray();

        private readonly IHostEnvironment _environment;
        private readonly IOptionsMonitor<T> _options;
        private readonly IConfigurationRoot _configuration;
        private readonly string _section;
        private readonly string _file;
        private bool _autoFlushChecked;

        public WritableOptions(
            IHostEnvironment environment,
            IOptionsMonitor<T> options,
            IConfigurationRoot configuration,
            string section,
            string file)
        {
            _environment = environment;
            _options = options;
            _configuration = configuration;
            _section = section;
            _file = file;
        }

        public T Value
        {
            get
            {
                if (!_autoFlushChecked)
                {
                    // Set flag before calling Update to prevent re-entrance if Update falls back to Value
                    _autoFlushChecked = true;
                    var current = _options.CurrentValue;
                    if (_securedProperties.Any(p => (p.GetValue(current) as SecuredConfigSetting)?.WasSecured == true))
                        Update(opt => { });
                }
                return _options.CurrentValue;
            }
        }

        public T Get(string name) => _options.Get(name);

        public void Update(Action<T> applyChanges)
        {
            var fileProvider = _environment.ContentRootFileProvider;
            var fileInfo = fileProvider.GetFileInfo(_file);
            var physicalPath = fileInfo.PhysicalPath;

            var jObject = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(physicalPath));
            var segments = _section.Split(':');

            var sectionObject = GetNestedToken(jObject, segments) is JToken token
                ? JsonConvert.DeserializeObject<T>(token.ToString())
                : (Value ?? new T());

            applyChanges(sectionObject);

            SetNestedToken(jObject, segments, JObject.Parse(JsonConvert.SerializeObject(sectionObject)));
            File.WriteAllText(physicalPath, JsonConvert.SerializeObject(jObject, Formatting.Indented));
            _configuration.Reload();
        }

        private static JToken? GetNestedToken(JObject root, string[] segments)
        {
            JToken current = root;
            foreach (var segment in segments)
            {
                current = (current as JObject)?[segment];
                if (current == null) return null;
            }
            return current;
        }

        private static void SetNestedToken(JObject root, string[] segments, JToken value)
        {
            JObject current = root;
            for (int i = 0; i < segments.Length - 1; i++)
            {
                if (current[segments[i]] is not JObject child)
                {
                    child = new JObject();
                    current[segments[i]] = child;
                }
                current = child;
            }
            current[segments[^1]] = value;
        }
    }
}
