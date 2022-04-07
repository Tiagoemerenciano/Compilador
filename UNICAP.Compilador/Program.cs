namespace UNICAP.Compilador.Main
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var code = ReadCodeFile("entrada_teste1.txt");

            LexicalAnalyser lexicalAnalyser = new(code.ToCharArray());

            lexicalAnalyser.SaveTokensInFile();
        }

        public static string ReadCodeFile(string fileName)
        {
            using (var codeText = new StreamReader(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName)))
            {
                return codeText.ReadToEnd();
            }
        }
    }
}