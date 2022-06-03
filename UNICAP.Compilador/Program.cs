namespace UNICAP.Compilador.Main
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            string caminhoArquivo = RecuperarCaminhoArquivo(args);

            var codigo = LerCodigoDoArquivo(caminhoArquivo);

            AnalisadorLexico analisadorLexico = new(codigo.ToCharArray());
            new Parser.Parser(analisadorLexico);

            analisadorLexico.SalvarTokensNoArquivoDeSaida();
        }

        private static string RecuperarCaminhoArquivo(string[] args)
        {
            string caminhoArquivo;

            if (args.Length == 0)
            {
                caminhoArquivo = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\entrada_teste.txt";
            }
            else
            {
                caminhoArquivo = args[0];
            }

            return caminhoArquivo;
        }

        public static string LerCodigoDoArquivo(string caminhoArquivo)
        {
            using (var textoCodigo = new StreamReader(caminhoArquivo))
            {
                return textoCodigo.ReadToEnd();
            }
        }
    }
}