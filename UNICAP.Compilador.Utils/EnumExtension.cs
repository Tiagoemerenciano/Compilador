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
    }
}
