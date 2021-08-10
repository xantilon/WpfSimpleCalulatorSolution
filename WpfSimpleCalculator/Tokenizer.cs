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
        private string digitList = "";
        private decimal num = 0m;

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
                    if (digitList.Length < 12)
                    {
                        if (digitList == "0")
                            digitList = "";
                        digitList += digit;
                    }
                    break;
                case "c":
                    num = 0;
                    if (digitList.Length > 1)
                    {
                        digitList = digitList.Substring(0, digitList.Length - 1);
                        if(digitList == "-") digitList = "";
                    }
                    else
                    {
                        digitList = "";
                    }
                    break;
                case ".":
                    if (!digitList.Contains("."))
                        digitList += ".";
                    break;
                case "±":
                    if (digitList.Length == 0 || digitList == "0")
                    {
                        digitList = (-num).ToString(CultureInfo.InvariantCulture.NumberFormat);
                        num = 0;
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
            if(!string.IsNullOrEmpty(digitList) )//&& digitList != "0")
                decimal.TryParse(digitList, NumberStyles.Float, CultureInfo.InvariantCulture, out num);
            digitList = "";
            return new Token { Number = num, TokenType = eTokenType.Number };
        }
        public void SetNumber(decimal? number) => num = number??0m;
       
            public string GetDigitlist() => digitList;
    }
}
