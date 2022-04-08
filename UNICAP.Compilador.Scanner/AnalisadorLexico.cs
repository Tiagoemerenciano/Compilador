using System.Text;
using UNICAP.Compilador.Lexical;
using UNICAP.Compilador.Utils;

namespace UNICAP.Compilador;

public class AnalisadorLexico
{
    public const string NOME_ARQUIVO_SAIDA = "tokens_lidos.txt";

    public char[] ConteudoArquivo { get; set; }
    public int Estado { get; set; } = 0;
    public int Posicao { get; set; } = 0;
    public int Linha { get; set; } = 1;
    public int Coluna { get; set; } = 1;
    public List<Token> Tokens { get; set; } = new List<Token>();

    public AnalisadorLexico(char[] conteudoArquivo)
    {
        ConteudoArquivo = conteudoArquivo.Append(' ').ToArray();
        while (true)
        {
            var token = GetNextToken();

            if (token == null)
            {
                Posicao = 0;
                break;
            }
            else
            {
                Tokens.Add(token);
            }
        }
    }

    public void SalvarTokensNoArquivoDeSaida()
    {
        using (StreamWriter streamWriter = new(Environment.CurrentDirectory + NOME_ARQUIVO_SAIDA))
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

            if (caracterAtual.IsFimDaLinha())
            {
                IncrementarLinha();
            }

            switch (Estado)
            {
                case 0:
                    if (caracterAtual.IsDigito())
                    {
                        Estado = 1;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual is '\'')
                    {
                        Estado = 4;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual is 'i')
                    {
                        Estado = 13;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual is 'c')
                    {
                        Estado = 16;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual is 'f')
                    {
                        Estado = 19;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual is 'm')
                    {
                        Estado = 22;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual is 'e')
                    {
                        Estado = 25;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual is 'w')
                    {
                        Estado = 28;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual is 'd')
                    {
                        Estado = 31;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual is '_' || caracterAtual.IsLetra())
                    {
                        Estado = 7;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual is '<' || caracterAtual is '>')
                    {
                        Estado = 9;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual is '=')
                    {
                        Estado = 34;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual is '!')
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
                    else if (caracterAtual is '"')
                    {
                        Estado = 35;
                        lexema.Append(caracterAtual);
                    }
                    else if (!caracterAtual.IsFimDaLinha() && !caracterAtual.IsEspaco())
                    {
                        lexema.Append(caracterAtual);
                        LancarExcecaoLexica(lexema);
                    }
                    break;
                case 1:
                    if (caracterAtual is '.')
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
                        return Token(TipoToken.INTEIRO, lexema);
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
                    if (caracterAtual.IsLetra() || caracterAtual.IsDigito())
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
                    if (caracterAtual is '\'')
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
                    if (caracterAtual.IsLetra() || caracterAtual.IsDigito())
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
                    if (!caracterAtual.IsLetra() && !caracterAtual.IsDigito())
                    {
                        Estado = 0;
                        return Token(TipoToken.IDENTIFICADOR, lexema);
                    }
                    lexema.Append(caracterAtual);
                    break;
                case 9:
                    if (caracterAtual is '=')
                    {
                        Estado = 10;
                        lexema.Append(caracterAtual);
                    }
                    else
                    {
                        Estado = 0;
                        return Token(TipoToken.OPERADOR_ATRIBUICAO, lexema);
                    }
                    break;
                case 10:
                    Estado = 0;
                    return Token(TipoToken.OPERADOR_RELACIONAL, lexema);
                case 11:
                    if (caracterAtual is '=')
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
                    if (caracterAtual is 'f')
                    {
                        Estado = 14;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual is 'n')
                    {
                        Estado = 15;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito() || caracterAtual.IsLetra() || caracterAtual.IsFimDaLinha())
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
                case 14:
                    Estado = 0;
                    return Token(TipoToken.PALAVRA_RESERVADA, lexema);
                case 15:
                    if (caracterAtual is 't')
                    {
                        Estado = 14;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito() || caracterAtual.IsLetra() || caracterAtual.IsFimDaLinha())
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
                case 16:
                    if (caracterAtual is 'h')
                    {
                        Estado = 17;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito() || caracterAtual.IsLetra() || caracterAtual.IsFimDaLinha())
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
                case 17:
                    if (caracterAtual is 'a')
                    {
                        Estado = 18;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito() || caracterAtual.IsLetra() || caracterAtual.IsFimDaLinha())
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
                case 18:
                    if (caracterAtual is 'r')
                    {
                        Estado = 14;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito() || caracterAtual.IsLetra() || caracterAtual.IsFimDaLinha())
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
                case 19:
                    if (caracterAtual is 'o')
                    {
                        Estado = 18;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual is 'l')
                    {
                        Estado = 20;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito() || caracterAtual.IsLetra() || caracterAtual.IsFimDaLinha())
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
                case 20:
                    if (caracterAtual is 'o')
                    {
                        Estado = 21;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito() || caracterAtual.IsLetra() || caracterAtual.IsFimDaLinha())
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
                case 21:
                    if (caracterAtual == 'a')
                    {
                        Estado = 15;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito() || caracterAtual.IsLetra() || caracterAtual.IsFimDaLinha())
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
                case 22:
                    if (caracterAtual == 'a')
                    {
                        Estado = 23;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito() || caracterAtual.IsLetra() || caracterAtual.IsFimDaLinha())
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
                case 23:
                    if (caracterAtual == 'i')
                    {
                        Estado = 24;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito() || caracterAtual.IsLetra() || caracterAtual.IsFimDaLinha())
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
                case 24:
                    if (caracterAtual == 'n')
                    {
                        Estado = 14;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito() || caracterAtual.IsLetra() || caracterAtual.IsFimDaLinha())
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
                case 25:
                    if (caracterAtual == 'l')
                    {
                        Estado = 26;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito() || caracterAtual.IsLetra() || caracterAtual.IsFimDaLinha())
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
                case 26:
                    if (caracterAtual == 's')
                    {
                        Estado = 27;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito() || caracterAtual.IsLetra() || caracterAtual.IsFimDaLinha())
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
                case 27:
                    if (caracterAtual == 'e')
                    {
                        Estado = 14;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito() || caracterAtual.IsLetra() || caracterAtual.IsFimDaLinha())
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
                case 28:
                    if (caracterAtual == 'h')
                    {
                        Estado = 29;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito() || caracterAtual.IsLetra() || caracterAtual.IsFimDaLinha())
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
                case 29:
                    if (caracterAtual == 'i')
                    {
                        Estado = 30;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito() || caracterAtual.IsLetra() || caracterAtual.IsFimDaLinha())
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
                case 30:
                    if (caracterAtual == 'l')
                    {
                        Estado = 27;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito() || caracterAtual.IsLetra() || caracterAtual.IsFimDaLinha())
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
                case 31:
                    if (caracterAtual == 'o')
                    {
                        Estado = 14;
                        lexema.Append(caracterAtual);
                    }
                    else if (caracterAtual.IsDigito() || caracterAtual.IsLetra() || caracterAtual.IsFimDaLinha())
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
                case 33:
                    Estado = 0;
                    return Token(TipoToken.OPERADOR_ARITMETICO, lexema);
                case 34:
                    if (caracterAtual == '=')
                    {
                        Estado = 10;
                        lexema.Append(caracterAtual);
                    }
                    else
                    {
                        Estado = 0;
                        return Token(TipoToken.OPERADOR_ATRIBUICAO, lexema);
                    }
                    break;
                case 35:
                    if (caracterAtual is '"')
                    {
                        Estado = 36;
                        lexema.Append(caracterAtual);
                    }
                    else if (!caracterAtual.IsLetra() && caracterAtual is not '_' && !caracterAtual.IsDigito())
                    {
                        LancarExcecaoLexica(lexema);
                    }
                    else
                    {
                        lexema.Append(caracterAtual);
                    }
                    break;
                case 36:
                    if (caracterAtual is not '"')
                    {
                        return Token(TipoToken.STRING, lexema);
                    }
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
        var coluna = Coluna - lexema.Length;
        return new Token(tipoToken, lexema.ToString(), Linha, coluna);
    }

    private void Voltar()
    {
        if (!FimDeArquivo())
        {
            Coluna -= 1;
            Posicao -= 1;
        }
    }

    private void IncrementarLinha()
    {
        Linha++;
        Coluna = 0;
    }

    private void LancarExcecaoLexica(StringBuilder lexema)
    {
        var coluna = Coluna > 1 ? Coluna - 1 : Coluna;
        throw new LexicalException($"Caracter inválido encontrado: {lexema} | Linha: {Linha} | Coluna: {coluna - 1}");
    }

    private char ProximoCaracter()
    {
        if (FimDeArquivo())
            return '\0';

        var caracter = ConteudoArquivo[Posicao];
        Posicao++;
        if (caracter is '\t')
        {
            Coluna += 5;
        }
        else if (!caracter.IsFimDaLinha())
        {
            Coluna++;
        }
        return caracter;
    }

    private bool FimDeArquivo()
    {
        return Posicao == ConteudoArquivo.Length;
    }
}