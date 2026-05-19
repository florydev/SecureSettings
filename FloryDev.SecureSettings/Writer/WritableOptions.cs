using FloryDev.SecureSettings.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FloryDev.SecureSettings.Writer
{
    /// <summary>
    /// This adds a different type of Option pattern to the system to allow it ot be writable back to the file/section where the options 
    /// comes from.    
    /// This code was taken and then updated from this post here: https://learn.microsoft.com/en-us/answers/questions/609232/how-to-save-the-updates-i-made-to-appsettings-conf
    /// </summary>

    /// </summary>

    public class WritableOptions<T> : ISecuredOptions<T> where T : class, new()
    {
        private readonly IHostEnvironment _environment;
        private readonly IOptionsMonitor<T> _options;
        private readonly IConfigurationRoot _configuration;
        private readonly string _section;
        private readonly string _file;

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

        public T Value => _options.CurrentValue;
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
