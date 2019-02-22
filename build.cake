using System.Linq;
// Target - The task you want to start. Runs the Default task if not specified.
var target = Argument("Target", "Default");
var configuration = Argument("Configuration", "Release");

// Set the package version based on an environment variable 
var packageVersion = 
    HasArgument("PackageVersion") ? Argument<string>("PackageVersion") :  
    !string.IsNullOrEmpty(EnvironmentVariable("PackageVersion")) ? EnvironmentVariable("PackageVersion") : "1.0.0";

// The build number to use in the version number of the built NuGet packages.
// There are multiple ways this value can be passed, this is a common pattern.
// 1. If command line parameter parameter passed, use that.
// 2. Otherwise if running on a build server, get it's build number.
// 3. Otherwise if an Environment variable exists, use that.
// 4. Otherwise default the build number to 0.
var buildNumber =
    HasArgument("BuildNumber") ? Argument<int>("BuildNumber") :
    EnvironmentVariable("BuildNumber") != null ? int.Parse(EnvironmentVariable("BuildNumber")) : 1;
 
Information($"Running target {target} in {configuration} configuration, version {packageVersion}");
 
var artifactsDirectory =Directory("./artifacts");

// Deletes the contents of folder that contain anything from a previous build.
Task("Clean")
    .Does(() =>
    {
var directories =new DirectoryPathCollection(PathComparer.Default);
directories.Add(artifactsDirectory);
//Find the directories MSBuild writes temporary files to.
var objDirsGlob ="**/obj";
directories.Add(GetDirectories(objDirsGlob));
var binDirsGlob ="**/bin";
directories.Add(GetDirectories(binDirsGlob));

        CleanDirectories(directories);
    });
 
// Run dotnet restore to restore all package references.
Task("Restore")
    .Does(() =>
    {
        DotNetCoreRestore();
    });
 
// Find all sln files and build them using the build configuration specified as an argument.
 Task("Build")
    .Does(() =>
    {
        DotNetCoreBuild(
            ".",
            new DotNetCoreBuildSettings()
            {
                Configuration = configuration,
                ArgumentCustomization = args => args.Append($"--no-restore /p:Version={packageVersion}"),
            });
    });
 
// Look under a 'Tests' folder and run dotnet test against all of those projects.
// Then drop the XML test results file in the Artifacts folder at the root.
Task("Test")
    .Does(() =>
    {
    var projects = GetFiles("./tests/**/*.csproj").Where(f =>!f.GetDirectory().FullPath.Contains("TestAssemblies"));
foreach(var project in projects)
        {
            Information("Testing project " + project);
            DotNetCoreTest(
                project.ToString(),
                new DotNetCoreTestSettings()
                {
                    Configuration = configuration,
                    NoBuild = true,
                    ArgumentCustomization = args => args.Append($"--no-restore /p:Version={packageVersion}"),
                });
        }
    });
 
// Run dotnet pack to produce NuGet packages from our projects. 
// (Last number in 1.0.0.0). The packages are dropped in the Artifacts directory.
Task("Pack")
    .Does(() =>
    {
        DotNetCorePack(
            ".",
            new DotNetCorePackSettings()
            {
                Configuration = configuration,
                OutputDirectory = artifactsDirectory,
                NoBuild = true,
                ArgumentCustomization = args => args.Append($"/p:PackageVersion={packageVersion}"),
            });
    });

// The default task to run if none is explicitly specified. In this case, we want
// to run everything starting from Clean, all the way up to Pack.
Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("Pack");

 
// Executes the task specified in the target argument.
RunTarget(target);