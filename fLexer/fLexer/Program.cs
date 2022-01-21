using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fLexer
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
            {",", "split token" }
        };

        static Dictionary<string, string> twoSymbolTokens = new Dictionary<string, string>()
        {

            {"<>", "not equal token" },
            {":=", "equate token" }

        };

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
            input = input.Replace("\r", "").Replace("\0", "").Replace(" ", "~");
            string[] paramss = input.Split('\n');
            int col = 1;
            bool isCom = false;
            string buffer = "";
            foreach (string param in paramss)
            {
                SearchTokens(ref tokens, param, col,ref isCom, ref buffer);
            }
        }
        static void SearchTokens(ref List<Token> tokens, string param, int col,ref bool isCom, ref string buffer)
        {
            List<Token> tokensIter = new List<Token>();
            SearchLiteral(ref tokensIter, ref param, col, ref isCom, ref buffer);
            if(isCom)
            {
                return;
            }
            SearchVariable(ref tokensIter, ref param, col);
            SearchBigTokens(ref tokensIter, ref param, col);
            SearchTwoSymbolToken(ref tokensIter, ref param, col);
            SearchOneSymbolToken(ref tokensIter, ref param, col);
            var sortedTokens = from u in tokensIter
                              orderby u.pos
                              select u;
            foreach(Token tok in sortedTokens)
            {
                Console.WriteLine(tok.name + " " + tok.pos);
                tokens.Add(tok);
            }

        }

        static void SearchOneSymbolToken(ref List<Token> tokens, ref string param, int col)
        {
            string result = "";
            for (int i = 0; i < param.Length; i++)
            {
                if(oneSymbolTokens.ContainsKey(param[i].ToString()))
                {
                    Token token = new Token();
                    token.name = param[i].ToString();
                    token.type = oneSymbolTokens[param[i].ToString()];
                    token.column = col;
                    token.pos = getLength(param, (i - param[i].ToString().Length + 1));
                    result += " 1 ";

                    tokens.Add(token);
                }
                else
                {
                    result += param[i].ToString();
                }
            }
            param = result;
        }

        static void SearchTwoSymbolToken(ref List<Token> tokens, ref string param, int col)
        {
            bool nextDel = false;
            string mbToken = "";
            string result = "";
            for (int i = 0; i < param.Length - 1; i++)
            {
                mbToken += param[i].ToString() + param[i + 1].ToString();
                if (twoSymbolTokens.ContainsKey(mbToken))
                {
                    Token token = new Token();
                    token.name = mbToken;
                    token.type = twoSymbolTokens[mbToken];
                    token.column = col;
                    token.pos = getLength(param, (i - mbToken.Length + 1));
                    result += " 2 ";
                    nextDel = true;
                    tokens.Add(token);
                }
                else if(!nextDel)
                {
                    result += param[i].ToString();
                }
                else
                {
                    nextDel = false;
                }
                mbToken = "";
            }
            if (param.Length > 0)
            {
                result += param[param.Length - 1].ToString();
            }
            param = result;
        }

        static void SearchVariable(ref List<Token> tokens, ref string param, int col)
        {
            param = param.Replace("\0", "");
            string mbToken = "";
            int count = 0;
            int ind = 0;
            string result = "";
            for (int i = 0; i < param.Length; i++)
            {
                if ((char.IsDigit(param[i])) || (char.IsLetter(param[i])))
                {
                    mbToken = mbToken + param[i];
                    result += param[i].ToString();

                }
                else
                {
                    if(!bigTokens.ContainsKey(mbToken) && mbToken.Length > 0 && (count % 2 == 0) )
                    {
                        //string test = param.Substring(result.Length + ind , i- result.Length - mbToken.Length);
                        //result += test.Remove(test.Length- mbToken.Length- 1, mbToken.Length);
                        //result += param[i];
                        //result += test;
                        result = result.Remove(result.Length - mbToken.Length);
                        result += " "+ mbToken.Length + " ";
                        result += param[i];
                        Token token = new Token();
                        token.name = mbToken;
                        token.type = "Variable token";
                        token.column = col;
                        token.pos = getLength(result, (i - mbToken.Length)+1);
                        tokens.Add(token);

                        ind += mbToken.Length - mbToken.Length.ToString().Length;
                    }
                    else
                    {
                        result += param[i].ToString();
                    }
                    mbToken = "";
                }
                if (param[i] == char.Parse("'"))
                {
                    count++;
                }
            }

            param = result;
        }

        static void SearchLiteral(ref List<Token> tokens,ref string param, int col, ref bool isCom, ref string buffer)
        {

            int idQ = -1;
            int count = -1;
            List<int> quot = new List<int>();
            List<int> quotNotText = new List<int>();
            List<int> quotNotTextShift = new List<int>();
            if (!param.Contains("\'") &&!isCom)
            {
                return;
            }
            if(isCom && !param.Contains("}"))
            {
                param = "";
                return;
            }
            for (int i = 0; i < param.Length; i++)
            {
                if (param[i] == '\'')
                {
                    quot.Add(i);
                }
            }
            for (int i = 0; i < param.Length; i++)
            {
                if(param[i] == '\'')
                {
                    count++;
                    if (idQ == -1)
                    {
                        idQ = count;
                        quotNotText.Add(i);
                        quotNotTextShift.Add(i);
                    }
                    if((param[i - 1] !='\'' || (idQ == count - 1) || (((count+1)%2) == 0 && param[i + 1] != '\'')) && param[i + 1] != '\'' && idQ != count)
                    {
                        quot[count] = 0;
                        quot[idQ] = 0;
                        quotNotText.Add(i);
                        quotNotTextShift.Add(i);
                        idQ = -1;
                    }
                }
            }
            if (param.Contains("//"))
            {
                string copy = "";
                int index = 0;
                List<int> posCom = new List<int>();
                while ((index = param.IndexOf("//", index)) != -1)
                {
                    posCom.Add(index);
                    index += 2;
                }
                List<int> posComCheck = new List<int>();
                for(int i = 0; i < quotNotText.Count; i+=2)
                {
                    for (int j = 0; j < posCom.Count; j ++)
                    {
                        if((posCom[j] < quotNotText[i+1] && posCom[j] > quotNotText[i]) && (!posComCheck.Contains(posCom[j])))
                        {
                            posComCheck.Add(posCom[j]);
                        }
                        else
                        {
                            if(posCom[j] < quotNotText[i] && (!posComCheck.Contains(posCom[j])))
                            {
                                copy = GetCopy(param, posCom[j]);
                                param = copy;
                                SearchLiteral(ref tokens, ref param, col,ref isCom, ref buffer);
                                return;
                            }
                        }
                    }
                }
            }
            if (param.Contains("{") || param.Contains("}"))
            {
                if (isCom)
                {
                    param = "{" + param;
                }
                string copy = "";
                int index = 0;
                List<int> posComAll = new List<int>();

                List<int> posComEnd = new List<int>();
                while ((index = param.IndexOf("{", index)) != -1)
                {
                    posComAll.Add(index);
                    index += 1;
                }
                index = 0;
                while ((index = param.IndexOf("}", index)) != -1)
                {
                    posComEnd.Add(index);
                    index += 1;
                }
                if (!param.Contains("}"))
                {

                    copy = GetCopyWithoutPart(param, posComAll[0], param.Length);
                    buffer = param.Remove(posComAll[0], param.Length);
                    param = copy;
                    
                    SearchLiteral(ref tokens, ref param, col, ref isCom, ref buffer);
                    isCom = true;
                    return;
                }
                else
                {
                    if(param.Contains("{"))
                    {
                        string res = "";
                        int iterSt = 0;
                        int iterEnd = 0;
                        bool isDel = false;
                        for(int i = 0; i < param.Length; i++)
                        {
                            if(i > posComEnd[posComEnd.Count-1])
                            {
                                res += param.Substring(i);
                                break;
                            }
                            if(i == posComAll[iterSt])
                            {
                                isDel = true;
                            }
                            if(i == posComEnd[iterEnd])
                            {
                                isDel = false;
                                for(int j = iterSt; j < posComAll.Count; j++)
                                {
                                    if(posComAll[j] > i)
                                    {
                                        iterEnd++;
                                        break;
                                    }
                                    iterSt++;
                                }
                            }
                            if(!isDel)
                            {
                                res += param[i];
                            }
                            else
                            {
                                res += " ";
                            }
                        }
                        param = res.Replace("}", " ");
                    }
                    isCom = false;
                    SearchLiteral(ref tokens, ref param, col, ref isCom, ref buffer);
                    return;

                }
            }
            int shift = 2;
            int countDel = 1;
            for (int i = 0; i < quotNotText.Count; i += 2)
            {
                string name = param.Substring(quotNotTextShift[i] + 1, (quotNotTextShift[i + 1] - (quotNotTextShift[i]) - 1));

                for (int j = quotNotTextShift[i] + 1; j < quotNotTextShift[i + 1]; j++)
                {
                    param = param.Remove(quotNotTextShift[i] + 1, 1);
                    countDel++;
                }

                countDel--;
                if (name.Length > 0)
                {
                    param = param.Insert(quotNotTextShift[i] + 1, name.Length.ToString());
                    for (int j = shift; j < quotNotTextShift.Count; j++)
                    {
                        quotNotTextShift[j] = quotNotTextShift[j] - countDel + name.Length.ToString().Length;
                    }
                    shift += 2;
                    countDel = 1;
                }
            }
        }

        static void SearchBigTokens(ref List<Token> tokens,ref string param, int col)
        {
            string mbToken = "";
            string result = "";
            for(int i = 0; i < param.Length; i++)
            {
                mbToken = mbToken + param[i];
                if(bigTokens.ContainsKey(mbToken))
                {
                    if ((i + 1 < param.Length) && param[i + 1] == 'L')
                    {

                    }
                    else
                    {
                        Token token = new Token();
                        token.name = mbToken;
                        token.type = bigTokens[mbToken];
                        token.column = col;
                        token.pos = getLength(param,(i - mbToken.Length + 1));
                        result += " " + mbToken.Length.ToString() + " ";
                        mbToken = "";
                        tokens.Add(token);
                    }
                }
                else if((!char.IsDigit(param[i])) && (!char.IsLetter(param[i])))
                {
                    result = result + mbToken;
                    mbToken = "";

                }
            }
            param = result;
        }
        public static List<int> CloneList(List<int> sourceList)
        {
            List<int> list = new List<int>(sourceList.Count);
            foreach (var item in sourceList)
            {
                list.Add(item);
            }
            return list;
        }

        static string GetCopy(string param, int index)
        {
            string copy = "";
            for (int i = 0; i < index; i++)
            {
                copy += param[i];
            }

            return copy;
        }

        static string GetCopyWithoutPart(string param, int startCom, int endCom)
        {
            string copy = "";
            for (int i = 0; i < param.Length; i++)
            {
                if (!(i > startCom && i < endCom))
                {
                    copy += param[i];
                }
            }
            return copy;
        }

        static int getLength(string param, int len)
        {
            param += " ";
            string num = "";
            int counter = 0;
            for(int i = 1; i < len+1; i++)
            {
                if(char.IsDigit(param[i]))
                {
                    num += param[i];
                }
                else if(param[i] != ' ')
                {
                    counter++;
                    int.TryParse(num, out int number);
                    counter += number;
                    num = "";
                }
            }
            return counter;
        }
    }
}
