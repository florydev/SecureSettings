namespace FloryDev.SecureSettings.ConnectionStrings
{
    internal static class SecureConnectionStringParser
    {
        private static readonly HashSet<string> SensitiveKeys = new(StringComparer.OrdinalIgnoreCase)
        {
            "password", "pwd", "user id", "uid", "user", "userid"
        };

        public static bool IsSensitiveKey(string key) => SensitiveKeys.Contains(key.Trim());

        public static bool IsEncryptedValue(string value) => SecureValueEncoding.IsEncrypted(value);

        public static string Wrap(string base64) => SecureValueEncoding.Wrap(base64);

        public static string Unwrap(string wrapped) => SecureValueEncoding.Unwrap(wrapped);

        public static List<(string Key, string Value, bool IsSensitive)> Parse(string connectionString)
        {
            var parts = new List<(string, string, bool)>();
            foreach (var segment in connectionString.Split(';'))
            {
                if (string.IsNullOrWhiteSpace(segment)) continue;
                var idx = segment.IndexOf('=');
                if (idx < 0) continue;
                var key = segment.Substring(0, idx).Trim();
                var value = segment.Substring(idx + 1);
                parts.Add((key, value, IsSensitiveKey(key)));
            }
            return parts;
        }

        public static string Reconstruct(List<(string Key, string Value, bool IsSensitive)> parts) =>
            string.Join(";", parts.Select(p => $"{p.Key}={p.Value}")) + ";";
    }
}
