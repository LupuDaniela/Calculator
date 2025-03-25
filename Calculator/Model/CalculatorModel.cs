using Calculator.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Calculator.Model
{
    public class CalculatorModel
    {
        private List<double> _memoryStack = new List<double>();

        public string CurrentInput { get; set; } = "0";
        public string Expression { get; private set; } = "";

        private double _runningTotal = 0;
        private string _currentOperator = "";

        private bool _isNewOperation = true;
        private bool _hasCalculated = false;
        private bool _isFirstOperation = true;

        public bool IsDigitGroupingEnabled { get; set; } = false;
        public bool RespectOperatorPrecedence { get; set; } = false;
        public bool HasCalculated { get; private set; } = false;
        public double RunningTotal => _runningTotal;

        public void AppendNumber(string number)
        {
            if (CurrentInput == "Error")
            {
                Clear();
            }
            if (_hasCalculated)
            {
                Clear();
                _hasCalculated = false;
            }
            if (number == ".")
            {
                if (CurrentInput.Contains(".")) return;
                CurrentInput += ".";
                _isNewOperation = false;
                return;
            }
            if (CurrentInput == "0")
                CurrentInput = number;
            else
                CurrentInput += number;
            _isNewOperation = false;
        }

        public void SetOperator(string op)
        {
            if (CurrentInput == "Error")
            {
                Clear();
                return;
            }

            double currentValue = ParseCurrentInput();
            if (double.IsNaN(currentValue))
            {
                CurrentInput = "Error";
                return;
            }

            if (!_isNewOperation && !string.IsNullOrEmpty(_currentOperator))
            {
                switch (_currentOperator)
                {
                    case "+": _runningTotal += currentValue; break;
                    case "−": _runningTotal -= currentValue; break;
                    case "×": _runningTotal *= currentValue; break;
                    case "÷":
                        if (currentValue == 0)
                        {
                            CurrentInput = "Error";
                            return;
                        }
                        _runningTotal /= currentValue;
                        break;
                }

                Expression = _runningTotal.ToString(CultureInfo.InvariantCulture) + " " + op;
            }
            else if (_isFirstOperation)
            {
                _runningTotal = currentValue;
                Expression = CurrentInput + " " + op;
                _isFirstOperation = false;
            }
            else
            {
                if (Expression.Length > 0)
                    Expression = Expression.Substring(0, Expression.Length - 1) + op;
            }

            _currentOperator = op;
            CurrentInput = "0";
            _isNewOperation = true;
        }

        public string Calculate()
        {
            if (CurrentInput == "Error")
            {
                Clear();
                return "Error";
            }

            double currentValue = ParseCurrentInput();
            if (double.IsNaN(currentValue))
            {
                CurrentInput = "Error";
                return "Error";
            }

            if (!_isNewOperation && !string.IsNullOrEmpty(_currentOperator))
            {
                if (!HasCalculated)
                {
                    Expression += " " + currentValue.ToString(CultureInfo.InvariantCulture) + " = ";
                }

                switch (_currentOperator)
                {
                    case "+": _runningTotal += currentValue; break;
                    case "−": _runningTotal -= currentValue; break;
                    case "×": _runningTotal *= currentValue; break;
                    case "÷":
                        if (currentValue == 0)
                        {
                            CurrentInput = "Error";
                            return "Error";
                        }
                        _runningTotal /= currentValue;
                        break;
                }

                bool isProgrammerMode = SettingsService.CurrentSettings?.IsProgrammerMode ?? false;
                string currentBase = SettingsService.CurrentSettings?.LastBaseUsed ?? "10";
                if (isProgrammerMode && currentBase != "10")
                {
                    CurrentInput = ConvertToBase(_runningTotal, currentBase);
                }
                else
                {
                    CurrentInput = _runningTotal.ToString(CultureInfo.InvariantCulture);
                }

                HasCalculated = true;
                _currentOperator = "";
                return GetDisplayText();
            }
            else
            {
                if (!HasCalculated)
                    Expression += " " + CurrentInput + " = ";
                HasCalculated = true;
                return GetDisplayText();
            }
        }

        private double ParseCurrentInput()
        {
            bool isProgrammerMode = SettingsService.CurrentSettings?.IsProgrammerMode ?? false;
            string currentBase = SettingsService.CurrentSettings?.LastBaseUsed ?? "10";

            double value = 0;
            if (isProgrammerMode && currentBase != "10")
            {
                try
                {
                    long intVal = Convert.ToInt64(CurrentInput, int.Parse(currentBase));
                    value = intVal;
                }
                catch
                {
                    return double.NaN;
                }
            }
            else
            {
                if (!double.TryParse(CurrentInput, out value))
                    return double.NaN;
            }
            return value;
        }

        public string ApplyPercentage()
        {
            if (CurrentInput == "Error")
            {
                Clear();
                return "Error";
            }
            if (!PerformPendingOperationIfNeeded())
                return "Error";
            if (!double.TryParse(CurrentInput, out double value))
            {
                CurrentInput = "Error";
                return "Error";
            }
            if (string.IsNullOrEmpty(_currentOperator))
            {
                value /= 100;
            }
            else
            {
                switch (_currentOperator)
                {
                    case "+":
                    case "−":
                        value = _runningTotal * (value / 100);
                        break;
                    case "×":
                    case "÷":
                        value /= 100;
                        break;
                }
            }
            CurrentInput = value.ToString(CultureInfo.InvariantCulture);
            return GetDisplayText();
        }

        public void Clear()
        {
            CurrentInput = "0";
            Expression = "";
            _runningTotal = 0;
            _currentOperator = "";
            _isNewOperation = true;
            HasCalculated = false;
            _isFirstOperation = true;
        }

        public void ClearEntry()
        {
            CurrentInput = "0";
        }

        public void Backspace()
        {
            if (CurrentInput == "Error")
            {
                Clear();
                return;
            }
            if (CurrentInput.Length > 1)
                CurrentInput = CurrentInput.Substring(0, CurrentInput.Length - 1);
            else
                CurrentInput = "0";
        }

        public string Square()
        {
            if (CurrentInput == "Error")
            {
                Clear();
                return "Error";
            }
            if (!PerformPendingOperationIfNeeded())
                return "Error";
            if (!double.TryParse(CurrentInput, out double val))
            {
                CurrentInput = "Error";
                return "Error";
            }
            double result = val * val;
            Expression = $"{CurrentInput}² = ";
            _hasCalculated = true;
            _runningTotal = result;
            CurrentInput = result.ToString(CultureInfo.InvariantCulture);
            return GetDisplayText();
        }

        public string SquareRoot()
        {
            if (CurrentInput == "Error")
            {
                Clear();
                return "Error";
            }
            if (!PerformPendingOperationIfNeeded())
                return "Error";
            if (!double.TryParse(CurrentInput, out double val))
            {
                CurrentInput = "Error";
                return "Error";
            }
            if (val < 0)
            {
                CurrentInput = "Error";
                return "Error";
            }
            double result = Math.Sqrt(val);
            Expression = $"√({CurrentInput}) = ";
            _hasCalculated = true;
            _runningTotal = result;
            CurrentInput = result.ToString(CultureInfo.InvariantCulture);
            return GetDisplayText();
        }

        public string Reciprocal()
        {
            if (CurrentInput == "Error")
            {
                Clear();
                return "Error";
            }
            if (!PerformPendingOperationIfNeeded())
                return "Error";
            if (!double.TryParse(CurrentInput, out double val))
            {
                CurrentInput = "Error";
                return "Error";
            }
            if (val == 0)
            {
                CurrentInput = "Error";
                return "Error";
            }
            double result = 1 / val;
            Expression = $"1/({CurrentInput}) = ";
            _hasCalculated = true;
            _runningTotal = result;
            CurrentInput = result.ToString(CultureInfo.InvariantCulture);
            return GetDisplayText();
        }

        public string ToggleSign()
        {
            if (CurrentInput == "Error")
            {
                Clear();
                return "Error";
            }
            if (!PerformPendingOperationIfNeeded())
                return "Error";
            if (!double.TryParse(CurrentInput, out double val))
            {
                CurrentInput = "Error";
                return "Error";
            }
            val = -val;
            CurrentInput = val.ToString(CultureInfo.InvariantCulture);
            return GetDisplayText();
        }

        private bool PerformPendingOperationIfNeeded()
        {
            if (!RespectOperatorPrecedence)
                return true;
            if (_isNewOperation || string.IsNullOrEmpty(_currentOperator))
                return true;
            if (!double.TryParse(CurrentInput, out double val))
            {
                CurrentInput = "Error";
                return false;
            }
            switch (_currentOperator)
            {
                case "+": _runningTotal += val; break;
                case "−": _runningTotal -= val; break;
                case "×": _runningTotal *= val; break;
                case "÷":
                    if (val == 0)
                    {
                        CurrentInput = "Error";
                        return false;
                    }
                    _runningTotal /= val;
                    break;
            }
            CurrentInput = _runningTotal.ToString(CultureInfo.InvariantCulture);
            Expression += $" {val} = {_runningTotal.ToString(CultureInfo.InvariantCulture)}";
            _currentOperator = "";
            _isNewOperation = true;
            return true;
        }

        public string GetDisplayText()
        {
            if (string.IsNullOrEmpty(CurrentInput) || CurrentInput == "Error")
                return "0";

            if (!IsDigitGroupingEnabled)
                return CurrentInput;

            if (CurrentInput.Contains("."))
            {
                var parts = CurrentInput.Split(new char[] { '.' }, 2);
                string intPart = parts[0];
                string fracPart = parts.Length > 1 ? parts[1] : "";

                string formattedIntPart;
                if (string.IsNullOrEmpty(intPart))
                    formattedIntPart = "0";
                else
                    formattedIntPart = double.Parse(intPart).ToString("N0", CultureInfo.CurrentCulture);

                return formattedIntPart + "." + fracPart;
            }
            else
            {
                return double.Parse(CurrentInput).ToString("N0", CultureInfo.CurrentCulture);
            }
        }


        public void MemoryStore()
        {
            if (double.TryParse(CurrentInput, out double val))
                _memoryStack.Add(val);
        }

        public void MemoryClear() => _memoryStack.Clear();

        public void MemoryRecall()
        {
            if (_memoryStack.Count > 0)
            {
                double lastValue = _memoryStack[_memoryStack.Count - 1];
                CurrentInput = lastValue.ToString(CultureInfo.InvariantCulture);
                _isNewOperation = false;
            }
        }

        public void MemoryAdd()
        {
            if (_memoryStack.Count > 0 && double.TryParse(CurrentInput, out double val))
                _memoryStack[_memoryStack.Count - 1] += val;
            else
                MemoryStore();
        }

        public void MemorySubtract()
        {
            if (_memoryStack.Count > 0 && double.TryParse(CurrentInput, out double val))
                _memoryStack[_memoryStack.Count - 1] -= val;
            else
                MemoryStore();
        }

        public List<double> GetMemoryItems()
        {
            var items = _memoryStack.ToList();
            items.Reverse();
            return items;
        }

        public void RemoveMemoryItem(double value)
        {
            for (int i = 0; i < _memoryStack.Count; i++)
            {
                if (Math.Abs(_memoryStack[i] - value) < 0.000001)
                {
                    _memoryStack.RemoveAt(i);
                    break;
                }
            }
        }


        public string ConvertToBase(double value, string baseVal)
        {
            if (baseVal == "10") return value.ToString(CultureInfo.InvariantCulture);
            int b = int.Parse(baseVal);
            if (b < 2 || b > 16) return value.ToString(CultureInfo.InvariantCulture);
            long longValue = (long)value;
            const string digits = "0123456789ABCDEF";
            if (longValue == 0) return "0";
            string result = "";
            while (longValue > 0)
            {
                int remainder = (int)(longValue % b);
                result = digits[remainder] + result;
                longValue /= b;
            }
            return result;
        }
    }
}
