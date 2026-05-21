namespace FloryDev.SecureSettings.Interfaces
{
    /// <summary>
    /// Retrieves the original value from a secured form.
    /// Implement this to provide a custom security mechanism (e.g. RSA, DPAPI, AES).
    /// Separated from <see cref="ISecureService"/> so a system can be restricted to
    /// securing values without being able to retrieve them.
    /// </summary>
    public interface IUnsecureService
    {
        /// <summary>Returns the original plain-text value from its secured form.</summary>
        string Unsecure(string securedValue);

        /// <summary>
        /// Returns true if the value is in secured form. Used to determine whether
        /// a value needs to be secured before storage.
        /// </summary>
        bool IsSecured(string value);
    }
}
