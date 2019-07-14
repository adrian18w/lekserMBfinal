using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LekserMB
{
    class Program
    {
        //utworzenie listy tokenów
        static List<Token> tokenList = new List<Token>();

        //stworzenie tzw. grup nazwanych dla poszczególnych regexów
        private static string pattern =
            @"(?<operator>\+|\-|\/|\*)|" +
            @"(?<variable>[a-zA-Z_$][a-zA-Z0-9_$]*)|" +
            @"(?<float>([0-9]+\.[0-9]+))|" +
            @"(?<digit>([0-9]+))|" +
            @"(?<bracket>\(|\))|" +
            @"(?<invalid>[^\s])";       


        static void Main(string[] args)
        {
            string expression = "(12-3)-2";
            Console.WriteLine("Wyrażenie: " + expression);           
            if(Lexer(expression))
            {
                Parser();
            }
            else
            {
                ShowError();
            }
        }

        public static void ShowError()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine("Error");
            Console.ResetColor();
        }

        public static void ShowOk()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Green;
            Console.WriteLine("Ok :) ");
            Console.ResetColor();
        }

        public static bool Lexer(string expression)
        {
            //utworzenie obiektu regexPattern klasy Regex, przekazujemy pattern
            Regex regexPattern = new Regex(pattern);

            //wyrażenie do sprawdzenia z patternem
            MatchCollection input = regexPattern.Matches(expression);

            //przelecenie po całym wyrażeniu z input i odpowiednie dopasowanie
            //poszczególnych częsci wyrażenia do grup z patternu
            foreach (Match m in input)
            {
                int i = 0;
                foreach (Group g in m.Groups)
                {
                    string matchValue = g.Value;

                    //testowanie wyniku dopasowania z własnością Success, jeśli jest true i i>2
                    if (g.Success && i > 2)
                    {
                        //wykorzystanie własności GroupNameFromNumber dla utworzonego wcześniej obiektu
                        //pobranie nazwy grupy odpowiadającej numerowi zmiennej i
                        string groupName = regexPattern.GroupNameFromNumber(i);

                        //utworzenie nowego tokenu i dodanie go do listy                       
                        tokenList.Add(new Token(groupName, matchValue));
                    }
                    i++;
                }
            }
            

            foreach (var item in tokenList)
            {
                if (item.Name != "invalid" && !IsPairBracket() &&
                    !IsBracketNextTo() && !IsOperStartEnd() &&
                    !IsOperNextToBracketInside() && !IsOperNextToOper() &&
                    !IsEqual() && !IsOperBefAftBracket())
                {                    
                    //return true;                    
                }
                else
                {                    
                    return false;
                }
            }
            return true;
        }

        public static bool IsOperBefAftBracket()
        {
            for (int i = 0; i < tokenList.Count-1; i++)
            {
                if (tokenList[i].Name != "operator" && tokenList[i].Value != "(" && tokenList[i+1].Value == "(" ||
                    tokenList[i+1].Name != "operator" && tokenList[i+1].Value != ")" && tokenList[i].Value == ")")
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsEqual()
        {
            for (int i = 0; i < tokenList.Count-1; i++)
            {                
                if (tokenList[i].Name != "bracket" && tokenList[i].Name == tokenList[i+1].Name)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsOperNextToOper()
        {
            
            for (int i = 0; i < tokenList.Count; i++)
            {
                if(i<tokenList.Count-1)
                {
                    if (tokenList[i].Name == "operator" && tokenList[i+1].Name == "operator")
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static bool IsOperNextToBracketInside()
        {
            for (int i = 0; i < tokenList.Count; i++)
            {           
                if (tokenList[i].Value == "(" && tokenList[i + 1].Name == "operator" ||
                    tokenList[i].Name == "operator" && tokenList[i + 1].Value == ")")
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsOperStartEnd()
        {

            if (tokenList[0].Name == "operator" || tokenList[tokenList.Count-1].Name == "operator")
            {
                return true;
            }
            return false;
        }
        public static bool IsPairBracket()
        {
            int bracket = 0;
            foreach (var a in tokenList)
            {
                if (a.Value == "(")
                {
                    bracket++;
                }
                if (a.Value == ")")
                {
                    bracket--;
                }
            }

            if (bracket != 0)
            {
                return true;
            }

            return false;
        }
        public static bool IsBracketNextTo()
        {
            for (int i = 0; i < tokenList.Count; i++)
            {
                if (tokenList[i].ToString() == "(" && tokenList[i + 1].ToString() == ")" ||
                    tokenList[i].ToString() == ")" && tokenList[i + 1].ToString() == "(")
                {
                    return true;
                }                
            }
            return false;
        }

        public static bool Parser()
        {
            ShowOk();
            return true;
        }

    }
}
