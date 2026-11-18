#tool "nuget:?package=ReportGenerator&version=5.6.0"

var target = Argument("target", "Build");
var configuration = Argument("configuration", "Debug");
var solution = "./OperationResult.sln";
var project = "./src/OperationResult/OperationResult.csproj";
var nugetKey = EnvironmentVariable("NUGET_KEY");
var nugetSource = EnvironmentVariable("NUGET_SOURCE");
var nugetVersion = XmlPeek(project, "/Project/PropertyGroup/Version");

Task("Build")
    .Does(() =>
{
    DotNetBuild(solution, new DotNetBuildSettings
    {
        Verbosity = DotNetVerbosity.Minimal,
        Configuration = configuration
    });
});

Task("UnitTest")
    .IsDependentOn("Build")
    .Does(() =>
{
    DotNetTest(solution, new DotNetTestSettings
    {
        Configuration = configuration,
        NoRestore = true,
        NoBuild = true,
        Settings = "coverlet.runsettings"
    });
});

Task("Coverage")
    .IsDependentOn("UnitTest")
    .Does(() =>
{
    GlobPattern reports = "./TestResults/*/*.xml";
    ReportGenerator(reports, "./coverageOutput", new ReportGeneratorSettings  { ReportTypes = new []
    {
        ReportGeneratorReportType.TextSummary,
        ReportGeneratorReportType.Html
    }});

    var summary = System.IO.File.ReadAllText("./coverageOutput/Summary.txt");

    const string patten = @"((Coverable lines|Covered lines|Line coverage): (?<value>\d+(.\d)?)%?)";
    var matches = System.Text.RegularExpressions.Regex.Matches(summary, patten);
    Information($"Coverage: {matches[0].Groups["value"].Value}");
});

Task("Publish")
    .IsDependentOn("Build")
    .Does(() =>
{
    DotNetNuGetPush($"./src/OperationResult/bin/{configuration}/Divino.OperationResult.{nugetVersion}.nupkg", new DotNetNuGetPushSettings
    {
        Source = nugetSource,
        ApiKey = nugetKey,
        SkipDuplicate = true
    });
});

RunTarget(target);
