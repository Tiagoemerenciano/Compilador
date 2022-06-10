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
        private int Escopo = -1;

        public Parser(AnalisadorLexico analisadorLexico)
        {
            AnalisadorLexico = analisadorLexico;

            Programa();
        }

        /// <summary>
        /// int main() <bloco>
        /// </summary>
        private void Programa()
        {
            GetNextToken();
            if (Token.Lexema != INT)
                LancarExcecaoSintatica($"Token esperado: int. Token encontrado: {Token.Lexema}");

            GetNextToken();
            if (Token.Lexema != MAIN)
                LancarExcecaoSintatica($"Token esperado: main. Token encontrado: {Token.Lexema}");

            GetNextToken();
            if (Token.Lexema != ABRE_PARENTESE)
                LancarExcecaoSintatica($"Token esperado: (. Token encontrado: {Token.Lexema}");

            GetNextToken();
            if (Token.Lexema != FECHA_PARENTESE)
                LancarExcecaoSintatica($"Token esperado: ). Token encontrado: {Token.Lexema}");

            GetNextToken();
            Bloco();
        }

        /// <summary>
        /// Realiza a validação do BLOCO
        /// {<declaracao_variavel> <comando>}
        /// </summary>
        private void Bloco()
        {
            Escopo++;

            if (Token.Lexema != ABRE_CHAVE)
            {
                LancarExcecaoSintatica($"Token esperado: {ABRE_CHAVE}. Token encontrado: {Token.Lexema}");
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
                LancarExcecaoSintatica($"Token esperado: {FECHA_CHAVE}. Token encontrado: {Token.Lexema}");

            RemoverSintaxesDoEscopo();

            Escopo--;
        }

        private void RemoverSintaxesDoEscopo()
        {
            var sintaxesParaRemover = Sintaxes.Where(sintaxe => sintaxe.Escopo == Escopo);
            for (var i = 0; i < sintaxesParaRemover.Count(); i++)
            {
                Sintaxes.Remove(sintaxesParaRemover.ElementAt(i));
            }
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
                    LancarExcecaoSintatica($"Token esperado: {ABRE_PARENTESE}. Token encontrado: {Token.Lexema}");

                GetNextToken();
                ExpressaoRelacional();

                if (Token.Lexema != FECHA_PARENTESE)
                    LancarExcecaoSintatica($"Token esperado: {FECHA_PARENTESE}. Token encontrado: {Token.Lexema}");

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

        /// <summary>
        /// <atribuicao> | <bloco> | if (<expressao_relacional>) <comando> else <comando>
        /// </summary>
        /// <returns>bool</returns>
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
            var sintaxe1 = ExpressaoAritmetica();

            if (Token.Tipo != TipoToken.OPERADOR_RELACIONAL)
                LancarExcecaoSintatica($"Token do tipo Operador Relacional esperado. Token encontrado: {Token.Lexema}");

            GetNextToken();
            var sintaxe2 = ExpressaoAritmetica();

            ValidarOperacaoExpressaoAritmetica(sintaxe1.Tipo, sintaxe2.Tipo);
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

            var tokenAnterior = Token;

            GetNextToken();
            if (Token.Tipo != TipoToken.OPERADOR_ATRIBUICAO)
                return false;

            var identificadorDeclarado = BuscarIdentificadorNoEscopo(tokenAnterior.Lexema);

            if (identificadorDeclarado == null)
                LancarExcecaoSemantica($"Variável não declarada: {tokenAnterior.Lexema}");

            GetNextToken();

            var sintaxeExpressaoAritmetica = ExpressaoAritmetica(tokenAnterior.Lexema);

            ValidarTiposIguais(identificadorDeclarado.Tipo, sintaxeExpressaoAritmetica.Tipo);

            if (Token.Lexema != PONTO_E_VIRGULA)
                return false;

            return true;
        }

        private void ValidarTiposIguais(TipoToken tipo1, TipoToken tipo2)
        {
            if (tipo1 != tipo2)
                LancarExcecaoSemantica($"Tipos incompatíveis: {tipo1} e {tipo2}");
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

                ValidarOperacaoTermo(sintaxe1.Token.Tipo, sintaxe2.Token.Tipo);

                if (sintaxe1.Token.Tipo == TipoToken.FLOAT || sintaxe2.Token.Tipo == TipoToken.FLOAT)
                {
                    if (sintaxe1.Token.Tipo == TipoToken.INTEIRO)
                    {
                        sintaxe1.Token.Tipo = TipoToken.FLOAT;
                        sintaxe1.Tipo = TipoToken.FLOAT;
                    }
                    if (sintaxe2.Token.Tipo == TipoToken.INTEIRO)
                        sintaxe2.Token.Tipo = TipoToken.FLOAT;
                }

                if (operador == SINAL_DIVISAO)
                {
                    sintaxe1.Token.Tipo = TipoToken.FLOAT;
                    sintaxe2.Token.Tipo = TipoToken.FLOAT;
                }
                GetNextToken();
            }

            return sintaxe1;
        }

        private void ValidarOperacaoTermo(TipoToken tipo1, TipoToken tipo2)
        {
            if (tipo1 != TipoToken.FLOAT   &&
                tipo2 != TipoToken.INTEIRO &&
                tipo1 != TipoToken.INTEIRO &&
                tipo2 != TipoToken.FLOAT)
                LancarExcecaoSemantica("Tipos incompatíveis");
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
                    LancarExcecaoSintatica($"Token esperado: {FECHA_PARENTESE}. Token encontrado: {Token.Lexema}");
                return sintaxe;
            }
            else
            {
                if (Token.Tipo == TipoToken.IDENTIFICADOR)
                {
                    var identificadorDeclarado = BuscarIdentificadorNoEscopo(Token.Lexema);

                    if (identificadorDeclarado != null)
                    {
                        return new Sintaxe(Token, identificadorDeclarado.Tipo, Escopo);
                    }
                    else
                    {
                        LancarExcecaoSemantica($"Variável não declarada: {Token.Lexema}");
                    }
                }
                else if (Token.Tipo == TipoToken.CHAR || Token.Tipo == TipoToken.FLOAT || Token.Tipo == TipoToken.INTEIRO || Token.Tipo == TipoToken.STRING)
                {
                    return new Sintaxe(Token, Token.Tipo, Escopo);
                }
                else
                {
                    LancarExcecaoSintatica($"Token esperado: <identificador> | <float> | <int> | <char> | <string>. Token encontrado: {Token.Lexema}");
                }
            }

            return new Sintaxe(Token, Token.Tipo, Escopo);
        }

        private Sintaxe? BuscarIdentificadorNoEscopo(string lexema)
        {
            return Sintaxes.FirstOrDefault(sintaxe => sintaxe.Token.Lexema == lexema && sintaxe.Escopo <= Escopo);
        }

        /// <summary>
        /// Realiza a validação de DECLARAÇÃO DE VARIÁVEL
        /// <tipo> <identificador>;
        /// </summary>
        private void DeclaracaoVariavel()
        {
            var tipo = Tipo();

            GetNextToken();
            Identificador();

            if (Sintaxes.Any(sintaxe => sintaxe.Token.Lexema == Token.Lexema && sintaxe.Escopo == Escopo))
                LancarExcecaoSemantica($"Variável já declarada no mesmo escopo: {Token.Lexema}");

            Sintaxes.Add(new Sintaxe(Token, tipo, Escopo));

            GetNextToken();
            if (Token.Lexema != PONTO_E_VIRGULA)
                LancarExcecaoSintatica($"Token esperado: ;. Valor encontrado: {Token.Lexema}");
        }

        private bool IsDeclaracaoVariavel()
        {
            return Token.Lexema == TipoToken.CHAR.GetDescription() || Token.Lexema == TipoToken.FLOAT.GetDescription() || Token.Lexema == TipoToken.INTEIRO.GetDescription() || Token.Lexema == TipoToken.STRING.GetDescription();
        }


        /// <summary>
        /// Realiza a validação do TIPO
        /// int | float | char | string
        /// </summary>
        private TipoToken Tipo()
        {
            if (Token.Lexema != INT && Token.Lexema != FLOAT && Token.Lexema != CHAR && Token.Lexema != STRING)
            {
                LancarExcecaoSintatica($"Token esperado: <float> | <int> | <char> | <string>. Token encontrado: {Token.Lexema}");
            }

            return Token.Lexema.GetValueByDescription<TipoToken>();
        }

        /// <summary>
        /// =
        /// </summary>
        private void Identificador()
        {
            if (Token.Tipo != TipoToken.IDENTIFICADOR)
                LancarExcecaoSintatica($"Token esperado: =. Token encontrado: {Token.Lexema}");
        }

        private void LancarExcecaoSintatica(string? mensagem = null)
        {
            var coluna = Token.Coluna > 1 ? Token.Coluna - 1 : Token.Coluna;

            var descricaoToken = Token.Tipo.GetDescription() ?? "Sintaxe";

            mensagem ??= $"{descricaoToken} inválida encontrado";

            throw new ParserException($"{mensagem} | Linha: {Token.Linha} | Coluna: {coluna - 1}");
        }

        private void LancarExcecaoSemantica(string? mensagem = null)
        {
            var coluna = Token.Coluna > 1 ? Token.Coluna - 1 : Token.Coluna;

            var descricaoToken = Token.Tipo.GetDescription() ?? "Semântica";

            mensagem ??= $"{descricaoToken} inválida encontrado";

            throw new SemanticaException($"{mensagem} | Linha: {Token.Linha} | Coluna: {coluna - 1}");
        }

        private void GetNextToken()
        {
            Token = AnalisadorLexico.GetNextToken();
        }
        private void ValidarOperacaoExpressaoAritmetica(TipoToken tipo1, TipoToken tipo2)
        {
            if ((tipo1 == TipoToken.CHAR && tipo2 != TipoToken.CHAR) || (tipo2 == TipoToken.CHAR && tipo1 != TipoToken.CHAR))
                LancarExcecaoSintatica("CHAR não opera com outros tipos de dados");
        }
    }
}