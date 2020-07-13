#tool "nuget:?package=ReportGenerator&version=4.5.8"

var target = Argument("target", "Build");

Task("Build")
    .Does(() =>
{
    var settings = new DotNetCoreBuildSettings { Verbosity = DotNetCoreVerbosity.Minimal };
    DotNetCoreBuild("./OperationResult.sln", settings);
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    var settings = new DotNetCoreTestSettings 
    {
        NoBuild = true,
        Logger = "console;verbosity=normal"
    };
    DotNetCoreTest("./OperationResult.sln", settings);
});

Task("Coverage")
    .IsDependentOn("Build")
    .Does(async () =>
{
    var settings = new DotNetCoreTestSettings 
    {
        NoRestore = true,
        NoBuild = true,
        Logger = "console;verbosity=normal",
        Settings = "coverlet.runsettings"
    };
    DotNetCoreTest("./OperationResult.sln", settings);
    
    ReportGenerator("./TestResults/*/*.xml", "./coverageOutput", new ReportGeneratorSettings  { ReportTypes = new []
    {
        ReportGeneratorReportType.TextSummary,
        ReportGeneratorReportType.Html
    }});

    var summary = System.IO.File.ReadAllText("./coverageOutput/Summary.txt");

    const string patten = @"((Coverable lines|Covered lines|Line coverage): (?<value>\d+(.\d)?)%?)";
    var matches = System.Text.RegularExpressions.Regex.Matches(summary, patten);
    Information($"Coverage: {matches[0].Groups["value"].Value}");
});

RunTarget(target);