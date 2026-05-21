namespace FloryDev.SecureSettings.Interfaces
{
    /// <summary>
    /// Combines <see cref="ISecureService"/> and <see cref="IUnsecureService"/> for
    /// providers that handle both directions.
    /// </summary>
    public interface ISecureUnsecureService : ISecureService, IUnsecureService
    {
    }
}
