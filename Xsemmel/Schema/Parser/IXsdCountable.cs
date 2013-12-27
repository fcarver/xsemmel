namespace XSemmel.Schema.Parser 
{

	public interface IXsdCountable 
    {

        string MinOccurs { get; }
        string MaxOccurs { get; }

	}
}