using System.ComponentModel;

namespace UNICAP.Compilador.Utils
{
    public static class EnumExtension
    {
        public static string GetDescription(this Enum valor)
        {
            var @enum = valor.GetType().GetField(valor.ToString());

            DescriptionAttribute[] atributosDoCampo = (DescriptionAttribute[]) @enum.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (atributosDoCampo != null && atributosDoCampo.Length > 0)
                return atributosDoCampo[0].Description;
            else
                return valor.ToString();
        }

        // Recuperar valor de enum pela description
        public static T GetValueFromDescription<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            return default(T);
        }
    }
}
