namespace FloryDev.SecureSettings.Interfaces
{
    public interface IDecrypter
    {
        string DecryptString(string encryptedString);
        bool ValueIsEncrypted(string value);
    }
}
