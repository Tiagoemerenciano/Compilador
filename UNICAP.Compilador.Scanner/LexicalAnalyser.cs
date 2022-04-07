using System.Text;
using UNICAP.Compilador.Lexical;
using UNICAP.Compilador.Utils;

namespace UNICAP.Compilador;

public class LexicalAnalyser
{
    public const string NAME_OUTPUT_FOLDER = "output.txt";

    public char[] Content { get; set; }
    public int Estado { get; set; } = 0;
    public int Position { get; set; } = 0;
    public int Linha { get; set; } = 1;
    public int Coluna { get; set; } = 0;
    public List<Token> Tokens { get; set; } = new List<Token>();
    public List<LexicalException> Errors { get; set; } = new List<LexicalException>();

    public LexicalAnalyser(char[] content)
    {
        Content = content;
        while (true)
        {
            var token = GetNextToken();

            if (token != null)
            {
                try
                {
                    Tokens.Add(token);
                }
                catch (LexicalException exception)
                {
                    Errors.Add(exception);
                }
            }
            else
            {
                Position = 0;
                break;
            }
        }
    }

    public void SaveTokensInFile()
    {
        using (StreamWriter streamWriter = new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), NAME_OUTPUT_FOLDER)))
        {
            foreach (var token in Tokens)
            {
                streamWriter.WriteLine(token.ToString());
            }
        }
    }

    private Token? GetNextToken()
    {
        if (FimDeArquivo())
        {
            return null;
        }

        var lexema = new StringBuilder();
        Estado = 0;

        while (true)
        {
            var caracterAtual = ProximoCaracter();

            switch (Estado)
            {
                case 0:
                    if (caracterAtual.IsDigito())
                    {
                        Estado = 1;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual == '\'')
                    {
                        Estado = 4;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual == 'i')
                    {
                        Estado = 13;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual == 'c')
                    {
                        Estado = 16;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual == 'f')
                    {
                        Estado = 19;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual == 'm')
                    {
                        Estado = 22;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual == 'e')
                    {
                        Estado = 25;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual == 'w')
                    {
                        Estado = 28;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual == 'd')
                    {
                        Estado = 31;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual == '_' || caracterAtual.IsLetra())
                    {
                        Estado = 7;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual == '=' || caracterAtual == '<' || caracterAtual == '>')
                    {
                        Estado = 9;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual == '!')
                    {
                        Estado = 11;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsCaracterEspecial())
                    {
                        Estado = 12;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsOperadorAritmetico())
                    {
                        Estado = 33;
                        lexema.Append(caracterAtual);
                    }
                    else if (!caracterAtual.IsFimDaLinha())
                    {
                        lexema.Append(caracterAtual);
                        LancarExcecaoLexica(lexema);
                    }
                    break;
                case 1:
                    if (caracterAtual == '.')
                    {
                        Estado = 2;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito())
                    {
                        lexema.Append(caracterAtual);
                    }
                    else
                    {
                        LancarExcecaoLexica(lexema);
                    }
                    break;
                case 2:
                    if (caracterAtual.IsDigito())
                    {
                        Estado = 3;
                        lexema.Append(caracterAtual);
                    }
                    else
                    {
                        LancarExcecaoLexica(lexema);
                    }
                    break;
                case 3:
                    if (!caracterAtual.IsDigito())
                    {
                        Estado = 0;
                        return Token(TipoToken.FLOAT, lexema);
                    }
                    lexema.Append(caracterAtual);
                    break;
                case 4:
                    if (caracterAtual.IsLetra())
                    {
                        Estado = 5;
                        lexema.Append(caracterAtual);
                    }
                    else
                    {
                        LancarExcecaoLexica(lexema);
                    }
                    break;
                case 5:
                    if (caracterAtual == '\'')
                    {
                        Estado = 6;
                        lexema.Append(caracterAtual);
                    }
                    else
                    {
                        LancarExcecaoLexica(lexema);
                    }
                    break;
                case 6:
                    Estado = 0;
                    return new Token(TipoToken.CHAR, lexema.ToString(), Linha, Coluna);
                case 7:
                    if (caracterAtual.IsLetra() || caracterAtual.IsDigito() || caracterAtual == '_')
                    {
                        Estado = 8;
                        lexema.Append(caracterAtual);
                    }
                    else
                    {
                        Estado = 0;
                        return Token(TipoToken.IDENTIFICADOR, lexema);
                    }
                    break;
                case 8:
                    if (!caracterAtual.IsLetra() && !caracterAtual.IsDigito() && caracterAtual != '_')
                    {
                        Estado = 0;
                        return Token(TipoToken.IDENTIFICADOR, lexema);
                    }
                    lexema.Append(caracterAtual);
                    break;
                case 9:
                    if (caracterAtual != '=')
                    {
                        return Token(TipoToken.OPERADOR_ATRIBUICAO, lexema);
                    }
                    Estado = 10;
                    lexema.Append(caracterAtual);
                    break;
                case 10:
                    Estado = 0;
                    return Token(TipoToken.OPERADOR_RELACIONAL, lexema);
                case 11:
                    if (caracterAtual == '=')
                    {
                        Estado = 10;
                        lexema.Append(caracterAtual);
                    }
                    else
                    {
                        LancarExcecaoLexica(lexema);
                    }
                    break;
                case 12:
                    Estado = 0;
                    return Token(TipoToken.CARACTER_ESPECIAL, lexema);
                case 13:
                    if (caracterAtual == 'f')
                    {
                        Estado = 14;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual == 'n')
                    {
                        Estado = 15;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito() || caracterAtual.IsLetra() || caracterAtual == '_' || caracterAtual.IsFimDaLinha())
                    {
                        Estado = 8;
                        lexema.Append(caracterAtual);
                    }
                    else
                    {
                        LancarExcecaoLexica(lexema);
                    }
                    break;
                case 14:
                    Estado = 0;
                    return Token(TipoToken.PALAVRA_RESERVADA, lexema);
                case 15:
                    if (caracterAtual == 't')
                    {
                        Estado = 14;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito() || caracterAtual.IsLetra() || caracterAtual == '_' || caracterAtual.IsFimDaLinha() || FimDeArquivo())
                    {
                        Estado = 8;
                        lexema.Append(caracterAtual);
                    }
                    else
                    {
                        LancarExcecaoLexica(lexema);
                    }
                    break;
                case 16:
                    if (caracterAtual == 'h')
                    {
                        Estado = 17;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito() || caracterAtual.IsLetra() || caracterAtual == '_' || caracterAtual.IsFimDaLinha())
                    {
                        Estado = 8;
                        lexema.Append(caracterAtual);
                    }
                    else
                    {
                        LancarExcecaoLexica(lexema);
                    }
                    break;
                case 17:
                    if (caracterAtual == 'a')
                    {
                        Estado = 18;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito() || caracterAtual.IsLetra() || caracterAtual == '_' || caracterAtual.IsFimDaLinha())
                    {
                        Estado = 8;
                        lexema.Append(caracterAtual);
                    }
                    else
                    {
                        LancarExcecaoLexica(lexema);
                    }
                    break;
                case 18:
                    if (caracterAtual == 'r')
                    {
                        Estado = 14;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito() || caracterAtual.IsLetra() || caracterAtual == '_' || caracterAtual.IsFimDaLinha())
                    {
                        Estado = 8;
                        lexema.Append(caracterAtual);
                    }
                    else
                    {
                        LancarExcecaoLexica(lexema);
                    }
                    break;
                case 19:
                    if (caracterAtual == 'o')
                    {
                        Estado = 18;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual == 'l')
                    {
                        Estado = 20;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito() || caracterAtual.IsLetra() || caracterAtual == '_' || caracterAtual.IsFimDaLinha())
                    {
                        Estado = 8;
                        lexema.Append(caracterAtual);
                    }
                    else
                    {
                        LancarExcecaoLexica(lexema);
                    }
                    break;
                case 20:
                    if (caracterAtual == 'o')
                    {
                        Estado = 21;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito() || caracterAtual.IsLetra() || caracterAtual == '_' || caracterAtual.IsFimDaLinha())
                    {
                        Estado = 8;
                        lexema.Append(caracterAtual);
                    }
                    else
                    {
                        LancarExcecaoLexica(lexema);
                    }
                    break;
                case 21:
                    if (caracterAtual == 'a')
                    {
                        Estado = 15;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito() || caracterAtual.IsLetra() || caracterAtual == '_' || caracterAtual.IsFimDaLinha())
                    {
                        Estado = 8;
                        lexema.Append(caracterAtual);
                    }
                    else
                    {
                        LancarExcecaoLexica(lexema);
                    }
                    break;
                case 22:
                    if (caracterAtual == 'a')
                    {
                        Estado = 23;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito() || caracterAtual.IsLetra() || caracterAtual == '_' || caracterAtual.IsFimDaLinha())
                    {
                        Estado = 8;
                        lexema.Append(caracterAtual);
                    }
                    else
                    {
                        LancarExcecaoLexica(lexema);
                    }
                    break;
                case 23:
                    if (caracterAtual == 'i')
                    {
                        Estado = 24;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito() || caracterAtual.IsLetra() || caracterAtual == '_' || caracterAtual.IsFimDaLinha())
                    {
                        Estado = 8;
                        lexema.Append(caracterAtual);
                    }
                    else
                    {
                        LancarExcecaoLexica(lexema);
                    }
                    break;
                case 24:
                    if (caracterAtual == 'n')
                    {
                        Estado = 14;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito() || caracterAtual.IsLetra() || caracterAtual == '_' || caracterAtual.IsFimDaLinha())
                    {
                        Estado = 8;
                        lexema.Append(caracterAtual);
                    }
                    else
                    {
                        LancarExcecaoLexica(lexema);
                    }
                    break;
                case 25:
                    if (caracterAtual == 'l')
                    {
                        Estado = 26;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito() || caracterAtual.IsLetra() || caracterAtual == '_' || caracterAtual.IsFimDaLinha())
                    {
                        Estado = 8;
                        lexema.Append(caracterAtual);
                    }
                    else
                    {
                        LancarExcecaoLexica(lexema);
                    }
                    break;
                case 26:
                    if (caracterAtual == 's')
                    {
                        Estado = 27;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito() || caracterAtual.IsLetra() || caracterAtual == '_' || caracterAtual.IsFimDaLinha())
                    {
                        Estado = 8;
                        lexema.Append(caracterAtual);
                    }
                    else
                    {
                        LancarExcecaoLexica(lexema);
                    }
                    break;
                case 27:
                    if (caracterAtual == 'e')
                    {
                        Estado = 14;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito() || caracterAtual.IsLetra() || caracterAtual == '_' || caracterAtual.IsFimDaLinha())
                    {
                        Estado = 8;
                        lexema.Append(caracterAtual);
                    }
                    else
                    {
                        LancarExcecaoLexica(lexema);
                    }
                    break;
                case 28:
                    if (caracterAtual == 'h')
                    {
                        Estado = 29;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito() || caracterAtual.IsLetra() || caracterAtual == '_' || caracterAtual.IsFimDaLinha())
                    {
                        Estado = 8;
                        lexema.Append(caracterAtual);
                    }
                    else
                    {
                        LancarExcecaoLexica(lexema);
                    }
                    break;
                case 29:
                    if (caracterAtual == 'i')
                    {
                        Estado = 30;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito() || caracterAtual.IsLetra() || caracterAtual == '_' || caracterAtual.IsFimDaLinha())
                    {
                        Estado = 8;
                        lexema.Append(caracterAtual);
                    }
                    else
                    {
                        LancarExcecaoLexica(lexema);
                    }
                    break;
                case 30:
                    if (caracterAtual == 'l')
                    {
                        Estado = 27;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito() || caracterAtual.IsLetra() || caracterAtual == '_' || caracterAtual.IsFimDaLinha())
                    {
                        Estado = 8;
                        lexema.Append(caracterAtual);
                    }
                    else
                    {
                        LancarExcecaoLexica(lexema);
                    }
                    break;
                case 31:
                    if (caracterAtual == 'o')
                    {
                        Estado = 14;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito() || caracterAtual.IsLetra() || caracterAtual == '_' || caracterAtual.IsFimDaLinha())
                    {
                        Estado = 8;
                        lexema.Append(caracterAtual);
                    }
                    else
                    {
                        LancarExcecaoLexica(lexema);
                    }
                    break;
                case 33:
                    Estado = 0;
                    return Token(TipoToken.OPERADOR_ARITMETICO, lexema);
                    break;
                default:
                    LancarExcecaoLexica(lexema);
                    break;
            }
        }
    }

    private Token Token(TipoToken tipoToken, StringBuilder lexema)
    {
        Voltar();
        return new Token(tipoToken, lexema.ToString(), Linha, Coluna);
    }

    private void Voltar()
    {
        if (!FimDeArquivo())
        {
            Coluna -= 1;
            Position -= 1;
        }
    }

    private void IncrementarLinha()
    {
        Linha++;
        Coluna = 0;
    }

    private void LancarExcecaoLexica(StringBuilder lexema)
    {
        throw new LexicalException($"Caracter inválido encontrado: {lexema} | Linha: {Linha} | Coluna: {Coluna}");
    }

    private char ProximoCaracter()
    {
        if (FimDeArquivo())
            return '\0';

        var character = Content[Position];
        Position++;
        Coluna++;
        return character;
    }

    private bool FimDeArquivo()
    {
        return Position == Content.Length;
    }
}