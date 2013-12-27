using System.Windows.Media;

namespace XSemmel.Helpers
{
    public static class ColorHelper
    {

        public static Color Create(byte a, Color color)
        {
            return Color.FromArgb(a, color.R, color.G, color.B);
        }

    }
}
