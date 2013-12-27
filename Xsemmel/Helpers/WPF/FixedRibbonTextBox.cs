using Microsoft.Windows.Controls.Ribbon;

namespace XSemmel.Helpers.WPF
{
    //http://stackoverflow.com/questions/6883475/cannot-set-ribbontextbox-isenable-to-false
    public class FixedRibbonTextBox : RibbonTextBox
    {
        protected override bool IsEnabledCore
        {
            get { return true; }
        }
    }
}
