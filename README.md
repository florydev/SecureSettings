# SecureSettings

SecureSettings is a .NET library to encrypt settings values in a more natural way, where the values can be changed more simply, automatically encrypted (still working on this), and does not rely on explicitly stored keys.

## How It Works
In any settings class you are using, usually serialized from appsettings.json you provide properties that are EncyrptedConfigSetting, like so:

    public EncryptedConfigSetting Password { get; set; }

This class would map to a section of the appsetting.json that for example might look like this:
  *"AppSettings": {
    "Username": "admin",
    "Password": "MyPassword"
  },*
In your application host setup you would bind the class from the configuration like so:

    builder.Services.ConfigureSecured<AppSettings>(builder.Configuration.GetSection("AppSettings"));

When your Hosted Service is instantiated it will automatically encrypt the value in the settings which is then written back to the file (still working on finding a way to do this automatically).

        public Worker(ILogger<Worker> logger, SecureSettingsManager secureSettingsManager, ISecuredOptions<AppSettings> settings)
        {
            Settings = settings.Value;
            settings.Update(opt => {
                opt = Settings;
            });
        }
By doing this the setting is automatically encrypted if it is in as plain text when it is first run and written back to the settings file.

The advantages of this pattern are:

 1. If a setting value needs to be changed it can just be modified in
    the settings file and the application reloaded and it will be
    re-encrypted.
 2.  Often plain text secrets are often often put in settings files
    for specific environments with the idea that the production file is
    held on a system with limited access. There is a lot of points of
    failure in such a system.
 3. Development credentials are often placed in the config file and then
    leaked out to code repositories or from file sharing

## Defining an Encryption Service
SecureSetting requires you to give a specific service for encryption and decryption which can be set with Dependency Injection as so:

    >             builder.Services.AddSingleton<IEncryptionService, EncryptionProvider>();
    >             builder.Services.AddSingleton<IDecryptionService, EncryptionProvider>();
The Windows Reference implementation provided show how this is meant to be implemented so that the keys used to encrypt the data are stored within a systems keystore. This means each user does not directly manage, or even has access to, the keys used to encrypt the data. By using a system like this it is possible to run service level applications through a system user from which no one can access the encryption key.

## Accessing Encrypted Data
You can access the value of an encrypted configuration parameter with a method call:

    var password = Settings.Password.GetDecryptedValue();

The reason it is a method call is to keep the decrypted value from being in memory. I have considered implementing this with a SecureString but the issue there is in order to access the value it will require the library to be marked ***unsafe*** which is not entirely clear. I will probably make a separate library for those that want to use it that way (it will seem a bit ironic to make a secure library that is marked unsafe).
The other advantage of doing it this way is it is far easier to determine in code WHEN the decrypted values are being accessed and you could easily add static analysis rules to find these cases and code review the access.

## Shadow Settings
With this system because the executing user instance is using a specific machine keystore when you have multiple people accessing the same configuration they won't be able to decrypt the data. They could enter in the plain text value of the setting, which would then be encrypted with their keystore 
>
> Written with [StackEdit](https://stackedit.io/).