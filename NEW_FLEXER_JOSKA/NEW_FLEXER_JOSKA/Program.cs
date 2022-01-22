using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEW_FLEXER_JOSKA
{
    class Program
    {
        static Dictionary<string, string> bigTokens = new Dictionary<string, string>()
        {
            {"PROGRAM", "start program token" },
            {"PROCEDURE", "Procedure token"},
            {"BEGIN", "Begin token"},
            {"VAR", "declaring block variables token" },
            {"STRING", "variables type token" },
            {"INTEGER", "variables type token" },
            {"BOOLEAN", "variables type token" },
            {"TEXT", "variables type token" },
            {"CHAR", "variables type token" },
            {"END", "end block token" },
            {"IF", "condition token" },
            {"THEN", "should token" },
            {"ELSE", "otherwise token" },
            {"TRUE", "true ans token" },
            {"FALSE", "false ans token" },
            {"RESET", "reset file token" },
            {"ASSIGN", "assign file token" },
            {"REWRITE", "rewrite file token" },
            {"CLOSE",  "close file token" },
            {"WHILE", "while token" },
            {"FOR", "for token" },
            {"DO", "do token" },
            {"INC", "increment token" },
            {"READ", "read token" },
            {"READLN", "readln token" },
            {"WRITE", "write token" },
            {"WRITELN", "writeln token" },

        };

        static Dictionary<string, string> oneSymbolTokens = new Dictionary<string, string>()
        {
            {";", "end action token" },
            {":", "declaring variable token" },
            {"=", "equality token" },
            {"(", "open bracket token" },
            {")", "close bracket token" },
            {"+", "plus token" },
            {"-", "minus token" },
            {"*", "multiply token" },
            {">", "more token" },
            {"<", "less token" },
            {"/", "divide token" },
            {",", "split token" },
            {".", "end program token" }
        };

        static Dictionary<string, string> twoSymbolTokens = new Dictionary<string, string>()
        {

            {"<>", "not equal token" },
            {":=", "equate token" },
            {">=", "more or equal token" },
            {"=<", "less or equal token" }
        };

        static string tw = "<>:=";

        struct Token
        {
            public string name;
            public string type;
            public int pos;
            public int column;
        }
        static void Main(string[] args)
        {
            List<Token> tokens = new List<Token>();
            string pathToFile = Environment.CurrentDirectory + "\\input1.txt";
            StreamReader sr = new StreamReader(pathToFile);
            string input = sr.ReadToEnd();
            input = input.Replace("\r", "").Replace("\0", "");
            string[] paramss = input.Split('\n');
            int col = 0;
            bool isCom = false;
            foreach (string st in paramss)
            {
                SearchTokens(ref tokens, st, col, ref isCom);
                col++;
            }
            WriteOut(tokens);
        }

        static void SearchTokens(ref List<Token> tokens, string param, int col, ref bool isCom)
        {
            string bufferCh = "";
            int count = 0;
            bool isComLocal = false;
            bool ex = false;
            char dop = '~';
            string buffLit = "";
            for (int i = 0; i < param.Length; i++)
            {
                GetDel(param[i], ref isCom, isComLocal);
                if (i + 1 < param.Length)
                {
                    GetLocalDel(param[i], param[i + 1], ref isComLocal, ref count);
                    if (!isCom && !isComLocal)
                    {
                        isExit(param[i], param[i + 1], ref ex);
                    }
                }
                
                if (ex)
                {
                    return;
                }
                if (isComLocal)
                {
                    buffLit += param[i];
                }
                else
                {
                    if (buffLit.Length > 1)
                    {
                        Token token = new Token();
                        token.name = buffLit.Remove(0, 1);
                        token.pos = i - buffLit.Length + 1;
                        token.column = col;
                        token.type = "literal token";
                        tokens.Add(token);
                    }
                    buffLit = "";
                }
                if (i == param.Length - 1)
                {
                    dop = ' ';
                }
                else
                {
                    dop = param[i + 1];
                }
                ChecSym(param[i], dop, ref bufferCh, ref i, ref tokens, col, isCom, isComLocal);
            }
            AfterFor(tokens, bufferCh, col);
        }
        static void AfterFor(List<Token> tokens, string bufferCh, int col)
        {
            if (bufferCh.Length > 0)
            {
                tokens.Add(GetToken(bufferCh, col, bufferCh.Length));
            }
        }
        static void ChecSym(char ch1, char ch2, ref string bufferCh, ref int i, ref List<Token> tokens, int col, bool isCom, bool isComLocal)
        {
            if ((char.IsDigit(ch1) || char.IsLetter(ch1)) && !isCom && !isComLocal)
            {
                bufferCh += ch1.ToString();
            }
            else if ((ch1 != ('\'') && ch1 != ('{') && ch1 != ('/')) && !isCom && !isComLocal)
            {
                if (bufferCh.Length > 0)
                {
                    tokens.Add(GetToken(bufferCh, col, i));
                }
                if (!tw.Contains(ch1) && ch1 != ' ')
                {
                    tokens.Add(GetToken(ch1.ToString(), col, i + 1));
                }
                else if (ch1 != ' ')
                {
                    if (tw.Contains(ch2))
                    {
                        tokens.Add(GetToken((ch1.ToString() + ch2.ToString()).ToString(), col, i + 2));
                        i++;
                    }
                    else
                    {
                        tokens.Add(GetToken(ch1.ToString(), col, i + 1));
                    }
                }
                bufferCh = "";

            }
        }
        static void isExit(char ch1, char ch2, ref bool isExit)
        {
            if ((ch1 == '/') && (ch2 == '/'))
            {
                isExit = true;
            }
        }
        static void GetDel(char ch, ref bool isCom, bool isComLocal)
        {
            if ((ch == '{') && (!isComLocal))
            {
                isCom = true;
            }
            if ((ch == '}') && (!isComLocal))
            {
                isCom = false;
            }

        }
        static void GetLocalDel(char ch1, char ch2, ref bool isComLocal, ref int count)
        {
            if (ch1 == '\'')
            {
                count++;
                isComLocal = true;
            }
            if ((count % 2 == 0) && (ch1 == '\''))
            {
                if (ch2 != '\'')
                {
                    isComLocal = false;
                }
            }
        }
        static Token GetToken(string name, int col, int pos)
        {
            Token token = new Token();
            token.name = name;
            token.pos = pos - name.Length;
            token.column = col;
            if (bigTokens.ContainsKey(name))
            {
                token.type = bigTokens[name];
                return token;
            }
            if (twoSymbolTokens.ContainsKey(name))
            {
                token.type = twoSymbolTokens[name];
                return token;
            }
            if (oneSymbolTokens.ContainsKey(name))
            {
                token.type = oneSymbolTokens[name];
                return token;
            }
            token.type = "variable token";
            return token;
        }


        static void WriteOut(List<Token> tokens)
        {
            StreamWriter f = new StreamWriter("output.txt");
            foreach (Token token in tokens)
            {
                string output = "token:" + token.name + " |type: " + token.type + " |pos: " + token.pos + " |st: " + token.column;
                f.WriteLine(output);
            }
            f.Close();
        }
    }

}
