namespace UNICAP.Compilador.Utils
{
    public static class CharExtensions
    {
        public static bool IsDigito(this char caracter)
        {
            return caracter >= '0' && caracter <= '9';
        }
        public static bool IsLetra(this char caracter)
        {
            return (caracter >= 'a' && caracter <= 'z') || (caracter >= 'A' && caracter <= 'Z') || caracter == '\'' || caracter == '_';
        }
        public static bool IsOperadorAritmetico(this char caracter)
        {
            return caracter is '+' || caracter is '-' || caracter is '*' || caracter is '/';
        }
        public static bool IsCaracterEspecial(this char caracter)
        {
            return caracter is ')' || caracter is '(' || caracter is '{' || caracter is '}' || caracter is ',' || caracter is ';';
        }
        public static bool IsEspaco(this char caracter)
        {
            return char.IsWhiteSpace(caracter);
        }
        public static bool IsFimDaLinha(this char caracter)
        {
            return caracter is '\n';
        }
    }
}
