using System.Text.RegularExpressions;

namespace UNICAP.Compilador.Utils
{
    public static class StringExtensions
    {
        public static bool IsReservedWord(this string term)
        {
            var reservedWords = new List<string>()
            {
                "main",
                "if",
                "else",
                "while",
                "do",
                "for",
                "int",
                "float",
                "char"
            };

            return reservedWords.Contains(term);
        }

        public static bool IsFloat(this string term)
        {
            return Regex.IsMatch(term, @"^[+-]?(\d+((\.)\d*)?\d+)([eE][+-]?\d+)?$");
        }

        public static bool IsInteger(this string term)
        {
            return Regex.IsMatch(term, @"^[+-]?(\d+)$");
        }

        public static bool IsRelationalOperator(this string term)
        {
            return term == "<=" || term == ">=" || term == "==" || term == "!=";
        }

        public static bool IsArithmeticOperator(this string term)
        {
            return term == "/" || term == "*" || term == "-" || term == "+";
        }

        public static bool IsAssignmentOperator(this string term)
        {
            return term == "=";
        }
    }
}
