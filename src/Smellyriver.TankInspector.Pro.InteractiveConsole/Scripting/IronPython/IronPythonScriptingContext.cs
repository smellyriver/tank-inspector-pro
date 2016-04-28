using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Smellyriver.TankInspector.Common;

namespace Smellyriver.TankInspector.Pro.InteractiveConsole.Scripting.IronPython
{
    /// <summary>
    /// Represents a Scripting Context for IronPython.
    /// </summary>
    public class IronPythonScriptingContext : AbstractScriptingContext, IIronPythonScriptingContext
    {
        private readonly ScriptEngine _pythonEngine;
        private readonly ScriptScope _scriptScope;

        private StringBuilder _sourceBlockBuffer;

        public ICollection<string> ExternalScriptPaths
        {
            get { return _pythonEngine.GetSearchPaths(); }
            set { _pythonEngine.SetSearchPaths(value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IronPythonScriptingContext"/> class.
        /// </summary>
        public IronPythonScriptingContext()
            : base(">>> ")
        {
            _pythonEngine = Python.CreateEngine();
            _pythonEngine.Runtime.IO.SetOutput(new ScriptOutputWriterStream(Output), Encoding.Unicode);
            _scriptScope = _pythonEngine.CreateScope();
            _sourceBlockBuffer = new StringBuilder();
        }

        /// <summary>
        /// Pushes a variable into the current scripting context as a global variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="variable">The variable.</param>
        public void InjectGlobalVariable(string name, object variable)
        {
            _scriptScope.SetVariable(name, variable);
        }

        /// <summary>
        /// Extracts a script file from an embedded resource in an assembly and loads it into the current scripting context.
        /// </summary>
        /// <param name="containingAssembly">The assembly containing the script file.</param>
        /// <param name="scriptFile">The script file, specified as a file name, for example: <c>Scripts/Foo.py</c>.</param>
        public void LoadEmbeddedScript(Assembly containingAssembly, string scriptFile)
        {
            var resourceNames = containingAssembly.GetManifestResourceNames();
            var bestMatch = resourceNames.Where(name => name.EndsWith(scriptFile.Replace('\\', '.').Replace('/', '.'), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            if (bestMatch == null)
            {
                throw new Exception(string.Format("Script file ending with '{0}' could not be found in assembly '{1}'. Ensure that the file is marked as an Embedded Resource.", scriptFile, containingAssembly.FullName));
            }

            var resourceStream = containingAssembly.GetManifestResourceStream(bestMatch);
            if (resourceStream == null)
            {
                throw new Exception(string.Format("Script file '{0}' was found in assembly '{1}', but the resource stream was null.", bestMatch, containingAssembly.FullName));
            }

            using (Diagnostics.PotentialExceptionRegion)
            {
                using (var reader = new StreamReader(resourceStream))
                {
                    _pythonEngine.Execute(reader.ReadToEnd(), _scriptScope);
                }
            }
        }

        private bool IsIncompleteCode(string code, string lastLine, ScriptCodeParseResult parseResult)
        {
            if (lastLine.EndsWith("\\"))    // todo: handle backslash in comments
                return true;

            if (string.IsNullOrEmpty(lastLine))
                return false;

            return parseResult == ScriptCodeParseResult.IncompleteStatement;
        }

        private void SetNormalPrompt()
        {
            this.Prompt = ">>> ";
        }

        private void SetContinuePrompt()
        {
            this.Prompt = "... ";
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="commandLine">The command line.</param>
        public override void ExecuteCommand(string commandLine)
        {
            Output.WriteLine(Prompt + commandLine);

            _sourceBlockBuffer.AppendLine(commandLine);
            var code = _sourceBlockBuffer.ToString();

            ScriptCodeParseResult parseResult;

            try
            {
                parseResult = _pythonEngine.CreateScriptSourceFromString(code, SourceCodeKind.InteractiveCode).GetCodeProperties();
            }
            catch(Exception ex)
            {
                using (Output.ChangeForegroundColor(ConsoleColor.Red))
                {
                    Output.EnsureNewLine();
                    Output.WriteLine(ex.Message);
                }
                _sourceBlockBuffer.Clear();
                return;
            }

            if (this.IsIncompleteCode(code, commandLine, parseResult))
            {
                this.SetContinuePrompt();
                return;
            }
            else
            {
                this.SetNormalPrompt();
                _sourceBlockBuffer.Clear();

                using (Diagnostics.PotentialExceptionRegion)
                {
                    try
                    {
                        var result = _pythonEngine.Execute(code, _scriptScope);
                        if (result != null)
                            Output.WriteLine(result.ToString());
                    }
                    catch (Exception ex)
                    {
                        using (Output.ChangeForegroundColor(ConsoleColor.Red))
                        {
                            Output.EnsureNewLine();
                            Output.WriteLine(ex.Message);
                        }
                    }
                }
            }
        }
    }
}