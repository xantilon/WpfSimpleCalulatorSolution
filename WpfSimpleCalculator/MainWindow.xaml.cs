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
        private Calculator calc = new Calculator();
        private Calculator calc2 = new Calculator();

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

            calc.AddInput(buttonContent);
            calc.DoWork();
            calc.SetDisplay(lcd);
            ////////////////////////////////////////////////////////////////////////////////////////

            ////////////////////////////////////////////////////////////////////////////////////////
            // LOWER DISPLAY: binding LCD2 from code behind
            calc2.AddInput(buttonContent);
            calc2.DoWork();
            calc2.SetDisplay(MainLCD);
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
