using UNICAP.Compilador.Lexical;

namespace UNICAP.Compilador.Parser
{
    public class Sintaxe
    {
        public Token Token { get; set; }
        public int Tipo { get; set; }
        public int Escopo { get; set; }
        public string Lexema { get; set; }
    }
}
