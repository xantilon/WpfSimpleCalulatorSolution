using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

#nullable enable
namespace WpfSimpleCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Tokenizer tokenizer = new Tokenizer();
        private static string errorMessage = "";
        public LCD MainLCD { get; set; } = new LCD();
        private Calculator calc = new Calculator(ref errorMessage);
        private Calculator calc2 = new Calculator(ref errorMessage);

        public MainWindow()
        {
            InitializeComponent();


            ////////////////////////////////////////////////////////////////////////////////////////
            ///// LOWER DISPLAY: Bind the data source to the TextBox control's Text dependency property
            var lcdBinding = new Binding("Display")
            {
                Source = MainLCD,
                Mode = BindingMode.OneWay,
                NotifyOnSourceUpdated = true
            };
            LcdBox2.SetBinding(TextBox.TextProperty, lcdBinding);
            ///////////////////////////////////////////////////////////////////////////////////////


            // bind a textBlock for error/status messages
            var msgBinding = new Binding("Message") { Source = MainLCD };
            Messages.SetBinding(TextBlock.TextProperty, msgBinding);
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string buttonContent = ((sender as Button)?.Tag as string) ?? "?";

            ////////////////////////////////////////////////////////////////////////////////////////
            //UPPER DISPLAY: if LCD is instantiated from XAML, get instance from event source
            var bindingExpression = BindingOperations.GetBindingExpression(LcdBox, TextBox.TextProperty);
            var lcd = (LCD)bindingExpression.DataItem;

            if (Calculator.IsDigit(buttonContent))
            {
                lcd.AddInput(buttonContent);
            }
            else
            {
                var tok = Tokenizer.Parse(buttonContent);
                if (tok != null)
                {
                    calc.AddInput(lcd.GetNumber(), tok);
                    lcd.SetNumber(calc.DoWork());
                    lcd.Message = errorMessage;
                }
            }
            ////////////////////////////////////////////////////////////////////////////////////////

            ////////////////////////////////////////////////////////////////////////////////////////
            // LOWER DISPLAY: binding LCD2 from code behind
            if (Calculator.IsDigit(buttonContent))
            {
                MainLCD.AddInput(buttonContent);
            }
            else
            {
                var tok = Tokenizer.Parse(buttonContent);
                if (tok != null)
                {
                    calc2.AddInput(MainLCD.GetNumber(), tok);
                    MainLCD.SetNumber(calc2.DoWork());
                    MainLCD.Message = errorMessage;
                }
            }
            ////////////////////////////////////////////////////////////////////////////////////////
        }

        // not used here, but could handle Post-Display-Change event
        private void OnTargetUpdated(object sender, DataTransferEventArgs args)
        {
            // Handle event
            var fe = sender as FrameworkElement;

        }
    }
}
