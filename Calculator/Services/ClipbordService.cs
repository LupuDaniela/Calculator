using System.Windows;

namespace Calculator.Services
{
    public class ClipboardService
    {
        public void SetText(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                Clipboard.SetText(text);
            }
        }

        public string GetText()
        {
            return Clipboard.ContainsText() ? Clipboard.GetText() : string.Empty;
        }
    }
}
