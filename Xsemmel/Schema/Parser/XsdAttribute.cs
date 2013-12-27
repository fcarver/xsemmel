using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;

namespace XSemmel.Schema.Parser 
{

	public class XsdAttribute : XsdNode, IXsdHasType
    {

        public XsdAttribute(XmlNode node) : base(node)
        {
            string attrName = VisualizerHelper.GetAttr(node, "name");
            if (attrName != null)
            {
                Name = attrName;
            }

            string attrDefault = VisualizerHelper.GetAttr(node, "default");
            if (attrDefault != null)
            {
                Default = attrDefault;
            }

            string attrFixed = VisualizerHelper.GetAttr(node, "fixed");
            if (attrFixed != null)
            {
                Fixed = attrFixed;
            }

            string attrForm = VisualizerHelper.GetAttr(node, "form");
            if (attrForm != null)
            {
                Form = attrForm;
            }

            string attrId = VisualizerHelper.GetAttr(node, "id");
            if (attrId != null)
            {
                Id = attrId;
            }

            string attrRef = VisualizerHelper.GetAttr(node, "ref");
            if (attrRef != null)
            {
                Reference = attrRef;
            }

            string attrType = VisualizerHelper.GetAttr(node, "type");
            if (attrType != null)
            {
                TypeName = attrType;
            }

            string attrUse = VisualizerHelper.GetAttr(node, "use");
            if (attrUse != null)
            {
                Use = attrUse;
            }
            else
            {
                Use = "optional";
            }
        }

	    public string Default { get; set; }
        public string Fixed { get; set; }
        public string Use { get; set; }
        public string Form { get; set; }
        public string TypeName { get; set; }
        public IXsdNode TypeTarget { get; set; }
        public string Id { get; set; }
        public string Reference { get; set; }

        public override UIElement GetPaintComponent(XsdVisualizer.PaintOptions po, int fontSize)
        {
            if (fontSize <= 0) return null;

            Brush foreground = Brushes.Gray;
            string displayName = Name;
            if ("optional".Equals(Use))
            {
                displayName += " (opt)";
                foreground = Brushes.DarkGray;  //darkGray is lighter than gray
            }

            StackPanel pnl = new StackPanel();


            TextBlock tb = new TextBlock
                               {
                                   Text = displayName,
                                   Foreground = foreground,
                                   FontSize = fontSize,
                               };
            pnl.Children.Add(tb);
            //pnl.Children.Add(GetPaintChildren(po, fontSize - 1));

            addMouseEvents(po, tb, this);

            return pnl;
        }

        private void addMouseEvents(XsdVisualizer.PaintOptions po, UIElement pnl, IXsdNode node)
        {
            pnl.MouseDown += (sender, args) =>
            {
                if (args.ClickCount == 2)
                {
                    if (TypeTarget != null)
                    {
                        po.Visualizer.SetRoot(TypeTarget);
                    }
                }
                args.Handled = true;
            };
            pnl.MouseMove += (sender, args) =>
            {
                po.Visualizer.SetDescription(node);
                args.Handled = true;
            };
        }

    }
}