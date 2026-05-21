using System.ComponentModel;
using System.Globalization;

namespace FloryDev.SecureSettings
{
    internal class SecuredConfigSettingConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) =>
            sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

        public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            var casted = value as string;
            return casted != null
                ? new SecuredConfigSetting { Value = casted }
                : base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            var casted = value as SecuredConfigSetting;
            return destinationType == typeof(string) && casted != null
                ? casted.Value
                : base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
