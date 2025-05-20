using System.Text;
using CodeAnalysis.Core;

namespace CodeAnalysis.Reporting;

public class TextReportGenerator : IReportGenerator
{
    public void GenerateReport(IEnumerable<AnalysisIssue> issues, string outputPath)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Code Analysis Report");
        sb.AppendLine("===================");
        sb.AppendLine();
        
        var groupedIssues = issues.GroupBy(i => i.Severity)
            .OrderByDescending(g => g.Key);

        foreach (var group in groupedIssues)
        {
            sb.AppendLine($"{group.Key} Issues:");
            sb.AppendLine(new string('-', 20));
            
            foreach (var issue in group.OrderBy(i => i.Location))
            {
                sb.AppendLine($"[{issue.Code ?? "NO_CODE"}] {issue.Message}");
                sb.AppendLine($"Location: {issue.Location}");
                sb.AppendLine();
            }
        }

        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        File.WriteAllText(outputPath, sb.ToString());
    }
} 