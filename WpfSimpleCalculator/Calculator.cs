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
        private Tokenizer tokenizer;
        private decimal? result = null;


        public Calculator()
        {
            tokenizer = new Tokenizer();
        }

        public static bool IsDigit(string input) => Regex.IsMatch(input, @"[\d,±c]");

        /// <summary>
        /// tries to reduce the inputQueue and returns the result for displaying
        /// </summary>
        /// <returns></returns>
        public void DoWork()
        {
            /// first TOO simple approach (no algebraic math)
            if (inputQueue.Any() && inputQueue.Peek().TokenType != eTokenType.Number)
                _ = inputQueue.Dequeue();

            errmsg = $"queue: [{string.Join("] [", inputQueue.Select(q => q.ToString()).ToList())}]";

            // assume it's always NUMBER OPERATOR NUMBER
            if (inputQueue.Count >= 3)
            {
                decimal n1 = inputQueue.Dequeue().Number;
                eOperator op1 = inputQueue.Dequeue().Operator;
                decimal n2 = inputQueue.Dequeue().Number;
                errmsg += $" {n1} {op1.ToString()} {n2}";
                result = Calculate(n1, n2, op1);
                inputQueue.Prepend(new Token
                {
                    Number = result!.Value,
                    TokenType = eTokenType.Number
                });
            }


            if (inputQueue.Any() && inputQueue.Peek()?.Operator == eOperator.Equals)
            {
                inputQueue.Clear();
                errmsg = "queue empty";
            }
        }


        internal void AddInput(string buttonContent)
        {
            if (Calculator.IsDigit(buttonContent))
            {
                tokenizer.AddDigit(buttonContent);
            }
            else
            {
                //first get the number
                Token numTok = tokenizer.GetNumber();

                // if last was number, replace it
                if (inputQueue.LastOrDefault()?.TokenType == eTokenType.Number)
                {
                    inputQueue.Undo();
                }
                inputQueue.Enqueue(numTok);

                // get the op
                var tok = Tokenizer.Parse(buttonContent);

                if (tok != null)
                {
                    if (tok.Operator == eOperator.Clear)
                    {
                        inputQueue.Clear();
                    }
                    else
                    {
                        inputQueue.Enqueue(tok);
                    }
                }
                result = inputQueue.LastOrDefault(t => t.TokenType == eTokenType.Number)?.Number ?? 0m;
            }
        }

        internal void SetDisplay(LCD lcd)
        {
            if (result != null)
            {
                lcd.SetNumber(result.Value);
                result = null;
            }
            else
                lcd.SetNumberString(tokenizer.GetDigitlist());

            lcd.SetErrorMessage(errmsg);
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
