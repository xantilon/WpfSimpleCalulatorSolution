using System;
using System.ComponentModel;
using System.Globalization;
using WpfSimpleCalculator.Models;

namespace WpfSimpleCalculator
{

    /// <summary>
    /// Das LCD ist für die Anzeige zuständig (logisch), aber auch für das Bauen der 
    /// Zahlen aus den eingegebenen Ziffern.
    /// </summary>
    public class LCD : INotifyPropertyChanged
    {
        private string digitList = "";
        private decimal number = 0m;

        /// <summary>
        /// gives the correctly formatted digitList
        /// </summary>
        public string Display
        {
            get
            {
                if (string.IsNullOrEmpty(digitList))
                    return number.ToString("N2", new CultureInfo("de_DE").NumberFormat);
                return Format(digitList);
            }
        }

        private string Format(string digitList)
        {
            string ret = digitList.Replace(".", ",");

            //add thousands separator
            var nfi = new NumberFormatInfo()
            {
                NumberDecimalDigits = 0,
                NumberGroupSeparator = "."
            };
            var splits = ret.Split(',');           
            int i = int.Parse(splits[0]);
            ret = i.ToString("N", nfi); // "1.234.567.890"            
            if (splits.Length > 1)
                ret += $",{splits[1]}";
            
            return ret;
        }

        public string Message { get; set; } = "Hello world!";


        public void AddInput(string digit)
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
                        digitList = digit;
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
                        number = -number;
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
            
            OnPropertyChanged("Display");
        }

        public Token GetNumber() 
        {
            decimal.TryParse(digitList, out decimal num);
            return new Token { Number = num, TokenType = eTokenType.Number };
        }

        public void SetNumber(decimal num)
        {
            number = num;
            digitList = "";
            OnPropertyChanged("Display");
        }


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
