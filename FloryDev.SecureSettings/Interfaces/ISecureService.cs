namespace FloryDev.SecureSettings.Interfaces
{
    /// <summary>
    /// Secures a plain-text value so it can be safely stored in configuration.
    /// Implement this to provide a custom security mechanism (e.g. RSA, DPAPI, AES).
    /// Separated from <see cref="IUnsecureService"/> to support asymmetric scenarios
    /// where one system can secure values but not retrieve them.
    /// </summary>
    public interface ISecureService
    {
        /// <summary>Secures the given plain-text value and returns the secured form.</summary>
        string Secure(string value);
    }
}
