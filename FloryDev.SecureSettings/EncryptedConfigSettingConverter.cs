using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloryDev.SecureSettings
{
    internal class EncryptedConfigSettingConverter : TypeConverter  
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var casted = value as string;
            return casted != null
                ? new EncryptedConfigSetting() { Value = casted}
                : base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var casted = value as EncryptedConfigSetting;
            return destinationType == typeof(string) && casted != null
                ? casted.Value
                : base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
