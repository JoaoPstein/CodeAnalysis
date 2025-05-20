using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeAnalysis.Core;

public interface ICodeAnalyzer
{
    string Name { get; }
    string Description { get; }
    IEnumerable<AnalysisIssue> Analyze(SyntaxNode root);
}

public record AnalysisIssue(
    string Message,
    string Location,
    IssueSeverity Severity,
    string? Code = null
);

public enum IssueSeverity
{
    Info,
    Warning,
    Error
} 