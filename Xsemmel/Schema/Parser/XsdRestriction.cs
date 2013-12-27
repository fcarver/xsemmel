using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;

namespace XSemmel.Schema.Parser 
{

	public class XsdRestriction : XsdNode
    {

        public XsdRestriction(XmlNode node) : base(node)
	    {
            string attrBase = VisualizerHelper.GetAttr(node, "base");
            if (attrBase != null)
            {
                Base = attrBase;
            }

            string attrId = VisualizerHelper.GetAttr(node, "id");
            if (attrId != null)
            {
                Id = attrId;
            }
        }

        public string Id { get; set; }
        public string Base { get; set; }

        public string MinExclusive { get; set; }
        public string MaxExclusive { get; set; }
        public string MinInclusive { get; set; }
        public string MaxInclusive { get; set; }
        public string Length { get; set; }
        public string MinLength { get; set; }
        public string MaxLength { get; set; }
        public string Pattern { get; set; }
        //        public string TotalDigits { get; set; }
        //        public string FractionDigits { get; set; }

        private readonly IList<string> _enumeration = new List<string>();

        public void AddEnum(string enumeration)
        {
            _enumeration.Add(enumeration);
        }

        public IList<string> Enumeration
        {
            get
            {
                return _enumeration;
            }
        }


        public override UIElement GetPaintComponent(XsdVisualizer.PaintOptions po, int fontSize)
        {
            if (fontSize <= 0) return null;

            StackPanel pnl = new StackPanel {Margin = new Thickness(5), Background = Brushes.BlanchedAlmond};

            if (!string.IsNullOrEmpty(Base))
            {
                pnl.Children.Add(new TextBlock { Text = Base, FontSize = fontSize });
            }
            if (!string.IsNullOrEmpty(Id))
            {
                pnl.Children.Add(new TextBlock { Text = "Id: "+Id, FontSize = fontSize });
            }
            if (!string.IsNullOrEmpty(MinExclusive))
            {
                pnl.Children.Add(new TextBlock { Text = "MinExclusive: " + MinExclusive, FontSize = fontSize });
            }
            if (!string.IsNullOrEmpty(MaxExclusive))
            {
                pnl.Children.Add(new TextBlock { Text = "MaxExclusive: " + MaxExclusive, FontSize = fontSize });
            }
            if (!string.IsNullOrEmpty(MinInclusive))
            {
                pnl.Children.Add(new TextBlock { Text = "MinInclusive: " + MinInclusive, FontSize = fontSize });
            }
            if (!string.IsNullOrEmpty(MaxInclusive))
            {
                pnl.Children.Add(new TextBlock { Text = "MaxInclusive: " + MaxInclusive, FontSize = fontSize });
            }
            if (!string.IsNullOrEmpty(MaxExclusive))
            {
                pnl.Children.Add(new TextBlock { Text = "MaxExclusive: " + MaxExclusive, FontSize = fontSize });
            }
            if (!string.IsNullOrEmpty(Length))
            {
                pnl.Children.Add(new TextBlock { Text = "Length: " + Length, FontSize = fontSize });
            }
            if (!string.IsNullOrEmpty(MinLength))
            {
                pnl.Children.Add(new TextBlock { Text = "MinLength: " + MinLength, FontSize = fontSize });
            }
            if (!string.IsNullOrEmpty(MaxLength))
            {
                pnl.Children.Add(new TextBlock { Text = "MaxLength: " + MaxLength, FontSize = fontSize });
            }
            if (!string.IsNullOrEmpty(Pattern))
            {
                pnl.Children.Add(new TextBlock { Text = "Pattern: " + Pattern, FontSize = fontSize });
            }
            foreach (string e in _enumeration)
            {
                pnl.Children.Add(new TextBlock { Text = "Enum: " + e, FontSize = fontSize });
            }

            addMouseEvents(po, pnl, this);

            return pnl;
        }

    }
}