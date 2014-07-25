using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using Brushes = System.Windows.Media.Brushes;

namespace XSemmel.Schema.Parser {

    public abstract class XsdNode : IXsdNode
    {

		private readonly HashSet<IXsdNode> _kids = new HashSet<IXsdNode>();

        private readonly IList<string> _annotations= new List<string>();


		public ICollection<IXsdNode> GetChildren() 
        {
			return _kids;
		}

        public IList<string> Annotation 
        {
            get 
            {
                return _annotations;
            } 
        }
        public string Name { get; protected set; }

        /// <summary>
        /// underlying XMLNode
        /// </summary>
        private readonly XmlNode _xmlNode;

        protected XsdNode(XmlNode node)
        {
            _xmlNode = node;
        }

        public XmlNode XmlNode
        {
            get { return _xmlNode; }
        }

        public void AddAnnotation(string s)
        {
            _annotations.Add(s);
        }

		public void AddKids(IXsdNode kid) 
        {
			_kids.Add(kid);
		}

        public IList<IXsdNode> GetAll(IList<IXsdNode> all) 
        {
			foreach (IXsdNode node in _kids) 
            {
				all.Add(node);
				node.GetAll(all);
			}
			return all;
		}

        protected static void addMouseEvents(XsdVisualizer.PaintOptions po, FrameworkElement pnl, IXsdNode node  )
        {
            pnl.MouseDown += (sender, args) =>
            {
                if (args.ClickCount == 2)
                {
                    IXsdNode xsdNode = node;
                    if (xsdNode is IXsdHasRef)
                    {
                        IXsdHasRef refer = (IXsdHasRef)xsdNode;
                        if (refer.RefTarget != null)
                        {
                            po.Visualizer.SetRoot(refer.RefTarget);
                            return;
                        }
                    }

                    po.Visualizer.SetRoot(xsdNode);
                }
                args.Handled = true;
            };
            pnl.MouseMove += (sender, args) =>
                                  {
                                      po.Visualizer.SetDescription(node);
                                      args.Handled = true;
                                  };
            
            ContextMenu mainMenu = new ContextMenu();
            mainMenu.Items.Add(new MenuItem { Header = node.ToString(), IsEnabled = false });
            MenuItem item1 = new MenuItem { Header = "Hide" };
            item1.Click += (e,args) =>
                               {
                                   po.Visualizer.IgnoreNodes.Add(node);
                                   po.Visualizer.Refresh();
                               };
            mainMenu.Items.Add(item1);

            pnl.ContextMenu = mainMenu;
        }

        public virtual UIElement GetPaintComponent(XsdVisualizer.PaintOptions po, int fontSize)
        {
            StackPanel pnl = new StackPanel();
            if (fontSize <= 0) return pnl;

            pnl.Children.Add(GetPaintTitle(po, fontSize));
            pnl.Children.Add(GetPaintChildren(po, fontSize - 1));
 
            addMouseEvents(po, pnl, this);

            return new Border { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1), Child = pnl, Margin = new Thickness(5) };
        }


        protected virtual UIElement GetPaintTitle(XsdVisualizer.PaintOptions po, int fontSize)
        {
            return new TextBlock
                       {
                           Text = ToString(),
                           Background = new LinearGradientBrush(Colors.SkyBlue, Colors.Transparent, 90),
                           FontSize = fontSize,
                           FontWeight = FontWeights.DemiBold
                       };
        }


        protected UIElement GetPaintChildren(XsdVisualizer.PaintOptions po, int fontSize)
        {
            StackPanel pnlKids = new StackPanel();
            pnlKids.Orientation = Orientation.Horizontal;

            foreach (IXsdNode kid in _kids)
            {
                if (po.HideTypes && kid is IXsdIsType)
                {
                    continue;
                }
                if (po.HideImportIncludes && kid is XsdImportInclude)
                {
                    continue;
                }

                bool referenceExpanded = false, typeExpanded = false;
                IXsdNode toPaint = kid;
                if (po.ExpandReferences && kid is IXsdHasRef && ((IXsdHasRef)kid).RefTarget != null)
                {
                    toPaint = ((IXsdHasRef)kid).RefTarget;
                    referenceExpanded = true;
                }
                if (po.ExpandTypes && kid is IXsdHasType && ((IXsdHasType)kid).TypeTarget != null)
                {
                    toPaint = ((IXsdHasType)kid).TypeTarget;
                    typeExpanded = true;
                }
                Debug.Assert(!(typeExpanded && referenceExpanded), "node cannot be type and ref");


                //shade expanded nodes
                if (referenceExpanded || typeExpanded)
                {
                    StackPanel pnlExpanded = new StackPanel {Margin = new Thickness(3)};

                    Color c = referenceExpanded ? Colors.Blue : /*typeExpanded */ Colors.Green;
                    pnlExpanded.Background = new SolidColorBrush(Color.FromArgb(32, c.R, c.G, c.G));

                    UIElement title = ((XsdNode)kid).GetPaintTitle(po, fontSize);

                    if (!po.DontPaint.Contains(toPaint))
                    {
                        var kidComp = toPaint.GetPaintComponent(po, fontSize);

                        if (title != null && kidComp != null)
                        {
                            pnlExpanded.Children.Add(title);
                            pnlExpanded.Children.Add(kidComp);
                        }

                        pnlKids.Children.Add(pnlExpanded);    
                    }
                }
                else
                {
                    if (!po.DontPaint.Contains(toPaint))
                    {
                        var kidComp = toPaint.GetPaintComponent(po, fontSize);
                        if (kidComp != null)
                        {
                            pnlKids.Children.Add(kidComp);
                        }
                    }
                }
            }

            return pnlKids;
        }

        public override string ToString()
        {
            return Name;
        }

    }
}