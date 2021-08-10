using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfSimpleCalculator.Models;

namespace WpfSimpleCalculator
{
    public class Tokenizer
    {
        private static char[] _digits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ',' };

        private string digitList = "0";

        public static Token Parse(string input)
        {
            Token tok = new Token {Number=0m, Operator = eOperator.Equals, TokenType = eTokenType.Operator};
            if (string.IsNullOrEmpty(input))
                return tok;
            eOperator inputOperator = (eOperator)input[0];
            switch (inputOperator)
            {
                case eOperator.Plus:
                case eOperator.Minus:
                case eOperator.Multiply:
                case eOperator.Divide:                 
                    tok.TokenType = eTokenType.Operator;
                    tok.Operator = inputOperator;
                    break;

            };
            return tok;
        }

        public void AddDigit(string digit)
        {
            switch (digit)
            {
                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                    if (digitList == "0")
                        digitList = "";
                    digitList += digit;
                    break;
                case "c":
                    if (digitList.Length > 1)
                    {
                        digitList.Substring(0, digitList.Length - 1);
                    }
                    else
                    {
                        digitList = "0";
                    }
                    break;
                case ".":
                    if (!digitList.Contains("."))
                        digitList += ".";
                    break;
                case "±":
                    if (digitList.Length == 0 || digitList == "0")
                    {
                        //lcd.SetNumber(-lcd.GetNumber().Number);
                        break;
                    }
                    if (digitList.StartsWith("-"))
                        digitList = digitList.Substring(1);
                    else
                        digitList = "-" + digitList;
                    break;
                default:
                    break;
            }

            //lcd.Refresh();
        }

        public Token GetNumber()
        {
            decimal.TryParse(digitList, out decimal num);
            digitList = "0";
            return new Token { Number = num, TokenType = eTokenType.Number };
        }

        public string GetDigitlist() => digitList;
    }
}
