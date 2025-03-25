using Calculator.Services;
using Calculator.Model;
using Calculator.Commands;
using System;
using System.Windows;
using System.Windows.Input;
using System.Globalization;

namespace Calculator.View
{
    public partial class ProgrammerMode : Window, ICalculatorOwner
    {
        private CalculatorModel _calculator;
        private InputHandler _inputHandler;
        private string _currentBase = "10";

        private Window _sidePanelWindow;
        private bool _isSidePanelOpen = false;

        public ProgrammerMode()
        {
            InitializeComponent();

            _calculator = new CalculatorModel();
            _inputHandler = new InputHandler(EquationTextBlock, ResultTextBlock, _calculator);

            _currentBase = SettingsService.CurrentSettings.LastBaseUsed;
            Title = $"Programmer Mode (Base {_currentBase})";
            _calculator.IsDigitGroupingEnabled = SettingsService.CurrentSettings.IsDigitGroupingEnabled;

            RefreshDisplay();

            AttachButtonHandlers();
            this.KeyDown += _inputHandler.KeyPressHandler;
            this.LocationChanged += ProgrammerMode_LocationChanged;

            if (MemoryButton != null)
            {
                MemoryButton.Click += MemoryButton_Click;
            }
            InitializeSidePanel();
        }

        public CalculatorModel CalculatorModel => _calculator;

        public void RefreshDisplay()
        {
            EquationTextBlock.Text = _calculator.Expression;
            ResultTextBlock.Text = _calculator.GetDisplayText();
        }

        public void SetSidePanelState(bool isOpen)
        {
            _isSidePanelOpen = isOpen;
        }

        private void AttachButtonHandlers()
        {
            MenuButton.Click += MenuButton_Click;
            BinButton.Click += Bin_Click;
            OctButton.Click += Oct_Click;
            DecButton.Click += Dec_Click;
            HexButton.Click += Hex_Click;

            foreach (var child in ButtonGrid.Children)
            {
                if (child is System.Windows.Controls.Grid grid)
                {
                    foreach (var gridChild in grid.Children)
                    {
                        if (gridChild is System.Windows.Controls.Button btn &&
                            btn.Name != "MenuButton" &&
                            btn.Name != "BinButton" &&
                            btn.Name != "OctButton" &&
                            btn.Name != "DecButton" &&
                            btn.Name != "HexButton" &&
                            btn.Name != "BackToStandardButton")
                        {
                            btn.Click += _inputHandler.ButtonClickHandler;
                        }
                    }
                }
                else if (child is System.Windows.Controls.Button button &&
                         button.Name != "MenuButton" &&
                         button.Name != "BinButton" &&
                         button.Name != "OctButton" &&
                         button.Name != "DecButton" &&
                         button.Name != "HexButton" &&
                         button.Name != "BackToStandardButton")
                {
                    button.Click += _inputHandler.ButtonClickHandler;
                }
            }
        }

        private void InitializeSidePanel()
        {
            _sidePanelWindow = new SidePanel(this)
            {
                Width = 300,
                Height = 800,
                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.NoResize,
                ShowInTaskbar = false
            };

            this.Closed += (s, e) => _sidePanelWindow.Close();
            this.ContentRendered += (s, e) => _sidePanelWindow.Owner = this;
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isSidePanelOpen)
            {
                _sidePanelWindow.Hide();
            }
            else
            {
                PositionSidePanel();
                _sidePanelWindow.Show();
            }
            _isSidePanelOpen = !_isSidePanelOpen;
        }

        private void ProgrammerMode_LocationChanged(object sender, EventArgs e)
        {
            if (_isSidePanelOpen)
            {
                PositionSidePanel();
            }
        }

        private void PositionSidePanel()
        {
            double newLeft = this.Left - _sidePanelWindow.Width;
            if (newLeft < 0) newLeft = 0;

            _sidePanelWindow.Left = newLeft;
            _sidePanelWindow.Top = this.Top;
        }

        private double GetDecimalValueFromInput()
        {
            bool isProg = SettingsService.CurrentSettings.IsProgrammerMode;
            string currentBase = SettingsService.CurrentSettings.LastBaseUsed;

            try
            {
                if (isProg && currentBase != "10")
                {
                    return Convert.ToInt64(_calculator.CurrentInput, int.Parse(currentBase));
                }
                else
                {
                    return double.Parse(_calculator.CurrentInput, CultureInfo.InvariantCulture);
                }
            }
            catch
            {
                return 0;
            }
        }

        private void Bin_Click(object sender, RoutedEventArgs e)
        {
            double decVal = (_calculator.CurrentInput != "0") ? GetDecimalValueFromInput() : _calculator.RunningTotal;
            _calculator.CurrentInput = _calculator.ConvertToBase(decVal, "2");
            Title = "Programmer Mode (Base 2)";
            RefreshDisplay();
            SettingsService.CurrentSettings.LastBaseUsed = "2";
            SettingsService.SaveSettings(SettingsService.CurrentSettings);
        }

        private void Oct_Click(object sender, RoutedEventArgs e)
        {
            double decVal = (_calculator.CurrentInput != "0") ? GetDecimalValueFromInput() : _calculator.RunningTotal;
            _calculator.CurrentInput = _calculator.ConvertToBase(decVal, "8");
            Title = "Programmer Mode (Base 8)";
            RefreshDisplay();
            SettingsService.CurrentSettings.LastBaseUsed = "8";
            SettingsService.SaveSettings(SettingsService.CurrentSettings);
        }

        private void Dec_Click(object sender, RoutedEventArgs e)
        {
            double decVal = (_calculator.CurrentInput != "0") ? GetDecimalValueFromInput() : _calculator.RunningTotal;
            _calculator.CurrentInput = decVal.ToString(CultureInfo.InvariantCulture);
            Title = "Programmer Mode (Base 10)";
            RefreshDisplay();
            SettingsService.CurrentSettings.LastBaseUsed = "10";
            SettingsService.SaveSettings(SettingsService.CurrentSettings);
        }

        private void Hex_Click(object sender, RoutedEventArgs e)
        {
            double decVal = (_calculator.CurrentInput != "0") ? GetDecimalValueFromInput() : _calculator.RunningTotal;
            _calculator.CurrentInput = _calculator.ConvertToBase(decVal, "16");
            Title = "Programmer Mode (Base 16)";
            RefreshDisplay();
            SettingsService.CurrentSettings.LastBaseUsed = "16";
            SettingsService.SaveSettings(SettingsService.CurrentSettings);
        }


        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            _calculator.Clear();
            RefreshDisplay();
        }

        private void MemoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (_calculator.GetMemoryItems().Count > 0)
            {
                MemoryWindow memWin = new MemoryWindow(_calculator);
                memWin.Owner = this;
                memWin.ShowDialog();
            }
        }

        private void EmptyMemoryButton_Click(object sender, RoutedEventArgs e)
        {
            _calculator.MemoryStore();
            MemoryWindow memWin = new MemoryWindow(_calculator);
            memWin.Owner = this;
            memWin.ShowDialog();
        }
    }
}
