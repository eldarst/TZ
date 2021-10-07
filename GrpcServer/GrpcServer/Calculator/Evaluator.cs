using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GrpcServer.Calculator
{
    public static class Evaluator
    {
        public static double Evaluate(string expression)
        {
            expression = expression.Replace(" ", "");
            return CountSimple(expression);
        }

        private static double CountSimple(string expression)
        {
            var numbers = new List<double>();
            var operation = "*/-+";

            var i = 0;
            while (i < expression.Length)
            {
                switch (expression[i])
                {
                    case '-' when i == 0:
                    case '-' when operation.Contains(expression[i - 1]):
                        expression = Simplify(expression, ++i, numbers, false);
                        break;
                    default:
                        {
                            if (!operation.Contains(expression[i]))
                                expression = Simplify(expression, i, numbers, true);
                            break;
                        }
                }

                ++i;

                if (i >= expression.Length - 1) continue;
                while (expression[i] == ' ')
                {
                    if (i < expression.Length - 1)
                        ++i;
                    else
                        break;
                }
            }


            var allOperations = expression.Replace(" ", "")
                .Where(c => c < '0' || c > '9')
                .ToList();

            return Count(numbers, allOperations);
        }

        private static string Simplify(string expression, int i, ICollection<double> numbers, bool positive)
        {
            if (expression[i] == '(')
            {
                var start = i;
                var end = FindClosingBracket(i, expression);
                var result = CountSimple(expression.Substring(start + 1, end - start - 1));
                if (result != 0)
                {
                    numbers.Add(positive ? result : result * (-1));
                    start = positive ? start : start - 1;
                    var subString = expression.Substring(start, end - start + 1);
                    var regex = new Regex(Regex.Escape(subString));
                    expression = regex.Replace(expression, string.Empty, 1);
                }
            }
            else
            {
                var number = GetFirstNumber(expression.Substring(i, expression.Length - i));
                if (number != string.Empty)
                {
                    var parsedNumber = double.Parse(number, CultureInfo.InvariantCulture);
                    number = number.Replace(',', '.');
                    parsedNumber = positive ? parsedNumber : -1 * parsedNumber;
                    number = positive ? number : "-" + number;
                    numbers.Add(parsedNumber);

                    var regex = new Regex(Regex.Escape(number));
                    expression = regex.Replace(expression, " ", 1);
                }
            }

            return expression;
        }

        private static int FindClosingBracket(int i, string expression)
        {
            var stack = new Stack<char>();
            for (; i < expression.Length; ++i)
            {
                switch (expression[i])
                {
                    case '(':
                        stack.Push('(');
                        break;
                    case ')':
                        stack.Pop();
                        break;
                }

                if (stack.Count == 0)
                    break;
            }

            return i;
        }

        private static string GetFirstNumber(string expression)
        {
            var i = 0;
            var result = new List<string>();
            while (i < expression.Length)
            {
                if (expression[i] >= '0' && expression[i] <= '9')
                    result.Add(expression[i].ToString());
                else if (expression[i] == '.')
                    result.Add(".");
                else
                    break;
                ++i;
            }

            if (result.Count == 0) return string.Empty;
            return result.Aggregate((k, j) => k + j);
        }

        private static double Count(IReadOnlyCollection<double> numbers, IReadOnlyCollection<char> allOperations)
        {
            var operations = new Dictionary<char, Func<double, double, double>>
            {
                {'+', (x, y) => x + y},
                {'-', (x, y) => y - x},
                {'*', (x, y) => x * y},
                {'/', (x, y) => y / x}
            };
            var priorityOperations = new List<char>
            {
                '*',
                '/'
            };
            var numberStack = new Stack<double>();
            numberStack.Push(numbers.First());
            var operationStack = new Stack<char>();
            for (var i = 0; i < allOperations.Count; ++i)
            {
                if (i < numbers.Count - 1)
                    numberStack.Push(numbers.ElementAt(i + 1));
                operationStack.Push(allOperations.ElementAt(i));
                if (i < allOperations.Count - 1)
                    if (priorityOperations.Contains(allOperations.ElementAt(i + 1)) &&
                        !priorityOperations.Contains(allOperations.ElementAt(i)))
                        continue;
                while (operationStack.Count > 0)
                    numberStack.Push(operations[operationStack.Pop()](numberStack.Pop(), numberStack.Pop()));
            }

            return numberStack.Pop();
        }

    }
}
