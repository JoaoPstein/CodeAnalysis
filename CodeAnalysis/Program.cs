using Microsoft.CodeAnalysis;
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
                Console.WriteLine("Forneça o caminho do diretório para análise.");
                return;
            }

            var projetoDiretorio = args[0];

            if (!Directory.Exists(projetoDiretorio))
            {
                Console.WriteLine($"O diretório {projetoDiretorio} não existe.");
                return;
            }

            var arquivosCSharp = Directory.GetFiles(projetoDiretorio, "*.cs", SearchOption.AllDirectories);
            if (arquivosCSharp.Length == 0)
            {
                Console.WriteLine("Nenhum arquivo .cs encontrado no projeto.");
                return;
            }

            var issues = new List<string>(); 
            foreach (var arquivo in arquivosCSharp)
            {
                Console.WriteLine($"Analisando: {arquivo}");
                try
                {
                    var code = File.ReadAllText(arquivo);

                    var syntaxTree = CSharpSyntaxTree.ParseText(code);
                    var root = syntaxTree.GetRoot();

                    MethodsDeclarations(root, issues);

                    ClassDeclaration(root, issues);

                    VariableeDeclaration(root, issues);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao analisar o arquivo {arquivo}: {ex.Message}");
                }
            }

            Console.WriteLine("Análise concluída.");
            Report.ValidateReport(issues); 
        }

        private static void VariableeDeclaration(SyntaxNode root, List<string> issues)
        {
            var variableDeclarations = root.DescendantNodes().OfType<VariableDeclaratorSyntax>();
            issues.AddRange(from variable in variableDeclarations
                let usage = root.DescendantNodes()
                    .OfType<IdentifierNameSyntax>()
                    .Where(id => id.Identifier.Text == variable.Identifier.Text)
                where !usage.Any()
                select
                    $"Variável '{variable.Identifier.Text}' não está sendo utilizada na linha {variable.GetLocation().GetLineSpan().StartLinePosition.Line + 1}");
        }

        private static void ClassDeclaration(SyntaxNode root, List<string> issues)
        {
            var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
            issues.AddRange(from classDeclaration in classDeclarations
                where classDeclaration.Members.Count > 20
                select
                    $"Classe '{classDeclaration.Identifier.Text}' parece ser grande demais na linha {classDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line + 1}");
        }

        private static void MethodsDeclarations(SyntaxNode root, List<string> issues)
        {
            var methodDeclarations = root.DescendantNodes().OfType<MethodDeclarationSyntax>();
            issues.AddRange(methodDeclarations.Select(method =>
                $"Método encontrado: {method.Identifier.Text} na linha {method.GetLocation().GetLineSpan().StartLinePosition.Line + 1}"));
        }
    }
}
