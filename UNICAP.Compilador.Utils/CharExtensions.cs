using System.Text.RegularExpressions;

namespace UNICAP.Compilador.Utils
{
    public static class CharExtensions
    {
        public static bool IsDigito(this char caracter)
        {
            return caracter >= '0' && caracter <= '9';
        }
        public static bool IsRelationalOperator(this char caracter)
        {
            return caracter is '<' || caracter is '>' || caracter is '=' || caracter is '!';
        }
        public static bool IsLetra(this char caracter)
        {
            return (caracter >= 'a' && caracter <= 'z') || (caracter >= 'A' && caracter <= 'Z') || caracter == '\'' || caracter == '_';
        }
        public static bool IsOperadorAritmetico(this char caracter)
        {
            return caracter is '+' || caracter is '-' || caracter is '*' || caracter is '/';
        }
        public static bool IsAttributionOperator(this char caracter)
        {
            return caracter is '=';
        }
        public static bool IsCaracterEspecial(this char caracter)
        {
            return caracter is ')' || caracter is '(' || caracter is '{' || caracter is '}' || caracter is ',' || caracter is ';';
        }
        public static bool IsConditionalOperator(this char caracter)
        {
            return caracter is '&' || caracter is '|';
        }
        public static bool IsOperator(this char caracter)
        {
            return IsRelationalOperator(caracter) || IsOperadorAritmetico(caracter) || IsConditionalOperator(caracter);
        }
        public static bool IsExclamation(this char caracter)
        {
            return caracter is '!';
        }
        public static bool IsSpace(this char caracter)
        {
            return char.IsWhiteSpace(caracter);
        }
        public static bool IsFimDaLinha(this char caracter)
        {
            return caracter is '\n' || caracter is '\r';
        }
        public static bool IsAssignmentOperator(this char caracter)
        {
            return caracter == '=';
        }
    }
}
