using UNICAP.Compilador.Lexical;
using UNICAP.Compilador.Utils;

namespace UNICAP.Compilador.Parser
{
    public class Sintaxe
    {
        public Sintaxe(Token token, TipoToken tipo, int escopo, string identificador)
        {
            Token = token;
            Tipo = tipo;
            Escopo = escopo;
            Identificador = identificador;
        }

        public Sintaxe(Token token, TipoToken tipo, int escopo)
        {
            Token = token;
            Tipo = tipo;
            Escopo = escopo;
        }

        public Token Token { get; set; }
        public TipoToken Tipo { get; set; }
        public int Escopo { get; set; }
        public string? Identificador { get; set; }
    }
}
