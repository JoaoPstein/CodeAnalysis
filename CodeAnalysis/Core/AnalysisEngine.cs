using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Serilog;

namespace CodeAnalysis.Core;

public class AnalysisEngine
{
    private readonly ILogger _logger;
    private readonly ICollection<ICodeAnalyzer> _analyzers;

    public AnalysisEngine(ILogger logger, ICollection<ICodeAnalyzer> analyzers)
    {
        _logger = logger;
        _analyzers = analyzers;
    }

    public async Task<IEnumerable<AnalysisIssue>> AnalyzeProjectAsync(string projectPath)
    {
        var issues = new List<AnalysisIssue>();
        
        if (!Directory.Exists(projectPath))
        {
            _logger.Error("Project directory {ProjectPath} does not exist", projectPath);
            return issues;
        }

        var csharpFiles = Directory.GetFiles(projectPath, "*.cs", SearchOption.AllDirectories);
        if (csharpFiles.Length == 0)
        {
            _logger.Warning("No C# files found in {ProjectPath}", projectPath);
            return issues;
        }

        foreach (var file in csharpFiles)
        {
            try
            {
                _logger.Information("Analyzing file: {File}", file);
                var fileIssues = await AnalyzeFileAsync(file);
                issues.AddRange(fileIssues);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error analyzing file {File}", file);
            }
        }

        return issues;
    }

    private async Task<IEnumerable<AnalysisIssue>> AnalyzeFileAsync(string filePath)
    {
        var issues = new List<AnalysisIssue>();
        var code = await File.ReadAllTextAsync(filePath);
        
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var root = await syntaxTree.GetRootAsync();

        foreach (var analyzer in _analyzers)
        {
            try
            {
                var analyzerIssues = analyzer.Analyze(root);
                issues.AddRange(analyzerIssues);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in analyzer {AnalyzerName} for file {File}", 
                    analyzer.Name, filePath);
            }
        }

        return issues;
    }
} 