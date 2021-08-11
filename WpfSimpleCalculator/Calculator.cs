using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WpfSimpleCalculator.Models;

namespace WpfSimpleCalculator
{
    public class Calculator : INotifyPropertyChanged
    {
        private LinkedList<Token> inputQueue = new();
        private string errmsg = "";
        private Tokenizer tokenizer;
        private decimal? result;


        public Calculator()
        {
            tokenizer = new Tokenizer();
        }

        public static bool IsDigit(string input)
        {
            return Regex.IsMatch(input, @"[\d\.±c]");
        }

        /// <summary>
        /// tries to reduce the inputQueue and returns the result for displaying
        /// </summary>
        /// <returns></returns>
        public void DoWork()
        {
            /// first TOO simple approach (no algebraic math)
            if (inputQueue.Any() && inputQueue.First().TokenType != eTokenType.Number)
            {
                inputQueue.RemoveFirst();
            }

            if (inputQueue.Any() && inputQueue.First()?.Operator == eOperator.Equals)
            {
                inputQueue.Clear();
                errmsg = "queue empty";
            }
            else if (inputQueue.Count >= 3)// assume it's always NUMBER OPERATOR NUMBER
            {
                decimal n1 = inputQueue.First().Number; inputQueue.RemoveFirst();
                eOperator op1 = inputQueue.First().Operator; inputQueue.RemoveFirst();
                decimal n2 = inputQueue.First().Number; inputQueue.RemoveFirst();
                errmsg += $" {n1} {op1} {n2}";
                result = Calculate(n1, n2, op1);
                _ = inputQueue.AddFirst(new Token
                {
                    Number = result!.Value,
                    TokenType = eTokenType.Number
                });
            }


            
        }


        internal void AddInput(string buttonContent)
        {
            if (IsDigit(buttonContent))
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
                    inputQueue.RemoveLast();
                }
                _ = inputQueue.AddLast(numTok);

                // get the op
                Token? tok = Tokenizer.Parse(buttonContent);
                result = 0m;
                if (tok != null)
                {
                    if (tok.Operator == eOperator.Clear)
                    {
                        inputQueue.Clear();                  
                    }
                    if (tok.Operator == eOperator.Equals && inputQueue.Count < 3)
                    {
                        result = inputQueue.FirstOrDefault(t => t.TokenType == eTokenType.Number)?.Number ?? 0m;
                        tokenizer.SetNumber(result);
                        inputQueue.Clear();
                        inputQueue.AddLast(new Token { Number = result.Value, TokenType=eTokenType.Number });
                    }
                    else
                    {
                        // if last was number, replace it
                        if (inputQueue.LastOrDefault()?.TokenType == eTokenType.Operator)
                        {
                            inputQueue.RemoveLast();
                        }
                        inputQueue.AddLast(tok); 
                        result = inputQueue.LastOrDefault(t => t.TokenType == eTokenType.Number)?.Number ?? 0m;
                    }
                }               
            }
            OnPropertyChanged("Queue");
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
                    if (n2 == 0)
                    {
                        errmsg = "durch NULL Teilen ist nicht erlaubt!";
                        return 0m;
                    }
                    return n1 / n2;
                case eOperator.Clear: return 0m;
                case eOperator.Equals: return n1;
                default:
                    errmsg = "Aktion nicht erlaubt";
                    return 0m;
            }
        }


        public string Queue => $"queue: {string.Join(' ',inputQueue.Select(q => q.ToString()))}";







        // Declare event
        public event PropertyChangedEventHandler PropertyChanged;
        // OnPropertyChanged to update property value in binding
        private void OnPropertyChanged(string propName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
