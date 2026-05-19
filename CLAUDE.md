# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run Commands

```powershell
# Build the full solution
dotnet build FloryDev.SecureSettings.sln

# Build a specific project
dotnet build FloryDev.SecureSettings/FloryDev.SecureSettings.csproj

# Pack NuGet packages (Release)
dotnet pack FloryDev.SecureSettings/FloryDev.SecureSettings.csproj -c Release
dotnet pack FloryDev.SecureSettings.WindowsEncryption/FloryDev.SecureSettings.WindowsEncryption.csproj -c Release

# Run the reference implementation (Windows only — requires the Windows key container)
dotnet run --project FloryDev.SecureSettings.ReferenceImplementation
```

There are no automated tests in this repo yet.

## Solution Structure

Three projects, two are NuGet packages and one is a runnable demo:

| Project | Purpose |
|---|---|
| `FloryDev.SecureSettings` | Core library — encryption abstractions, `EncryptedConfigSetting`, DI wiring |
| `FloryDev.SecureSettings.WindowsEncryption` | Windows-only `IEncryptDecryptService` implementation using `RSACryptoServiceProvider` with a named key container |
| `FloryDev.SecureSettings.ReferenceImplementation` | .NET Worker Service demo showing end-to-end usage with shadow settings and EF Core |

## Architecture

### Core Data Flow

Settings classes (POCOs) use `EncryptedConfigSetting` as the property type for secrets:

```csharp
public class AppSettings {
    public EncryptedConfigSetting Password { get; set; }
}
```

When the config system binds a plain-text string from JSON, `EncryptedConfigSettingConverter` (a `TypeConverter`) intercepts the conversion and creates an `EncryptedConfigSetting`. Setting `.Value` on that object immediately encrypts the value if it is not already encrypted. Callers decrypt on demand via `GetDecryptedValue()`, which avoids keeping the plaintext in memory.

### The Static Wiring Problem

DI does not work inside `IOptions`-bound POCOs. `EncryptedConfigSetting` therefore holds its `IEncryptionService` and `IDecryptionService` as **static** properties. `SecureSettingsManager` is the singleton that sets those statics at startup; it must be registered **and resolved** (via injection into a hosted service or similar) before any `EncryptedConfigSetting.Value` is accessed.

```csharp
// Required registrations — order matters at resolve time
builder.Services.AddSingleton<IEncryptionService, EncryptionProvider>();
builder.Services.AddSingleton<IDecryptionService, EncryptionProvider>();
builder.Services.AddSingleton<SecureSettingsManager>();  // wires up the statics
```

`ConfigurationExtensions.GetSecuredConnectionString()` shares the same static pattern and is also wired by `SecureSettingsManager`.

### Write-back via `ISecuredOptions<T>`

`ConfigureSecured<T>()` registers `ISecuredOptions<T>` (backed by `WritableOptions<T>`). Calling `.Update(opt => { ... })` on it serialises the entire settings section back to the JSON file with Newtonsoft.Json and then calls `IConfigurationRoot.Reload()`. This is how a plain-text value in the file gets replaced with its encrypted form on first run.

### Shadow Settings Pattern

The reference implementation uses a `shadow.json` file (not in source control) for developer-specific credentials. The pattern detects `Debugger.IsAttached` to decide which file to use:

```csharp
if (Debugger.IsAttached)
    builder.Services.ConfigureSecured<AppSettings>(shadow.GetSection(...), "shadow.json");
else
    builder.Services.ConfigureSecured<AppSettings>(builder.Configuration.GetSection(...));
```

`shadow.json` mirrors the shape of the relevant section of `appsettings.json`. Each developer's encrypted values are keyed to their own Windows key container, so they cannot decrypt each other's values.

### Windows Encryption

`EncryptionProvider` uses `RSACryptoServiceProvider` with a named CNG key container (`KeyContainerName` in the `WindowsSecuritySettings` config section). The key is stored in the Windows Key Storage Provider — the user never sees or manages the key directly. `ValueIsEncrypted` is currently implemented by attempting decryption and catching exceptions (acknowledged as a placeholder in the code).

## NuGet Publishing Notes

`FloryDev.SecureSettings` bundles `Docs/README.md` as the package readme. `FloryDev.SecureSettings.WindowsEncryption` references the core package by NuGet ID (not a project reference), so bumping the core package version requires updating the `<PackageReference>` version in the Windows project before publishing.
