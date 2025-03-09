using System.Text;

namespace CodeAnalysis;

public static class Report
{
    public static void ValidateReport(List<string> issues)
    {
        if (issues.Count != 0)
        {
            GenerateReport(issues);
            Console.WriteLine("Relatório gerado com problemas encontrados.");
        }
        else
            Console.WriteLine("Nenhum problema encontrado.");
    }

    private static void GenerateReport(List<string> issues)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Relatório de Análise de Código:");
        sb.AppendLine("==============================");

        foreach (var issue in issues)
            sb.AppendLine($"- {issue}");

        var reportDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Relatorios");
        Directory.CreateDirectory(reportDirectory);
        var reportPath = Path.Combine(reportDirectory, "relatorio.txt");
        File.WriteAllText(reportPath, sb.ToString());
    }
}
// Suporte para múltiplos arquivos ou diretórios: Aceitar diretórios inteiros de código e analisar vários arquivos simultaneamente.
// Integração com CI/CD: Implementar essa ferramenta como parte de pipelines de CI/CD, para garantir que o código esteja sempre sendo analisado antes de ser integrado.
// Opções de personalização: Permitir que o usuário escolha quais tipos de análises deseja realizar.
