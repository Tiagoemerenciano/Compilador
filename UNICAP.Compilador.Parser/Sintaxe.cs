using UNICAP.Compilador.Lexical;

namespace UNICAP.Compilador.Parser
{
    public class Sintaxe
    {
        public Sintaxe(Token token, int tipo, int escopo, string identificador)
        {
            Token = token;
            Tipo = tipo;
            Escopo = escopo;
            Identificador = identificador;
        }

        public Sintaxe(Token token, int tipo, int escopo)
        {
            Token = token;
            Tipo = tipo;
            Escopo = escopo;
        }

        public Token Token { get; set; }
        public int Tipo { get; set; }
        public int Escopo { get; set; }
        public string? Identificador { get; set; }
    }
}
