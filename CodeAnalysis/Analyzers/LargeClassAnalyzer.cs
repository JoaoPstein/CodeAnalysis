using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CodeAnalysis.Core;

namespace CodeAnalysis.Analyzers;

public class LargeClassAnalyzer : BaseAnalyzer
{
    private const int DefaultMaxMembers = 20;
    private readonly int _maxMembers;

    public LargeClassAnalyzer(int? maxMembers = null)
    {
        _maxMembers = maxMembers ?? DefaultMaxMembers;
    }

    public override string Name => "Large Class Analyzer";
    public override string Description => $"Detects classes with more than {_maxMembers} members, which might indicate a violation of Single Responsibility Principle.";

    public override IEnumerable<AnalysisIssue> Analyze(SyntaxNode root)
    {
        var issues = new List<AnalysisIssue>();
        var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

        foreach (var classDeclaration in classDeclarations)
        {
            if (classDeclaration.Members.Count > _maxMembers)
            {
                issues.Add(new AnalysisIssue(
                    $"Class '{classDeclaration.Identifier.Text}' has {classDeclaration.Members.Count} members (maximum recommended is {_maxMembers})",
                    GetLocationString(classDeclaration),
                    IssueSeverity.Warning,
                    "LC001"
                ));
            }
        }

        return issues;
    }
} 