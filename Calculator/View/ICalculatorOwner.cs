using Calculator.Model;

namespace Calculator.View
{
    public interface ICalculatorOwner
    {
        CalculatorModel CalculatorModel { get; }
        void RefreshDisplay();

        void SetSidePanelState(bool isOpen);
    }
}
