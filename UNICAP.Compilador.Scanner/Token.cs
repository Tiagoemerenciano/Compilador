using UNICAP.Compilador.Utils;

namespace UNICAP.Compilador.Lexical
{
    public class Token
    {
        public TipoToken Tipo { get; set; }
        public string Lexema { get; }
        public int Linha { get; }
        public int Coluna { get; }

        public Token(TipoToken tipo, string termo, int linha, int coluna)
        {
            Tipo = tipo;
            Lexema = termo;
            Linha = linha;
            Coluna = coluna;
        }

        public override string ToString()
        {
            return $"Tipo: {Tipo.GetDescription()} | " +
                   $"Texto: {Lexema} | " +
                   $"Linha: {Linha} | " +
                   $"Coluna: {Coluna}";
        }
    }
}
