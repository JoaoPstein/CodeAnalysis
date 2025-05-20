using Microsoft.CodeAnalysis;

namespace CodeAnalysis.Core;

public abstract class BaseAnalyzer : ICodeAnalyzer
{
    public abstract string Name { get; }
    public abstract string Description { get; }

    public abstract IEnumerable<AnalysisIssue> Analyze(SyntaxNode root);

    protected string GetLocationString(SyntaxNode node)
    {
        var location = node.GetLocation();
        var lineSpan = location.GetLineSpan();
        return $"Line {lineSpan.StartLinePosition.Line + 1}, Column {lineSpan.StartLinePosition.Character + 1}";
    }

    protected IEnumerable<AnalysisIssue> NoIssues => Enumerable.Empty<AnalysisIssue>();
} 