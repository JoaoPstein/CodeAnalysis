using CodeAnalysis.Core;

namespace CodeAnalysis.Reporting;

public interface IReportGenerator
{
    void GenerateReport(IEnumerable<AnalysisIssue> issues, string outputPath);
} 