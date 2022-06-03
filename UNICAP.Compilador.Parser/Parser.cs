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

        private AnalisadorLexico AnalisadorLexico { get; set; }
        private Token Token { get; set; }
        private IList<Sintaxe> Sintaxes { get; set; } = new List<Sintaxe>();

        public Parser(AnalisadorLexico analisadorLexico)
        {
            AnalisadorLexico = analisadorLexico;

            Programa();
        }

        private void Programa()
        {
            Token = AnalisadorLexico.GetNextToken();
            if (Token.Lexema != INT)
                LancarExcecaoSintatica();

            Token = AnalisadorLexico.GetNextToken();
            if (Token.Lexema != MAIN)
                LancarExcecaoSintatica();

            Token = AnalisadorLexico.GetNextToken();
            if (Token.Lexema != ABRE_PARENTESE)
                LancarExcecaoSintatica();

            Token = AnalisadorLexico.GetNextToken();
            if (Token.Lexema != FECHA_PARENTESE)
                LancarExcecaoSintatica();

            Bloco();
        }

        /// <summary>
        /// Realiza a validação do BLOCO
        /// {<declaracao_variavel> <comando>}
        /// </summary>
        private void Bloco()
        {
            Token = AnalisadorLexico.GetNextToken();
            if (Token.Lexema != ABRE_CHAVE)
                LancarExcecaoSintatica();

            DeclaracaoVariavel();
            Comando();

            Token = AnalisadorLexico.GetNextToken();
            if (Token.Lexema != FECHA_CHAVE)
                LancarExcecaoSintatica();
        }

        /// <summary>
        /// Realiza a validação do COMANDO
        /// <comando_basico> | <iteracao> | if <expressao_relacional> <comando> else <comando>
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
            else
            {
                Token = AnalisadorLexico.GetNextToken();
                if (Token.Lexema != IF)
                    LancarExcecaoSintatica();

                Token = AnalisadorLexico.GetNextToken();
                if (Token.Lexema != ABRE_PARENTESE)
                    LancarExcecaoSintatica();

                ExpressaoRelacional();

                Token = AnalisadorLexico.GetNextToken();
                if (Token.Lexema != FECHA_PARENTESE)
                    LancarExcecaoSintatica();

                Comando();

                Token = AnalisadorLexico.GetNextToken();
                if (Token.Lexema != ELSE)
                    LancarExcecaoSintatica();

                Comando();
            }
            
        }

        /// <summary>
        /// Realiza a validação da EXPRESSÃO RELACIONAL
        ///     <expressao_aritmetica> <operador_relacional> <expressao_aritmetica>
        /// </summary>
        private void ExpressaoRelacional()
        {
            ExpressaoAritmetica();

            Token = AnalisadorLexico.GetNextToken();
            if (Token.Tipo != TipoToken.OPERADOR_RELACIONAL)
                LancarExcecaoSintatica();

            ExpressaoAritmetica();
        }

        private bool IsIteracao()
        {
            throw new NotImplementedException();
        }

        private bool IsComandoBasico()
        {
            throw new NotImplementedException();
        }

        private void Iteracao()
        {
            Token = AnalisadorLexico.GetNextToken();
            if (Token.Lexema != WHILE)
                LancarExcecaoSintatica();

            Token = AnalisadorLexico.GetNextToken();
            if (Token.Lexema != ABRE_PARENTESE)
                LancarExcecaoSintatica();

            ExpressaoRelacional();

            Token = AnalisadorLexico.GetNextToken();
            if (Token.Lexema != FECHA_PARENTESE)
                LancarExcecaoSintatica();

            Comando();
        }

        private void ComandoBasico()
        {
            if (IsAtribuicao())
                Atribuicao();
            else
                Bloco();
        }

        private bool IsAtribuicao()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Realiza a validação da ATRIBUIÇÃO
        /// <identificador> <operador_atribuicao> <expressao_aritmetica>;
        /// </summary>
        private void Atribuicao()
        {
            Token = AnalisadorLexico.GetNextToken();
            if (Token.Tipo != TipoToken.IDENTIFICADOR)
                LancarExcecaoSintatica();

            Token = AnalisadorLexico.GetNextToken();
            if (Token.Tipo != TipoToken.OPERADOR_ATRIBUICAO)
                LancarExcecaoSintatica();

            ExpressaoAritmetica();

            Token = AnalisadorLexico.GetNextToken();
            if (Token.Lexema != PONTO_E_VIRGULA)
                LancarExcecaoSintatica();
        }

        /// <summary>
        /// Realiza a validação da EXPRESSÃO ARITMÉTICA
        /// <expressao_aritmetica> + <termo> | <expressao_aritmetica> - <termo> | <termo> 
        /// </summary>
        private void ExpressaoAritmetica()
        {
            Token = AnalisadorLexico.GetNextToken();
            if (Termo())
                return;

            ExpressaoAritmetica();
            Token = AnalisadorLexico.GetNextToken();
            if (Token.Lexema == SINAL_SUBTRACAO || Token.Lexema == SINAL_ADICAO)
                Termo();
            else
                LancarExcecaoSintatica();
        }

        /// <summary>
        /// Realiza a validação do TERMO
        /// <termo> * <fator> | <termo> / <fator> | <fator>
        /// </summary>
        private bool Termo()
        {
            if (Fator())
                return true;
            else
            {
                Termo();

                Token = AnalisadorLexico.GetNextToken();
                if (Token.Lexema == SINAL_MULTIPLICACAO || Token.Lexema == SINAL_DIVISAO)
                    Fator();
                else
                    LancarExcecaoSintatica();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Realiza a validação do FATOR
        /// (<expressao_aritmetica>) | <identificador> | <tipo>
        /// </summary>
        private bool Fator()
        {
            Token = AnalisadorLexico.GetNextToken();
            if (Tipo())
                return true;
            else if (Identificador())
                return true;
            else
            {
                Token = AnalisadorLexico.GetNextToken();
                if (Token.Lexema == ABRE_PARENTESE)
                {
                    ExpressaoAritmetica();

                    Token = AnalisadorLexico.GetNextToken();
                    if (Token.Lexema != FECHA_PARENTESE)
                        LancarExcecaoSintatica();
                }
                else
                    LancarExcecaoSintatica();
            }

            return true;
        }

        private void DeclaracaoVariavel()
        {
            Tipo();
            Identificador();
            if (Token.Lexema != PONTO_E_VIRGULA)
                LancarExcecaoSintatica();
        }

        /// <summary>
        /// Realiza a validação do TIPO
        /// int | float | char | string
        /// </summary>
        private bool Tipo()
        {
            Token = AnalisadorLexico.GetNextToken();
            if (Token.Tipo != TipoToken.INTEIRO || Token.Tipo != TipoToken.FLOAT || Token.Tipo != TipoToken.CHAR || Token.Tipo != TipoToken.STRING)
                LancarExcecaoSintatica();

            return true;
        }

        /// <summary>
        /// Realiza a validação do IDENTIFICADOR
        /// =
        /// </summary>
        private bool Identificador()
        {
            Token = AnalisadorLexico.GetNextToken();
            if (Token.Tipo != TipoToken.IDENTIFICADOR)
                LancarExcecaoSintatica();

            return true;
        }

        private void LancarExcecaoSintatica()
        {
            var coluna = Token.Coluna > 1 ? Token.Coluna - 1 : Token.Coluna;

            var descricaoToken = Token.Tipo.GetDescription() ?? "Sintaxe";

            throw new LexicalException($"{descricaoToken} inválida encontrado: {Token.Lexema} | Linha: {Token.Linha} | Coluna: {coluna - 1}");
        }

        private void NovaSintaxe(Token token, int tipo, int escopo)
        {
            Sintaxes.Add(new Sintaxe
            {
                Escopo = escopo,
                Lexema = token.Lexema,
                Tipo = tipo,
                Token = token
            });
        }
    }
}