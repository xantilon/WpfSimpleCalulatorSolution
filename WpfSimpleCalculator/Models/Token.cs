using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfSimpleCalculator.Models
{
    public class Token
    {
        public eTokenType TokenType { get; set; }
        public decimal Number { get; set; }
        public eOperator Operator { get; set; }
    }
}
