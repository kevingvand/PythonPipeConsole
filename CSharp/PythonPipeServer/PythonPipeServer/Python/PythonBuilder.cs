using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythonPipeServer.Python
{
    public class PythonBuilder
    {
        public string Script { get; set; }

        public PythonBuilder(string executingPath = null)
        {
            if (executingPath != null)
            {
                AddImports("sys");
                AppendPath(executingPath);
            }
        }

        public void AddImports(params string[] packages)
        {
            AddLine($"import {string.Join(", ", packages)}");
        }

        public void AppendPath(string path)
        {
            AddLine($"sys.path.append('{GetFormattedPath(path)}')");
        }

        public void DefineVariable(string variableName, string expression, int indent = 0)
        {
            AddLine($"{variableName} = {expression}", indent);
        }

        public void DefineStringVariable(string variableName, string value, int indent = 0)
        {
            DefineVariable(variableName, $@"'{value}'", indent);
        }

        public void PrintString(string text, int indent = 0) => PrintExpression($"'{text}'", indent);

        public void PrintExpression(string expression, int indent = 0)
        {
            AddLine($"print({expression})");
        }

        public void AddLine(string line, int indent = 0)
        {
            for (int i = 0; i < indent; i++)
                Script += "\t";

            Script += $"{line}{Environment.NewLine}";
        }

        public void PrependLine(string line, int indent = 0)
        {
            var formattedLine = string.Empty;
            for (int i = 0; i < indent; i++)
                formattedLine += "\t";
            formattedLine += $"{line}{Environment.NewLine}";

            Script = formattedLine + Script;
        }

        public string GetFormattedPath(string path)
        {
            return Path.GetFullPath(path).Replace('\\', '/');
        }
    }
}
