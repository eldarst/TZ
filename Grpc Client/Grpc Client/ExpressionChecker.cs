using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Grpc_Client
{
    public static class ExpressionChecker
    {
        public static bool ExpressionIsCorrect(string expression)
        {
            int answer = 1;
            answer *= CheckBrackets(expression) ? 1 : 0;
            answer *=  CheckSymbols(expression) ? 1 : 0;

            return answer == 1;
        }
        private static bool CheckBrackets(string expression)
        {
            var brackets = new Stack<char>();
            foreach (var c in expression)
            {
                switch (c)
                {
                    case '(':
                        brackets.Push('(');
                        break;
                    case ')' when brackets.Count == 0:
                        return false;
                    case ')':
                        brackets.Pop();
                        break;
                }
            }

            return brackets.Count == 0;
        }

        private static bool CheckSymbols(string expression)
        {
            var symbols = "*/.+-() 1234567890";

            var others = expression.Where(c => !symbols.Contains(c)).ToList();

            return others.Count == 0;
        }
    }
}
