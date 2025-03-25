using Calculator.Model;
using Calculator.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Calculator.Commands
{
    public class InputHandler
    {
        private readonly TextBlock _equationTextBlock;
        private readonly TextBlock _resultTextBlock;
        private readonly CalculatorModel _calculator;

        public InputHandler(TextBlock equationTextBlock, TextBlock resultTextBlock, CalculatorModel calculator)
        {
            _equationTextBlock = equationTextBlock;
            _resultTextBlock = resultTextBlock;
            _calculator = calculator;
        }

        public void ButtonClickHandler(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string buttonContent = button.Content?.ToString() ?? "";

                bool isProgrammerMode = SettingsService.CurrentSettings.IsProgrammerMode;
                string currentBase = SettingsService.CurrentSettings.LastBaseUsed;

                if (isProgrammerMode)
                {
                    if ("+−×÷=MC MR MS M+ M- ⌫ ± 𝑥² √𝑥 ⅟𝑥 CE CLR".Split(' ').Contains(buttonContent))
                    {}
                    else
                    {
                        
                        if (currentBase == "2" && !"01".Contains(buttonContent)) return;
                        if (currentBase == "8" && !"01234567".Contains(buttonContent)) return;
                        if (currentBase == "16" && !"0123456789ABCDEF".Contains(buttonContent)) return;

                        if ("ABCDEF".Contains(buttonContent))
                        {
                            _calculator.AppendNumber(buttonContent);
                            UpdateDisplay();
                            return;
                        }
                    }
                }

                switch (buttonContent)
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
                        _calculator.AppendNumber(buttonContent);
                        break;

                    case "MC": _calculator.MemoryClear(); break;
                    case "MR": _calculator.MemoryRecall(); break;
                    case "MS": _calculator.MemoryStore(); break;
                    case "M+": _calculator.MemoryAdd(); break;
                    case "M-": _calculator.MemorySubtract(); break;

                    case "+":
                    case "−":
                    case "×":
                    case "÷":
                        _calculator.SetOperator(buttonContent);
                        break;

                    case ".":
                        _calculator.AppendNumber(".");
                        break;

                    case "=":
                        _calculator.Calculate();
                        break;
                    case "%":
                        _calculator.ApplyPercentage();
                        break;
                    case "⌫":
                        _calculator.Backspace();
                        break;
                    case "±":
                        _calculator.ToggleSign();
                        break;
                    case "𝑥²":
                        _calculator.Square();
                        break;
                    case "√𝑥":
                        _calculator.SquareRoot();
                        break;
                    case "⅟𝑥":
                        _calculator.Reciprocal();
                        break;
                    case "CLR":
                        _calculator.Clear();
                        break;
                    case "CE":
                        if (!isProgrammerMode) _calculator.ClearEntry();
                        break;
                }

                UpdateDisplay();
            }
        }

        public void KeyPressHandler(object sender, KeyEventArgs e)
        {
            bool isProgrammerMode = SettingsService.CurrentSettings.IsProgrammerMode;
            string currentBase = SettingsService.CurrentSettings.LastBaseUsed;

            if (isProgrammerMode)
            {
                if (e.Key == Key.Add || e.Key == Key.Subtract ||
                    e.Key == Key.Multiply || e.Key == Key.Divide ||
                    e.Key == Key.Enter || e.Key == Key.Back || e.Key == Key.Escape)
                {
                }
                else
                {
                    
                    string keyStr = e.Key.ToString(); 

                    if (currentBase == "2" &&
                       !"D0D1NumPad0NumPad1".Contains(keyStr)) return;

                    if (currentBase == "8" &&
                       !"D0D1D2D3D4D5D6D7NumPad0NumPad1NumPad2NumPad3NumPad4NumPad5NumPad6NumPad7".Contains(keyStr))
                        return;
                    if (currentBase == "16" &&
                       !("D0D1D2D3D4D5D6D7D8D9" +"NumPad0NumPad1NumPad2NumPad3NumPad4NumPad5NumPad6NumPad7NumPad8NumPad9" +"ABCDEF").Contains(keyStr))
                        return;
                    


                }
            }

            switch (e.Key)
            {
                case Key.D0: case Key.NumPad0: _calculator.AppendNumber("0"); break;
                case Key.D1: case Key.NumPad1: _calculator.AppendNumber("1"); break;
                case Key.D2: case Key.NumPad2: _calculator.AppendNumber("2"); break;
                case Key.D3: case Key.NumPad3: _calculator.AppendNumber("3"); break;
                case Key.D4: case Key.NumPad4: _calculator.AppendNumber("4"); break;
                case Key.D5: case Key.NumPad5: _calculator.AppendNumber("5"); break;
                case Key.D6: case Key.NumPad6: _calculator.AppendNumber("6"); break;
                case Key.D7: case Key.NumPad7: _calculator.AppendNumber("7"); break;
                case Key.D8: case Key.NumPad8: _calculator.AppendNumber("8"); break;
                case Key.D9: case Key.NumPad9: _calculator.AppendNumber("9"); break;

                case Key.Add: case Key.OemPlus: _calculator.SetOperator("+"); break;
                case Key.Subtract: case Key.OemMinus: _calculator.SetOperator("−"); break;
                case Key.Multiply: _calculator.SetOperator("×"); break;
                case Key.Divide: case Key.OemQuestion: _calculator.SetOperator("÷"); break;

                case Key.A: _calculator.AppendNumber("A"); break;
                case Key.B: _calculator.AppendNumber("B"); break;
                case Key.C: _calculator.AppendNumber("C"); break;
                case Key.D: _calculator.AppendNumber("D"); break;
                case Key.E: _calculator.AppendNumber("E"); break;
                case Key.F: _calculator.AppendNumber("F"); break;

                case Key.Enter:
                    _calculator.Calculate();
                    break;

                case Key.Back:
                    _calculator.Backspace();
                    break;

                case Key.Escape:
                    _calculator.Clear();
                    break;

                case Key.OemPeriod:
                case Key.Decimal:
                    _calculator.AppendNumber(".");
                    break;
            }

            UpdateDisplay();
            e.Handled = true;
        }

        private void UpdateDisplay()
        {
            string expression = _calculator.Expression;

            Application.Current.Dispatcher.Invoke(() =>
            {
                if (string.IsNullOrEmpty(expression))
                {
                    _equationTextBlock.Text = "";
                }
                else if (_calculator.HasCalculated)
                {
                    if (!_equationTextBlock.Text.EndsWith("="))
                    {
                        _equationTextBlock.Text = expression;
                    }
                }
                else
                {
                    _equationTextBlock.Text = expression;
                }

                _resultTextBlock.Text = _calculator.GetDisplayText();
            });
        }
    }
}
