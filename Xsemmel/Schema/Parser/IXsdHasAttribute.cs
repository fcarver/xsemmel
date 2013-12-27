using System;
using System.Collections.Generic;

namespace XSemmel.Schema.Parser 
{

    public interface IXsdHasAttribute
    {

        void AddAtts(XsdAttribute attr);

        ICollection<XsdAttribute> GetAttributes();

    }
}
