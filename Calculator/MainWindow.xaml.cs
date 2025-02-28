using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Calculator;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    string currentInput = "";
    string fullExpression = "";
    bool isNewOperation = true;
    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = this;
        Divide.Content = "\u00F7";
        SquareNumber.Content = "x\u00b2";
        SquareRoot.Content = "\u221Ax";
    }


    private void NameButton_Click(object sender, RoutedEventArgs e)
    {
        string name = ((Button)sender).Name;
        string digit = "";

        switch (name)
        {
            case "ZeroButton": digit = "0"; break;
            case "OneButton": digit = "1"; break;
            case "TwoButton": digit = "2"; break;
            case "ThreeButton": digit = "3"; break;
            case "FourButton": digit = "4"; break;
            case "FiveButton": digit = "5"; break;
            case "SixButton": digit = "6"; break;
            case "SevenButton": digit = "7"; break;
            case "EightButton": digit = "8"; break;
            case "NineButton": digit = "9"; break;
        }

        if (isNewOperation)
        {
            currentInput = "";
            isNewOperation = false;
        }

        currentInput += digit;
        OutputText.Text = currentInput;
        fullExpression += digit;
        AllEcuationOut.Text = fullExpression;
    }

    private void Add_Click(object sender, RoutedEventArgs e)
    {
        ProcessOperator("+");
    }

    private void Minus_Click(object sender, RoutedEventArgs e)
    {
        ProcessOperator("-");
    }

    private void Multiply_Click(object sender, RoutedEventArgs e)
    {
        ProcessOperator("*");
    }

    private void Divide_Click(object sender, RoutedEventArgs e)
    {
        ProcessOperator("/");
    }

    private void ProcessOperator(string op)
    {
        if (currentInput == "" && fullExpression == "")
            return;

        if (fullExpression.Length > 0 && IsOperator(fullExpression[fullExpression.Length - 1].ToString()))
        {
            fullExpression = fullExpression.Substring(0, fullExpression.Length - 1) + GetDisplayOperator(op);
        }
        else
        {
            fullExpression += GetDisplayOperator(op);
        }

        AllEcuationOut.Text = fullExpression;
        isNewOperation = true;
    }

    private string GetDisplayOperator(string op)
    {
        switch (op)
        {
            case "+": return "+";
            case "-": return "-";
            case "*": return "x";
            case "/": return "÷";
            default: return op;
        }
    }

    private bool IsOperator(string character)
    {
        return character == "+" || character == "-" || character == "x" || character == "÷";
    }

    private string ConvertToCalculableExpression(string displayExpression)
    {
        return displayExpression.Replace("x", "*").Replace("÷", "/");
    }

    private void Equal_Click(object sender, RoutedEventArgs e)
    {
        if (fullExpression == "")
            return;

        try
        {
            // Used to calculate in order
            string expressionToCalculate = ConvertToCalculableExpression(fullExpression);
            DataTable dt = new DataTable();
            var result = dt.Compute(expressionToCalculate, "");

           
            fullExpression += "=" + result.ToString();
            AllEcuationOut.Text = fullExpression;

            currentInput = result.ToString();
            OutputText.Text = currentInput;

            fullExpression = result.ToString();
            isNewOperation = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error in calculation: " + ex.Message, "Calculation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Backspace_Click(object sender, RoutedEventArgs e)
    {
        if (currentInput.Length > 0)
        {
            currentInput = currentInput.Substring(0, currentInput.Length - 1);
            if (currentInput == "")
                currentInput = "0";

            OutputText.Text = currentInput;
        }

        if (fullExpression.Length > 0)
        {
            fullExpression = fullExpression.Substring(0, fullExpression.Length - 1);
            AllEcuationOut.Text = fullExpression;
        }
    }

    private void Clear_Click(object sender, RoutedEventArgs e)
    {
        currentInput = "";
        fullExpression = "";
        OutputText.Text = "0";
        AllEcuationOut.Text = "";
        isNewOperation = true;
    }

    private void ClearRecentEntry_Click(object sender, RoutedEventArgs e)
    {
        currentInput = "";
        OutputText.Text = "0";

        // If the last part of the expression is a number, remove it
        if (fullExpression.Length > 0)
        {
            string pattern = @"\d+$"; // Match digits at the end of the string
            string newExpression = System.Text.RegularExpressions.Regex.Replace(
                fullExpression, pattern, "");

            fullExpression = newExpression;
            AllEcuationOut.Text = fullExpression;
        }

        isNewOperation = true;
    }

    private void Square_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(currentInput))
            return;

        try
        {
            double value = double.Parse(currentInput);
            double result = value * value;

            currentInput = result.ToString();
            OutputText.Text = currentInput;

            fullExpression += "²=" + result;
            AllEcuationOut.Text = fullExpression;

            fullExpression = result.ToString();
            isNewOperation = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error calculating square: " + ex.Message, "Calculation Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void SquareRoot_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(currentInput))
            return;
        try
        {
            string pattern = @"(\d+(\.\d+)?)$"; 
            Match match = Regex.Match(currentInput, pattern);

            if (!match.Success)
                return;

            double value = double.Parse(match.Value);
            if (value < 0)
            {
                MessageBox.Show("Cannot calculate square root of a negative number", "Error",
                               MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            double result = Math.Sqrt(value);

            currentInput = Regex.Replace(currentInput, pattern, result.ToString());
            OutputText.Text = currentInput;

            fullExpression = fullExpression.Replace(match.Value, "√(" + match.Value + ")");
            AllEcuationOut.Text = fullExpression;

            fullExpression = result.ToString();
            isNewOperation = true;
        }
        catch(Exception ex)
        {
            MessageBox.Show("Error calculating square root: " + ex.Message, "Calculation Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Percentage_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(currentInput) || string.IsNullOrEmpty(fullExpression))
            return;


        try
        {
            double percentageValue = double.Parse(currentInput) / 100;

            string[] parts = fullExpression.Split(new char[] { '+', '-', '*', '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
                return;

            double baseValue = double.Parse(parts[parts.Length - 2]);

            double result = baseValue * percentageValue;

            currentInput = result.ToString();
            OutputText.Text = currentInput;

            fullExpression += "% = " + result;
            AllEcuationOut.Text = fullExpression;

            isNewOperation = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error calculating percentage: " + ex.Message,
                            "Calculation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Reciprocal_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(currentInput))
            return;

        try
        {
            double value = double.Parse(currentInput);

            if (value == 0)
            {
                MessageBox.Show("Cannot divide by zero!", "Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            double result = 1 / value;

            currentInput = result.ToString();
            OutputText.Text = currentInput;

            fullExpression = "1/(" + fullExpression + ") = " + result;
            AllEcuationOut.Text = fullExpression;

            isNewOperation = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error calculating reciprocal: " + ex.Message,
                            "Calculation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
