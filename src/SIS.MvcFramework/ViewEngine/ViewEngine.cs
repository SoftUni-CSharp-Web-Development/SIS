using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace SIS.MvcFramework.ViewEngine
{
    public class ViewEngine : IViewEngine
    {
        public string GetHtml<T>(string viewName, string viewCode, T model)
        {
            var viewTypeName = viewName + "View";
            var cSharpMethodBody = this.GenerateCSharpMethodBody(viewCode);
            string typeNamespace = typeof(T).Namespace;
            string typeFullName = typeof(T).FullName.Replace("+", ".");

            var viewCodeAsCSharpCode = File.ReadAllText("../../../../../SIS.MvcFramework/ViewEngine/CSharpTemplate.txt");

            viewCodeAsCSharpCode = viewCodeAsCSharpCode.Replace("@ViewTypeName", viewTypeName)
                                .Replace("@CSharpMethodBody", cSharpMethodBody)
                                .Replace("@Namespace", typeNamespace)
                                .Replace("@TypeFullName", typeFullName);

            var instanceOfViewClass = this.GetInstance(viewCodeAsCSharpCode, "MyAppViews."
                                                                    + viewTypeName, typeof(T)) as IView<T>;
            var html = instanceOfViewClass.GetHtml(model);
            return html;
        }

        private object GetInstance(string cSharpCode, string typeName, Type viewModelType)
        {
            var compilation = CSharpCompilation.Create(Path.GetRandomFileName() + ".dll")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("netstandard")).Location))
                .AddReferences(MetadataReference.CreateFromFile(typeof(IView<>).GetTypeInfo().Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(viewModelType.Assembly.Location));

            var netStandardReferences = Assembly.Load(new AssemblyName("netstandard")).GetReferencedAssemblies();
            foreach (var netStandardReference in netStandardReferences)
            {
                compilation = compilation.AddReferences(MetadataReference.CreateFromFile(Assembly.Load(netStandardReference).Location));
            }

            compilation = compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(cSharpCode));

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);
                if (!result.Success)
                {
                    var errors = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (var diagnostic in errors)
                    {
                        Console.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }

                    throw new InvalidOperationException("Emit compilation unsuccessful!");
                }

                var assembly = Assembly.Load(ms.ToArray());
                var viewType = assembly.GetType(typeName);
                return Activator.CreateInstance(viewType);
            }
        }

        private string GenerateCSharpMethodBody(string viewCode)
        {
            var lines = viewCode.Split(Environment.NewLine);
            var stringBuilder = new StringBuilder();

            var cSharpStartSymbols = new List<string> { "{", "}", "@for", "@else", "@if", "@var" };

            foreach (var line in lines)
            {
                if (cSharpStartSymbols.Any(x => line.Trim().StartsWith(x)))
                {
                    stringBuilder.AppendLine(line.Replace("@", string.Empty));
                }
                else
                {
                    var htmlLine = line.Replace("\"", "\\\"");
                    while (htmlLine.Contains("@"))
                    {
                        var specialSymbolIndex = htmlLine.IndexOf("@", StringComparison.InvariantCulture);
                        var endOfCode = new Regex(@"[\s&<\\]+").Match(htmlLine, specialSymbolIndex).Index;

                        string expression = null;
                        if (endOfCode == 0 || endOfCode == -1)
                        {
                            expression = htmlLine.Substring(specialSymbolIndex + 1);
                            htmlLine = htmlLine.Substring(0, specialSymbolIndex) + "\" + " + expression + " + \"";
                        }
                        else
                        {
                            expression = htmlLine.Substring(specialSymbolIndex + 1, endOfCode - specialSymbolIndex - 1);
                            htmlLine = htmlLine.Substring(0, specialSymbolIndex) + "\" + " + expression +
                                                    " + \"" + htmlLine.Substring(endOfCode);
                        }
                    }

                    stringBuilder.AppendLine($"html.AppendLine(\"{htmlLine}\");");
                }
            }

            return stringBuilder.ToString();
        }
    }
}