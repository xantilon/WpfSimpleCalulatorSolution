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


        public static Token? Parse(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;
            eOperator inputOperator = (eOperator)input[0];
            Token tok = new();
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
    }
}
