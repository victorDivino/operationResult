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

RunTarget(target);