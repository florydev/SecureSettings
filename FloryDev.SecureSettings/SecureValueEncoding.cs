namespace FloryDev.SecureSettings
{
    public static class SecureValueEncoding
    {
        private const string Prefix = "{-FDSS:";
        private const string Suffix = "-}";

        public static bool IsSecured(string value) =>
            value.StartsWith(Prefix) && value.EndsWith(Suffix);

        public static string Wrap(string base64) => $"{Prefix}{base64}{Suffix}";

        public static string Unwrap(string wrapped) =>
            wrapped.Substring(Prefix.Length, wrapped.Length - Prefix.Length - Suffix.Length);
    }
}
