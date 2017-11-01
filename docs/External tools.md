Starting with build 01-MAR-2014, external tools can be run directly from Xsemmel.

E.g. if you like to generate C# sourcecode based on a XSD file, you can use a tool like [xsd2code](https://xsd2code.codeplex.com/). To call that from Xsemmel, go to _File_ -> _Options_ -> _External Tools_ and specify as
**File name:** C:\Program Files (x86)\Xsd2Code\Xsd2Code.exe
**Arguments:** {""$(ItemPath)" My.Namespace.$(ItemFilenameWithoutExt) "$(ItemDir)\$(ItemFilenameWithoutExt).cs" /collection Array /xa+ /platform Net35 /sc+ /is+"}
Restart Xsemmel, open a XSD file. Go to _Tools_ and click on _Xsd2Code_




