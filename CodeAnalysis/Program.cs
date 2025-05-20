using CodeAnalysis.Analyzers;
using CodeAnalysis.Core;
using CodeAnalysis.Reporting;
using Serilog;

namespace CodeAnalysis;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Please provide the path to the project directory for analysis.");
            return;
        }

        var projectPath = args[0];

        // Configure logging
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File("logs/analysis.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try
        {
            // Configure analyzers
            var analyzers = new ICodeAnalyzer[]
            {
                new UnusedVariableAnalyzer(),
                new LargeClassAnalyzer()
                // Add more analyzers here as needed
            };

            // Create analysis engine
            var engine = new AnalysisEngine(Log.Logger, analyzers);

            // Run analysis
            Log.Information("Starting analysis of project: {ProjectPath}", projectPath);
            var issues = await engine.AnalyzeProjectAsync(projectPath);

            // Generate report
            var reportPath = Path.Combine("Reports", $"analysis-report-{DateTime.Now:yyyyMMdd-HHmmss}.txt");
            var reporter = new TextReportGenerator();
            reporter.GenerateReport(issues, reportPath);

            Log.Information("Analysis completed. Report generated at: {ReportPath}", reportPath);

            // Print summary
            var issueCount = issues.Count();
            Console.WriteLine($"\nAnalysis Summary:");
            Console.WriteLine($"Total issues found: {issueCount}");
            
            if (issueCount > 0)
            {
                var severityCounts = issues.GroupBy(i => i.Severity)
                    .Select(g => $"{g.Key}: {g.Count()}");
                Console.WriteLine($"Issues by severity: {string.Join(", ", severityCounts)}");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred during analysis");
            Console.WriteLine("An error occurred during analysis. Check the logs for details.");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
