namespace UNICAP.Compilador.Main
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("É necessário informar o caminho do arquivo de entrada.");
            }
            
            var codigo = LerCodigoDoArquivo(args[0]);

            AnalisadorLexico analisadorLexico = new AnalisadorLexico(codigo.ToCharArray());

            analisadorLexico.SalvarTokensNoArquivoDeSaida();
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