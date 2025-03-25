using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Calculator.Model;

namespace Calculator.View
{
    public partial class MemoryWindow : Window
    {
        private CalculatorModel _calculator;
        private MainWindow _mainWindow;

        public MemoryWindow(CalculatorModel calculator)
        {
            InitializeComponent();
            _calculator = calculator;

            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainWindow mainWindow)
                {
                    _mainWindow = mainWindow;
                    break;
                }
            }

            LoadMemoryItems();

            //if (_mainWindow != null)
            //{
            //    this.Left = _mainWindow.Left + (_mainWindow.Width - this.Width) / 2;
            //    this.Top = _mainWindow.Top + (_mainWindow.Height - this.Height) / 2;
            //}
        }

        private void LoadMemoryItems()
        {
            List<double> memoryItems = _calculator.GetMemoryItems();
            MemoryList.Items.Clear();

            foreach (var item in memoryItems)
            {
                MemoryList.Items.Add(item.ToString());
            }
        }

        private void ClearMemory_Click(object sender, RoutedEventArgs e)
        {
            _calculator.MemoryClear();
            Close();
        }

        private void DeleteMemoryItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string valueStr)
            {
                if (double.TryParse(valueStr, out double value))
                {
                    _calculator.RemoveMemoryItem(value);
                    LoadMemoryItems();

                    if (_calculator.GetMemoryItems().Count == 0)
                    {
                        Close();
                    }
                }
            }
        }

        private void MemoryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MemoryList.SelectedItem is string selectedValue)
            {
                if (double.TryParse(selectedValue, out double value))
                {
                    _calculator.CurrentInput = value.ToString();
                    if (_mainWindow != null)
                    {
                        _mainWindow.RefreshDisplay();
                    }
                }
            }
        }

        private void MemoryOperation_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string operation = button.Content.ToString();

                switch (operation)
                {
                    case "MC":
                        _calculator.MemoryClear();
                        Close();
                        break;
                    case "M+":
                        _calculator.MemoryAdd();
                        LoadMemoryItems();
                        break;
                    case "M-":
                        _calculator.MemorySubtract();
                        LoadMemoryItems();
                        break;
                }

                if (_mainWindow != null)
                {
                    _mainWindow.RefreshDisplay();
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            if (this.Owner != null)
            {
                this.Left = this.Owner.Left + (this.Owner.Width - this.Width) / 2;
                this.Top = this.Owner.Top + (this.Owner.Height - this.Height) / 2;
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }


    }
}