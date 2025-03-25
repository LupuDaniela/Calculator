using System;
using System.Collections.Generic;
using System.IO;

namespace Calculator.Services
{
    public class CalculatorSettings
    {
        public bool IsProgrammerMode { get; set; } = false;
        public bool RespectOperatorPrecedence { get; set; } = true;

        public bool IsDigitGroupingEnabled { get; set; } = false;
        public string LastBaseUsed { get; set; } = "10";
    }

    public static class SettingsService
    {
        private static string settingsFilePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CalculatorSettings.txt");

        private static CalculatorSettings? _currentSettings;

        public static CalculatorSettings CurrentSettings
        {
            get
            {
                if (_currentSettings == null)
                {
                    _currentSettings = LoadSettings();
                }
                return _currentSettings;
            }
            set
            {
                _currentSettings = value;
                SaveSettings(_currentSettings);
            }
        }

        private static CalculatorSettings LoadSettings()
        {
            CalculatorSettings result = new CalculatorSettings();

            if (File.Exists(settingsFilePath))
            {
                string[] lines = File.ReadAllLines(settingsFilePath);
                foreach (var line in lines)
                {
                    var parts = line.Split('=');
                    if (parts.Length != 2) continue;

                    string key = parts[0].Trim();
                    string val = parts[1].Trim();

                    switch (key)
                    {
                        case "RESPECT_OPERATOR_PRECEDENCE":
                            bool.TryParse(val, out bool precedence);
                            result.RespectOperatorPrecedence = precedence;
                            break;
                        case "IS_DIGIT_GROUPING_ENABLED":
                            bool.TryParse(val, out bool digitGrouping);
                            result.IsDigitGroupingEnabled = digitGrouping;
                            break;
                        case "LAST_BASE_USED":
                            result.LastBaseUsed = val;
                            break;
                        case "IS_PROGRAMMER_MODE":
                            bool.TryParse(val, out bool progMode);
                            result.IsProgrammerMode = progMode;
                            break;
                    }
                }
            }
            return result;
        }


        public static void SaveSettings(CalculatorSettings settings)
        {
            var lines = new List<string>
              {
                $"RESPECT_OPERATOR_PRECEDENCE={settings.RespectOperatorPrecedence}",
                $"IS_DIGIT_GROUPING_ENABLED={settings.IsDigitGroupingEnabled}",
                $"LAST_BASE_USED={settings.LastBaseUsed}",
                $"IS_PROGRAMMER_MODE={settings.IsProgrammerMode}"
              };
            File.WriteAllLines(settingsFilePath, lines);
        }

    }
}
