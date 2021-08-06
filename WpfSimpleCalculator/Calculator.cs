using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WpfSimpleCalculator.Models;

namespace WpfSimpleCalculator
{
    public class Calculator
    {
        private Queue<Token> inputQueue = new();
        private StringBuilder _digitList = new("0");
        private string errmsg;

        public Calculator(ref string errorMessage)
        {
            errmsg = errorMessage;
        }

        public static bool IsDigit(string input) => Regex.IsMatch(input, @"[\d,±c]");

        public void AddInput(Token currentNumber, Token input)
        {
            if (input.Operator == eOperator.Clear)
            {
                inputQueue.Clear();
                return;
            }

            if (inputQueue.LastOrDefault()?.TokenType == eTokenType.Number)
                inputQueue.Undo();
            inputQueue.Enqueue(currentNumber);
            inputQueue.Enqueue(input);
        }
        
        /// <summary>
        /// tries to reduce the inputQueue and returns the result for displaying
        /// </summary>
        /// <returns></returns>
        public decimal DoWork()
        {
            decimal ret = 0m;
            /// first TOO simple approach (no algebraic math)
            if (inputQueue.Any() && inputQueue.Peek().TokenType != eTokenType.Number)
                _ = inputQueue.Dequeue();

            // assume it's always NUMBER OPERATOR NUMBER
            if (inputQueue.Count >= 3)
            {
                decimal n1 = inputQueue.Dequeue().Number;
                eOperator op1 = inputQueue.Dequeue().Operator;
                decimal n2 = inputQueue.Dequeue().Number;
                errmsg = $"{n1}{op1.ToString()}{n2}";
                decimal result = Calculate(n1, n2, op1);
                inputQueue.Prepend(new Token
                {
                    Number = result,
                    TokenType = eTokenType.Number
                });
                ret = result;
            }
            else
            {

                ret = inputQueue.LastOrDefault(t => t.TokenType == eTokenType.Number)?.Number ?? 0m;
            }

            if (inputQueue.Any() && inputQueue.Peek()?.Operator == eOperator.Equals)
                inputQueue.Clear();

            return ret;
        }

        private decimal Calculate(decimal n1, decimal n2, eOperator op)
        {
            switch (op)
            {
                case eOperator.Plus: return n1 + n2;
                case eOperator.Minus: return n1 - n2;
                case eOperator.Multiply: return n1 * n2;
                case eOperator.Divide: 
                    if(n2 == 0)
                    {
                        errmsg = "durch NULL Teilen ist nicht erlaubt!";
                        return 0m;
                    }
                    return n1 / n2;
                default:
                    errmsg = "Aktion nicht erlaubt";
                    return 0m;
            }
        }
    }
}
