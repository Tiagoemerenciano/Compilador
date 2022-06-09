using UNICAP.Compilador.Lexical;
using UNICAP.Compilador.Utils;

namespace UNICAP.Compilador.Parser
{
    public class Parser
    {
        private const string INT = "int";
        private const string MAIN = "main";
        private const string ABRE_PARENTESE = "(";
        private const string FECHA_PARENTESE = ")";
        private const string ABRE_CHAVE = "{";
        private const string FECHA_CHAVE = "}";
        private const string PONTO_E_VIRGULA = ";";
        private const string IF = "if";
        private const string ELSE = "else";
        private const string WHILE = "while";
        private const string SINAL_SUBTRACAO = "-";
        private const string SINAL_ADICAO = "+";
        private const string SINAL_MULTIPLICACAO = "*";
        private const string SINAL_DIVISAO = "/";
        private const string FLOAT = "float";
        private const string CHAR = "char";
        private const string STRING = "string";

        private AnalisadorLexico AnalisadorLexico { get; set; }
        private Token Token { get; set; }
        private IList<Sintaxe> Sintaxes { get; set; } = new List<Sintaxe>();
        private int Escopo = 0;

        public Parser(AnalisadorLexico analisadorLexico)
        {
            AnalisadorLexico = analisadorLexico;

            Programa();
        }

        private void Programa()
        {
            GetNextToken();
            if (Token.Lexema != INT)
                LancarExcecaoSintatica();

            GetNextToken();
            if (Token.Lexema != MAIN)
                LancarExcecaoSintatica();

            GetNextToken();
            if (Token.Lexema != ABRE_PARENTESE)
                LancarExcecaoSintatica();

            GetNextToken();
            if (Token.Lexema != FECHA_PARENTESE)
                LancarExcecaoSintatica();

            GetNextToken();
            Bloco();
        }

        /// <summary>
        /// Realiza a validação do BLOCO
        /// {<declaracao_variavel> <comando>}
        /// </summary>
        private void Bloco()
        {
            if (Token.Lexema != ABRE_CHAVE)
            {
                LancarExcecaoSintatica();
            }

            GetNextToken();
            while (IsDeclaracaoVariavel())
            {
                DeclaracaoVariavel();
                GetNextToken();
            }

            while (IsComando())
            {
                Comando();
            }

            if (Token.Lexema != FECHA_CHAVE)
                LancarExcecaoSintatica();
        }

        private bool IsComando()
        {
            return Token.Lexema == IF || Token.Lexema == WHILE || Token.Tipo == TipoToken.IDENTIFICADOR || Token.Lexema == ABRE_CHAVE;
        }

        /// <summary>
        /// Realiza a validação do COMANDO
        /// <comando_basico> | <iteracao> | if (<expressao_relacional>) <comando> else <comando>
        /// </summary>
        private void Comando()
        {
            if (IsComandoBasico())
            {
                ComandoBasico();
            }
            else if (IsIteracao())
            {
                Iteracao();
            }            
            else if (Token.Lexema == IF)
            {
                GetNextToken();
                if (Token.Lexema != ABRE_PARENTESE)
                    LancarExcecaoSintatica();

                GetNextToken();
                ExpressaoRelacional();

                if (Token.Lexema != FECHA_PARENTESE)
                    LancarExcecaoSintatica();

                GetNextToken();
                Comando();

                if (Token.Lexema == ELSE)
                    Comando();
            }
            GetNextToken();
        }

        private bool IsIteracao()
        {
            return Token.Lexema == WHILE;
        }
        
        private bool IsComandoBasico()
        {
            return Token.Tipo == TipoToken.IDENTIFICADOR || Token.Lexema == ABRE_CHAVE;
        }

        /// <summary>
        /// Realiza a validação da EXPRESSÃO RELACIONAL
        /// <expressao_aritmetica> <operador_relacional> <expressao_aritmetica>
        /// </summary>
        private void ExpressaoRelacional()
        {
            ExpressaoAritmetica();

            if (Token.Tipo != TipoToken.OPERADOR_RELACIONAL)
                LancarExcecaoSintatica();

            GetNextToken();
            ExpressaoAritmetica();
        }

        private bool Iteracao()
        {
            if (Token.Lexema != WHILE)
                return false;

            GetNextToken();
            if (Token.Lexema != ABRE_PARENTESE)
                return false;

            GetNextToken();
            ExpressaoRelacional();

            if (Token.Lexema != FECHA_PARENTESE)
                return false;

            Comando();

            return true;
        }

        /// <summary>
        /// <atribuicao> | <bloco> | if (<expressao_relacional>) <comando> else <comando>
        /// </summary>
        /// <returns></returns>
        private bool ComandoBasico()
        {
            if (Atribuicao())
                return true;
            else if (Token.Lexema == IF)
            {
                GetNextToken();
                if (Token.Lexema != ABRE_PARENTESE)
                    return false;

                GetNextToken();
                ExpressaoRelacional();

                GetNextToken();
                if (Token.Lexema != FECHA_PARENTESE)
                    return false;

                Comando();

                if (Token.Lexema != ELSE)
                    return false;

                Comando();
            }
            else
            {
                Bloco();
            }

            return true;
        }

        /// <summary>
        /// Realiza a validação da ATRIBUIÇÃO
        /// <identificador> <operador_atribuicao> <expressao_aritmetica>;
        /// </summary>
        private bool Atribuicao()
        {
            if (Token.Tipo != TipoToken.IDENTIFICADOR)
                return false;

            var identificador = Token.Lexema;

            GetNextToken();
            if (Token.Tipo != TipoToken.OPERADOR_ATRIBUICAO)
                return false;

            GetNextToken();
            Sintaxes.Add(ExpressaoAritmetica(identificador));

            if (Token.Lexema != PONTO_E_VIRGULA)
                return false;

            return true;
        }

        /// <summary>
        /// Realiza a validação da EXPRESSÃO ARITMÉTICA
        /// <expressao_aritmetica> + <termo> | <expressao_aritmetica> - <termo> | <termo> 
        /// </summary>
        private Sintaxe ExpressaoAritmetica(string? identificador = null)
        {
            var sintaxe1 = Termo();
            sintaxe1.Identificador = identificador;

            while (Token.Lexema == SINAL_ADICAO || Token.Lexema == SINAL_SUBTRACAO)
            {
                GetNextToken();
                var sintaxe2 = Termo();
                sintaxe2.Identificador = identificador;

                ValidarOperacaoExpressaoAritmetica(sintaxe1.Token.Tipo, sintaxe2.Token.Tipo);

                if (sintaxe1.Token.Tipo == TipoToken.FLOAT || sintaxe2.Token.Tipo == TipoToken.FLOAT)
                {
                    sintaxe1.Token.Tipo = TipoToken.FLOAT;
                    sintaxe2.Token.Tipo = TipoToken.FLOAT;
                }
                else
                {
                    sintaxe1.Token.Tipo = TipoToken.INTEIRO;
                    sintaxe2.Token.Tipo = TipoToken.INTEIRO;
                }
            }

            return sintaxe1;
        }

        /// <summary>
        /// Realiza a validação do TERMO
        /// <termo> * <fator> | <termo> / <fator> | <fator>
        /// </summary>
        private Sintaxe Termo()
        {
            var sintaxe1 = Fator();

            GetNextToken();
            while (Token.Lexema == SINAL_MULTIPLICACAO || Token.Lexema == SINAL_DIVISAO)
            {
                var operador = Token.Lexema;
                GetNextToken();
                var sintaxe2 = Fator();

                if (sintaxe1.Token.Tipo == TipoToken.FLOAT || sintaxe2.Token.Tipo == TipoToken.FLOAT)
                {
                    if (sintaxe1.Token.Tipo == TipoToken.INTEIRO)
                        sintaxe1.Token.Tipo = TipoToken.FLOAT;
                    if (sintaxe2.Token.Tipo == TipoToken.INTEIRO)
                        sintaxe2.Token.Tipo = TipoToken.FLOAT;
                }

                if (operador == SINAL_DIVISAO)
                {
                    sintaxe1.Token.Tipo = TipoToken.FLOAT;
                    sintaxe2.Token.Tipo = TipoToken.FLOAT;
                }
            }

            return sintaxe1;
        }

        /// <summary>
        /// Realiza a validação do FATOR
        /// (<expressao_aritmetica>) | <identificador> | <float> | <int> | <char> | <string>
        /// </summary>
        private Sintaxe Fator()
        {
            if (Token.Lexema == ABRE_PARENTESE)
            {
                GetNextToken();
                var sintaxe = ExpressaoAritmetica();
                GetNextToken();
                if (Token.Lexema != FECHA_PARENTESE)
                    LancarExcecaoSintatica();
                return sintaxe;
            }
            else
            {
                if (Token.Tipo == TipoToken.IDENTIFICADOR)
                {
                    var identificadorDeclarado = BuscarIdentificador(Token.Lexema);

                    if (identificadorDeclarado != null)
                    {
                        return new Sintaxe(Token, identificadorDeclarado.Tipo, Escopo);
                    }
                    else
                    {
                        LancarExcecaoSintatica("Variável não declarada");
                    }
                }
                else if (Token.Tipo == TipoToken.CHAR || Token.Tipo == TipoToken.FLOAT || Token.Tipo == TipoToken.INTEIRO || Token.Tipo == TipoToken.STRING)
                {
                    return new Sintaxe(Token, (int)TipoSintaxe.FATOR, Escopo);
                }
                else
                {
                    LancarExcecaoSintatica($"Token esperado: <identificador> | <float> | <int> | <char> | <string>. Token encontrado: ");
                }
            }

            return new Sintaxe(Token, (int)TipoSintaxe.FATOR, Escopo);
        }

        private Sintaxe? BuscarIdentificador(string lexema)
        {
            return Sintaxes.FirstOrDefault(sintaxe => sintaxe.Token.Lexema == lexema);
        }

        /// <summary>
        /// Realiza a validação de DECLARAÇÃO DE VARIÁVEL
        /// <tipo> <identificador>;
        /// </summary>
        private void DeclaracaoVariavel()
        {
            Tipo();

            GetNextToken();
            Identificador();

            Sintaxes.Add(new Sintaxe(Token, (int)TipoSintaxe.DECLARACAO_VARIAVEL, Escopo));

            GetNextToken();
            if (Token.Lexema != PONTO_E_VIRGULA)
            {
                LancarExcecaoSintatica("Declaração de variável inválida");
            }
        }

        private bool IsDeclaracaoVariavel()
        {
            return Token.Lexema == TipoToken.CHAR.GetDescription() || Token.Lexema == TipoToken.FLOAT.GetDescription() || Token.Lexema == TipoToken.INTEIRO.GetDescription() || Token.Lexema == TipoToken.STRING.GetDescription();
        }


        /// <summary>
        /// Realiza a validação do TIPO
        /// int | float | char | string
        /// </summary>
        private bool Tipo(bool validacao = false)
        {
            if (Token.Lexema != INT && Token.Lexema != FLOAT && Token.Lexema != CHAR && Token.Lexema != STRING)
            {
                if (validacao)
                    return false;
                LancarExcecaoSintatica();
            }

            return true;
        }

        /// <summary>
        /// Realiza a validação do IDENTIFICADOR
        /// =
        /// </summary>
        private bool Identificador(bool validacao = false)
        {
            if (Token.Tipo != TipoToken.IDENTIFICADOR)
            {
                if (validacao)
                    return false;

                LancarExcecaoSintatica();
            }

            return true;
        }

        private void LancarExcecaoSintatica(string? mensagem = null)
        {
            var coluna = Token.Coluna > 1 ? Token.Coluna - 1 : Token.Coluna;

            var descricaoToken = Token.Tipo.GetDescription() ?? "Sintaxe";

            mensagem ??= $"{descricaoToken} inválida encontrado";

            throw new ParserException($"{mensagem}: {Token.Lexema} | Linha: {Token.Linha} | Coluna: {coluna - 1}");
        }

        private void GetNextToken()
        {
            Token = AnalisadorLexico.GetNextToken();
        }
        private TipoToken ValidarOperacaoExpressaoAritmetica(TipoToken tipo1, TipoToken tipo2)
        {
            if (tipo1 == TipoToken.INTEIRO && tipo2 == TipoToken.INTEIRO)
                return TipoToken.INTEIRO;
            else if ((tipo1 == TipoToken.INTEIRO && tipo2 == TipoToken.FLOAT) || (tipo1 == TipoToken.FLOAT && tipo2 == TipoToken.INTEIRO))
                return TipoToken.FLOAT;
            else if ((tipo1 == TipoToken.CHAR && tipo2 != TipoToken.CHAR) || (tipo2 == TipoToken.CHAR && tipo1 != TipoToken.CHAR))
                LancarExcecaoSintatica("CHAR não opera com outros tipos de dados");

            return TipoToken.CHAR;
        }
    }
}