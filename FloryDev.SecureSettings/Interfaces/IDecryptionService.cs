namespace FloryDev.SecureSettings.Interfaces
{
    /// <summary>
    /// This is the interface for decryption of secure settings. I expect that typically both encryption and decryption might be handled
    /// by one object in some scenarios you might not even want to have a decrypter object so that is primarily why they are seperated. An 
    /// example of this is where one system is the public key of another, it cannot decrypt any data but it can encrypt it for the other
    /// system's use.
    /// </summary>
    public interface IDecryptionService
    {
        /// <summary>
        /// Decrypts the given encrypted string.
        /// </summary>
        /// <param name="encryptedString">The string to decrypt.</param>
        /// <returns>The decrypted string.</returns>
        string DecryptString(string encryptedString);

        /// <summary>
        /// Checks if the given value is encrypted. In most scenarios the only way to really check this is to actually decrypt the value
        /// but this method exists to manage exceptions better
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the value is encrypted, otherwise false.</returns>
        bool ValueIsEncrypted(string value);
    }
}
