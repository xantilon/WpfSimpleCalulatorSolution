using System;
using System.ComponentModel;
using System.Globalization;
using WpfSimpleCalculator.Models;

namespace WpfSimpleCalculator
{

    /// <summary>
    /// Das LCD ist für die Anzeige zuständig (logisch)
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
                {
                    //return number.ToString("N3", new CultureInfo("de_DE").NumberFormat);
                    string numberFormatted = number.ToString("0.###########", new CultureInfo("de_DE").NumberFormat);
                    if (number == Math.Floor(number))
                        numberFormatted += "."; 
                    return numberFormatted;
                }
                return Format(digitList);
            }
        }

        //public string DigitList { get=>digitList; set => digitList=value; }

        public void Refresh() => OnPropertyChanged("Display");

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
            var i = Int64.Parse(splits[0]);
            ret = i.ToString("N", nfi); // "1.234.567.890"            
            if (splits.Length > 1)
                ret += $",{splits[1]}";

            return ret;
        }

        public string Message { get; private set; } = "Hello world!";


        //public void AddInput(string digit)
        //{
        //    switch (digit)
        //    {
        //        case "0":
        //        case "1":
        //        case "2":
        //        case "3":
        //        case "4":
        //        case "5":
        //        case "6":
        //        case "7":
        //        case "8":
        //        case "9":
        //            if (digitList == "0")
        //                digitList = digit;
        //            digitList += digit;
        //            break;
        //        case "c":
        //            if (digitList.Length > 1)
        //            {
        //                digitList.Substring(0, digitList.Length - 1);
        //            }
        //            else
        //            {
        //                digitList = "0";
        //            }
        //            break;
        //        case ".":
        //            if (!digitList.Contains("."))
        //                digitList += ".";
        //            break;
        //        case "±":
        //            if (digitList.Length == 0 || digitList == "0")
        //            {
        //                number = -number;
        //                break;
        //            }                        
        //            if (digitList.StartsWith("-"))
        //                digitList = digitList.Substring(1);
        //            else
        //                digitList = "-" + digitList;
        //            break;
        //        default:
        //            break;
        //    }            

        //    OnPropertyChanged("Display");
        //}



        public void SetNumber(decimal num)
        {
            number = num;
            digitList = "";
            OnPropertyChanged("Display");
        }

        public void SetNumberString(string digits)
        {
            digitList = digits;
            OnPropertyChanged("Display");
        }

        public void SetErrorMessage(string msg) 
        {
            Message = msg;
            OnPropertyChanged("Message");
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
