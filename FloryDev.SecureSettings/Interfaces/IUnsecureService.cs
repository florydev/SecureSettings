namespace FloryDev.SecureSettings.Interfaces
{
    /// <summary>
    /// Retrieves the original value from a secured form.
    /// Implement this to provide a custom security mechanism (e.g. RSA, DPAPI, Key Vault).
    /// Separated from <see cref="ISecureService"/> so a system can be restricted to
    /// securing values without being able to retrieve them — or retrieval-only as with
    /// a read-only Key Vault runtime provider.
    /// </summary>
    public interface IUnsecureService
    {
        /// <summary>Returns the original plain-text value from its secured form.</summary>
        Task<string> UnsecureAsync(string securedValue);

        /// <summary>
        /// Returns true if the value is in secured form. Used to determine whether
        /// a value needs to be secured before storage. Always synchronous — this is
        /// a local string check with no I/O.
        /// </summary>
        bool IsSecured(string value);
    }
}
