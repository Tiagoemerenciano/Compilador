using System.ComponentModel;

namespace UNICAP.Compilador.Utils
{
    public enum TipoToken
    {
        [Description("float")]
        FLOAT = 0,
        [Description("char")]
        CHAR = 1,
        [Description("identificador")]
        IDENTIFICADOR = 2,
        [Description("operador relacional")]
        OPERADOR_RELACIONAL = 3,
        [Description("operador aritmético")]
        OPERADOR_ARITMETICO = 4,
        [Description("operador atribuição")]
        OPERADOR_ATRIBUICAO = 5,
        [Description("caracter especial")]
        CARACTER_ESPECIAL = 6,
        [Description("palavra reservada")]
        PALAVRA_RESERVADA = 7,
        [Description("string")]
        STRING = 8,
        [Description("int")]
        INTEIRO = 9
    }
}
