using System;
using System.Windows.Data;
using System.Windows.Media;

namespace XSemmel.Schema
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class IssueTypeToColorConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(Brush))
                throw new InvalidOperationException("The target must be a Brush");

            switch ((ValidationIssue.Type)value)
            {
                case ValidationIssue.Type.Error:
                    return Brushes.Red;
                case ValidationIssue.Type.Warning:
                    return Brushes.Yellow;
                case ValidationIssue.Type.Information:
                    return Brushes.CornflowerBlue;
                default:
                    return Brushes.White;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

    }
}
