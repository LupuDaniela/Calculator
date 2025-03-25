using Calculator.Commands;
using Calculator.Model;
using Calculator.Services;
using System;
using System.Windows;
using System.Windows.Input;

namespace Calculator.View
{
    public partial class MainWindow : Window, ICalculatorOwner
    {
        private Window _sidepanelWindow;
        private bool _isSidepanelOpen = false;
        private CalculatorModel _calculator;
        private InputHandler _inputHandler;
        private ClipboardService _clipboardService;

        public MainWindow()
        {
            InitializeComponent();

            _calculator = new CalculatorModel();
            _inputHandler = new InputHandler(EquationTextBlock, ResultTextBlock, _calculator);
            _clipboardService = new ClipboardService();

            var settings = SettingsService.CurrentSettings;
            _calculator.IsDigitGroupingEnabled = settings.IsDigitGroupingEnabled;
            _calculator.RespectOperatorPrecedence = settings.RespectOperatorPrecedence;

            EquationTextBlock.Text = "";
            ResultTextBlock.Text = "0";

            RefreshDisplay();
            InitializeSidePanel();

            if (MenuButton != null)
            {
                MenuButton.Click += MenuButton_Click;
            }

            AttachButtonHandlers();

            this.LocationChanged += MainWindow_LocationChanged;
            this.KeyDown += MainWindow_KeyDown;

            if (MemoryButton != null)
            {
                MemoryButton.Click += MemoryButton_Click;
            }
        }
        public CalculatorModel CalculatorModel => _calculator;

        public void RefreshDisplay()
        {
            if (string.IsNullOrEmpty(_calculator.Expression))
            {
                EquationTextBlock.Text = "";
                ResultTextBlock.Text = "0";
            }
            else
            {
                EquationTextBlock.Text = $"{_calculator.Expression} =";
                ResultTextBlock.Text = _calculator.GetDisplayText();
            }
        }

        public void SetSidePanelState(bool isOpen)
        {
            _isSidepanelOpen = isOpen;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            _inputHandler.KeyPressHandler(sender, e);
        }

        private void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            if (_isSidepanelOpen)
            {
                PositionSidePanel();
            }
        }

        private void AttachButtonHandlers()
        {
            foreach (var child in ButtonGrid.Children)
            {
                if (child is System.Windows.Controls.Grid grid)
                {
                    foreach (var gridChild in grid.Children)
                    {
                        if (gridChild is System.Windows.Controls.Button btn && btn != MenuButton)
                        {
                            btn.Click += (s, e) => _inputHandler.ButtonClickHandler(s, e);
                        }
                    }
                }
                else if (child is System.Windows.Controls.Button button && button != MenuButton)
                {
                    button.Click += (s, e) => _inputHandler.ButtonClickHandler(s, e);
                }
            }
        }

        private void InitializeSidePanel()
        {
            _sidepanelWindow = new SidePanel(this)
            {
                Width = 300, 
                Height = 800,
                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.NoResize,
                ShowInTaskbar = false
            };

            this.Closed += (s, e) => _sidepanelWindow.Close();
            this.ContentRendered += (s, e) => _sidepanelWindow.Owner = this;
        }

        private void PositionSidePanel()
        {
            _sidepanelWindow.Left = this.Left - _sidepanelWindow.Width;
            _sidepanelWindow.Top = this.Top;
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isSidepanelOpen)
            {
                _sidepanelWindow.Hide();
            }
            else
            {
                PositionSidePanel();
                _sidepanelWindow.Show();
            }
            _isSidepanelOpen = !_isSidepanelOpen;
        }

        private void StandardCalculation_Click(object sender, RoutedEventArgs e)
        {
            _calculator.RespectOperatorPrecedence = SettingsService.CurrentSettings.RespectOperatorPrecedence;
            EquationTextBlock.Text = $"{_calculator.Expression} =";
            ResultTextBlock.Text = _calculator.Calculate();
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
