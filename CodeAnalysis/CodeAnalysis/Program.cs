using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Forneça o caminho do arquivo C# para análise.");
                return;
            }

            var filePath = args[0];

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Arquivo não encontrado: {filePath}");
                return;
            }

            var code = File.ReadAllText(filePath);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);

            var root = syntaxTree.GetRoot();

            var methodDeclarations = root.DescendantNodes().OfType<MethodDeclarationSyntax>();

            var issues = methodDeclarations.Select(method => $"Método encontrado: {method.Identifier.Text} na linha {method.GetLocation().GetLineSpan().StartLinePosition.Line + 1}").ToList();

            var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

            issues.AddRange(from classDeclaration in classDeclarations where classDeclaration.Members.Count > 20 select $"Classe '{classDeclaration.Identifier.Text}' parece ser grande demais na linha {classDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line + 1}");

            var variableDeclarations = root.DescendantNodes().OfType<VariableDeclaratorSyntax>();

            issues.AddRange(from variable in variableDeclarations
                let usage = root.DescendantNodes()
                    .OfType<IdentifierNameSyntax>()
                    .Where(id => id.Identifier.Text == variable.Identifier.Text)
                where !usage.Any()
                select $"Variável '{variable.Identifier.Text}' não está sendo utilizada na linha {variable.GetLocation().GetLineSpan().StartLinePosition.Line + 1}");

           Report.ValidateReport(issues);
        }
    }
}
