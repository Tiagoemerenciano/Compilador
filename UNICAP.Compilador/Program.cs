namespace UNICAP.Compilador.Main
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var codigo = LerCodigoDoArquivo("entrada_teste1.txt");

            AnalisadorLexico analisadorLexico = new AnalisadorLexico(codigo.ToCharArray());

            analisadorLexico.SalvarTokensNoArquivoDeSaida();
        }

        public static string LerCodigoDoArquivo(string nomeArquivo)
        {
            using (var textoCodigo = new StreamReader(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), nomeArquivo)))
            {
                return textoCodigo.ReadToEnd();
            }
        }
    }
}