
namespace XSemmel.Schema.Parser 
{

    public interface IXsdHasType : IXsdNode
    {

	    string TypeName
	    {
	        get;
	    }

        IXsdNode TypeTarget
        {
            get;
            set;
        }

    }

}

