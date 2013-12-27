using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CSharp;

namespace XSemmel.XPath
{

    /// <summary>
    /// This class a able to compile C# source files (scripts).
    /// </summary>
    public class ScriptEngine
    {

        /// <summary>
        /// Loads a script out of a string.
        /// </summary>
        /// <param name="code">The code</param>
        /// <param name="references">References needed by the script</param>
        /// <returns>The compiled script</returns>
        public Assembly LoadFromString(string code, params Assembly[] references)
        {
            return loadFromFile(code, SourceType.String, references);
        }

        /// <summary>
        /// Loads a script out of a file.
        /// </summary>
        /// <param name="filename">The file</param>
        /// <param name="references">References needed by the script</param>
        /// <returns>The compiled script</returns>
        /// <remarks>Source: <![CDATA[http://en.how-to.mobi/index.php?sd=de&id=137933]]></remarks>
        public Assembly LoadFromFile(string filename, params Assembly[] references)
        {
            return loadFromFile(filename, SourceType.File, references);
        }

        private enum SourceType
        {
            File,
            String
        }

        private Assembly loadFromFile(string fileOrString, SourceType source, params Assembly[] references)
        {
            CompilerParameters options = new CompilerParameters();
            // we want a Dll (or "Class Library" as its called in .Net)
            options.GenerateExecutable = false;
            // Saves us from deleting the Dll when we are done with it, though 
            // you could set this to false and save start-up time by next time
            // by not having to re-compile
            options.GenerateInMemory = true;
            // And set any others you want, there a quite a few, take some time 
            // to look through them all and decide which fit your application best!
            options.IncludeDebugInformation = true;
            //            options.OutputAssembly = "Ganymed.Server.DeviceFamily.Saturn";

            // Add any references you want the users to be able to access, be 
            // warned that giving them access to some classes can allow
            // harmful code to be written and executed. I recommend that you 
            // write your own Class library that is the only reference it allows
            // thus they can only do the things you want them to.
            // (though things like "System.Xml.dll" can be useful, just need to
            // provide a way users can read a file to pass in to it)
            // This will expose ALL public classes to the "script"
            foreach (var reference in references)
            {
                options.ReferencedAssemblies.Add(reference.Location);
            }

            // This class implements the 'CodeDomProvider' class as its base. 
            // All of the current .Net languages (at least Microsoft ones)
            // come with their own implemtation, thus you can allow the user to
            // use the language of thier choice (though i recommend that
            // you don't allow the use of c++, which is too volatile for
            // scripting use - memory leaks anyone?)

            var providerOptions = new Dictionary<string, string> 
            { 
                { "CompilerVersion", "v3.5" } 
            };
            CSharpCodeProvider csProvider = new CSharpCodeProvider(providerOptions);
            CompilerResults result;
            switch (source)
            {
                case SourceType.File:
                    result = csProvider.CompileAssemblyFromFile(options, fileOrString);
                    break;
                case SourceType.String:
                    result = csProvider.CompileAssemblyFromSource(options, fileOrString);
                    break;
                default:
                    throw new Exception("Unknown SourceType: " + source);
            }

            if (result.Errors.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                if (source == SourceType.String)
                {
                    string[] lines = fileOrString.Split('\n');
                    for (int i = 1; i < lines.Count(); i++)
                    {
                        sb.Append(i).Append(": ").Append(lines[i - 1]).Append('\n');
                    }
                }
                sb.Append('\n');

                foreach (CompilerError error in result.Errors)
                {
                    //CompileError overwrote ToString()
                    if (error.IsWarning)
                    {
                        sb.Append("Warning: ").Append(error.ToString()).Append('\n');
                        //_log.WarningFormat("Warning in script: {0}", error);
                    }
                    else
                    {
                        sb.Append("Error: ").Append(error.ToString()).Append('\n');
                        //_log.ErrorFormat("Error in script: {0}", error);
                    }
                }
                if (result.Errors.HasErrors)
                {
                    throw new Exception("Script has errors:\n" + sb);
                }
            }

            return result.CompiledAssembly;
        }

        public T FromScriptFile<T>(string file)
        {
            string code = File.ReadAllText(file);
            Assembly[] refs = getReferencedAssemblies(code);
            Assembly a = LoadFromString(code, refs);
            return As<T>(a);
        }

        /// <summary>
        /// Searches for T (or a type sub-classing T) in the specified assembly
        /// and calls it's default constructor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assembly"></param>
        /// <returns>Object initializes by the default-constructor</returns>
        public T As<T>(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes()
                .Where(type => type.IsClass)
                .Where(type =>
                       type.Equals(typeof(T))
                    || type.IsSubclassOf(typeof(T))
                    || ((IList<Type>)type.GetInterfaces()).Contains(typeof(T))))
            {
                return (T)Activator.CreateInstance(type);
            }
            throw new Exception("Assembly does not contain type T");
        }

        private Assembly[] getReferencedAssemblies(string _code)
        {
            string exePath = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;

            List<Assembly> refedAss = new List<Assembly>();

            Regex rx = new Regex("// ref (file|gac) (.*);");
            Match match = rx.Match(_code);
            while (match.Success)
            {
                bool isFile = "file".Equals(match.Groups[1].Value);
                Group g = match.Groups[2];

                string assembly = g.Value;

                if (isFile)
                {
                    if (!Path.IsPathRooted(assembly))
                    {
                        assembly = Path.Combine(exePath, assembly);
                    }

                    if (!File.Exists(assembly))
                    {
                        throw new Exception(string.Format("Referenced assembly nor found: {0}", assembly));
                    }

                    refedAss.Add(Assembly.LoadFile(assembly));
                }
                else // from GAC 
                {
                    refedAss.Add(Assembly.LoadWithPartialName(assembly));
                }

                match = match.NextMatch();
            }
            return refedAss.ToArray();
        }
    }
}
