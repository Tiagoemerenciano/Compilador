using System.ComponentModel;

namespace UNICAP.Compilador.Utils
{
    public enum TipoToken
    {
        [Description("Float")]
        FLOAT = 0,
        [Description("Char")]
        CHAR = 1,
        [Description("Identificador")]
        IDENTIFICADOR = 2,
        [Description("Operador Relacional")]
        OPERADOR_RELACIONAL = 3,
        [Description("Operador Aritmético")]
        OPERADOR_ARITMETICO = 4,
        [Description("Operador Atribuição")]
        OPERADOR_ATRIBUICAO = 5,
        [Description("Caracter Especial")]
        CARACTER_ESPECIAL = 6,
        [Description("Palavra Reservada")]
        PALAVRA_RESERVADA = 7,
        [Description("String")]
        STRING = 8,
        [Description("Inteiro")]
        INTEIRO = 9
    }
}
