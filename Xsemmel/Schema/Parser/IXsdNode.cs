using System.Collections.Generic;
using System.Windows;
using System.Xml;

namespace XSemmel.Schema.Parser 
{

    public interface IXsdNode
    {

	    ICollection<IXsdNode> GetChildren();

	    IList<IXsdNode> GetAll(IList<IXsdNode> all);

        UIElement GetPaintComponent(XsdVisualizer.PaintOptions po, int fontSize);

        XmlNode XmlNode { get; }
    }

}

