using System.ComponentModel;
using System.Reflection;

namespace UNICAP.Compilador.Utils
{
    public static class EnumExtension
    {
        // Recuperar description do enum
        public static string GetDescription(this Enum value)
        {
            var @enum = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[]) @enum.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }
}
