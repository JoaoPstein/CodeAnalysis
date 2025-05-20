using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CodeAnalysis.Core;

namespace CodeAnalysis.Analyzers;

public class UnusedVariableAnalyzer : BaseAnalyzer
{
    public override string Name => "Unused Variable Analyzer";
    public override string Description => "Detects variables that are declared but never used in the code.";

    public override IEnumerable<AnalysisIssue> Analyze(SyntaxNode root)
    {
        var issues = new List<AnalysisIssue>();
        var variableDeclarations = root.DescendantNodes().OfType<VariableDeclaratorSyntax>();
        
        foreach (var variable in variableDeclarations)
        {
            var usage = root.DescendantNodes()
                .OfType<IdentifierNameSyntax>()
                .Where(id => id.Identifier.Text == variable.Identifier.Text);

            if (!usage.Any())
            {
                issues.Add(new AnalysisIssue(
                    $"Variable '{variable.Identifier.Text}' is declared but never used",
                    GetLocationString(variable),
                    IssueSeverity.Warning,
                    "UV001"
                ));
            }
        }

        return issues;
    }
} 