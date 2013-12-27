
namespace XSemmel.Schema.Parser 
{

    public interface IXsdHasRef : IXsdNode
    {

	    string RefName
	    {
	        get;
	    }

        IXsdNode RefTarget
        {
            get;
            set;
        }

    }

}

