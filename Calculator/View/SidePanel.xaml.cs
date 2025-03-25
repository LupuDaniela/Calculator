using Calculator.Services;
using System.Windows;
using System.Windows.Controls;

namespace Calculator.View
{
    public partial class SidePanel : Window
    {
        private ICalculatorOwner _owner;

        public SidePanel(ICalculatorOwner owner)
        {
            InitializeComponent();
            _owner = owner;

            this.ResizeMode = ResizeMode.NoResize;

            StandardModeButton.IsChecked = !SettingsService.CurrentSettings.IsProgrammerMode;
            ProgrammerModeButton.IsChecked = SettingsService.CurrentSettings.IsProgrammerMode;

            StandardCalcMode.IsChecked = SettingsService.CurrentSettings.RespectOperatorPrecedence;
            InOrderCalcMode.IsChecked = !SettingsService.CurrentSettings.RespectOperatorPrecedence;

            DigitGroupingToggleButton.IsChecked = SettingsService.CurrentSettings.IsDigitGroupingEnabled;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            _owner.SetSidePanelState(false);
        }

        private void ProgrammerModeButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsService.CurrentSettings.IsProgrammerMode = true;
            SettingsService.SaveSettings(SettingsService.CurrentSettings);

            ProgrammerMode progWin = new ProgrammerMode();
            progWin.Show();

            if (_owner is Window win)
            {
                win.Hide();
            }
        }

        private void ToggleDigitGrouping_Click(object sender, RoutedEventArgs e)
        {
            bool newState = !SettingsService.CurrentSettings.IsDigitGroupingEnabled;

            SettingsService.CurrentSettings.IsDigitGroupingEnabled = newState;
            _owner.CalculatorModel.IsDigitGroupingEnabled = newState;

            DigitGroupingToggleButton.IsChecked = newState;

            SettingsService.SaveSettings(SettingsService.CurrentSettings);
            _owner.RefreshDisplay();
        }

        private void CutButton_Click(object sender, RoutedEventArgs e)
        {
            var cb = new ClipboardService();
            cb.SetText(_owner.CalculatorModel.CurrentInput);
            _owner.CalculatorModel.ClearEntry();
            _owner.RefreshDisplay();
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            var cb = new ClipboardService();
            cb.SetText(_owner.CalculatorModel.CurrentInput);
        }

        private void PasteButton_Click(object sender, RoutedEventArgs e)
        {
            var cb = new ClipboardService();
            string text = cb.GetText()?.Trim();
            if (!string.IsNullOrEmpty(text))
            {
                _owner.CalculatorModel.Clear();

                bool hasDecimal = false;

                foreach (char c in text)
                {
                    if (char.IsDigit(c))
                    {
                        _owner.CalculatorModel.AppendNumber(c.ToString());
                    }
                    else if ((c == '.' || c == ',') && !hasDecimal)
                    {
                        _owner.CalculatorModel.AppendNumber(".");
                        hasDecimal = true;
                    }
                }
                _owner.RefreshDisplay();
            }
        }


        private void CalcMode_Changed(object sender, RoutedEventArgs e)
        {
            if (StandardCalcMode == null || InOrderCalcMode == null) return;

            if (StandardCalcMode.IsChecked == true)
            {
                SettingsService.CurrentSettings.RespectOperatorPrecedence = true;
                _owner.CalculatorModel.RespectOperatorPrecedence = true;
            }
            else if (InOrderCalcMode.IsChecked == true)
            {
                SettingsService.CurrentSettings.RespectOperatorPrecedence = false;
                _owner.CalculatorModel.RespectOperatorPrecedence = false;
            }
            SettingsService.SaveSettings(SettingsService.CurrentSettings);
        }

        private void Mode_Changed(object sender, RoutedEventArgs e)
        {
            if (StandardModeButton == null || ProgrammerModeButton == null) return;

            bool isCurrentlyProgrammer = SettingsService.CurrentSettings.IsProgrammerMode;

            if (StandardModeButton.IsChecked == true && isCurrentlyProgrammer)
            {
                SettingsService.CurrentSettings.IsProgrammerMode = false;
                SettingsService.SaveSettings(SettingsService.CurrentSettings);

                MainWindow stdWin = new MainWindow();
                stdWin.Show();

                if (_owner is Window win)
                    win.Close();
            }
            else if (ProgrammerModeButton.IsChecked == true && !isCurrentlyProgrammer)
            {
                SettingsService.CurrentSettings.IsProgrammerMode = true;
                SettingsService.SaveSettings(SettingsService.CurrentSettings);

                ProgrammerMode progWin = new ProgrammerMode();
                progWin.Show();

                if (_owner is Window win)
                    win.Close();
            }
        }
    }
}