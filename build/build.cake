#tool "nuget:?package=NUnit.ConsoleRunner"
#tool "nuget:?package=NUnit.Extension.NUnitV2ResultWriter"
#addin "Cake.Git"
#addin "Cake.FileHelpers"
#addin "nuget:http://nuget.oss-concept.ch/nuget/?package=Opten.Cake"

var target = Argument("target", "Default");

string feedUrl = "https://www.nuget.org/api/v2/package";
string version = null;

var dest = Directory("./artifacts");

// Cleanup

Task("Clean")
	.Does(() =>
{
	if (DirectoryExists(dest))
	{
		CleanDirectory(dest);
		DeleteDirectory(dest, recursive: true);
	}
});

// Versioning

Task("Version")
	.IsDependentOn("Clean") 
	.Does(() =>
{
	if (DirectoryExists(dest) == false)
	{
		CreateDirectory(dest);
	}

	version = GitDescribe("../", false, GitDescribeStrategy.Tags, 0);

	PatchAssemblyInfo("../src/Opten.Web.Infrastructure/Properties/AssemblyInfo.cs", version);
	FileWriteText(dest + File("Opten.Web.Infrastructure.variables.txt"), "version=" + version);
});

// Building

Task("Restore-NuGet-Packages")
	.IsDependentOn("Version") 
	.Does(() =>
{ 
	NuGetRestore("../Opten.Web.Infrastructure.sln", new NuGetRestoreSettings {
		NoCache = true
	});
});

Task("Build") 
	.IsDependentOn("Restore-NuGet-Packages") 
	.Does(() =>
{
	MSBuild("../src/Opten.Web.Infrastructure/Opten.Web.Infrastructure.csproj", settings =>
		settings.SetConfiguration("Debug"));

	MSBuild("../src/Opten.Web.Infrastructure/Opten.Web.Infrastructure.csproj", settings =>
		settings.SetConfiguration("Release"));

	MSBuild("../tests/Opten.Web.Infrastructure.Test/Opten.Web.Infrastructure.Test.csproj", settings =>
		settings.SetConfiguration("Release"));
});

Task("Run-Unit-Tests")
	.IsDependentOn("Build")
	.Does(() =>
{
	var results = dest + Directory("tests");

	if (DirectoryExists(results) == false)
	{
		CreateDirectory(results);
	}

	//TODO: Why not csproj?
	NUnit3("../tests/Opten.Web.Infrastructure.Test/bin/Release/Opten.Web.Infrastructure.Test.dll", new NUnit3Settings {
		Results = new[] {
			new NUnit3Result {
				FileName = results + File("Opten.Web.Infrastructure.Test.xml"),
				Format = "nunit2" // Wait until Bamboo 5.14 is out to support NUnit 3!
			}
		},
		Configuration = "Release"
	});
});

Task("Pack")
	.IsDependentOn("Run-Unit-Tests")
	.Does(() =>
{
	NuGetPackWithDependencies("./Opten.Web.Infrastructure.nuspec", new NuGetPackSettings {
		Version = version,
		BasePath = "../",
		OutputDirectory = dest
	}, feedUrl);
});


// Deploying

Task("Deploy")
	.Does(() =>
{
	// This is from the Bamboo's Script Environment variables
	string packageId = "Opten.Web.Infrastructure";

	// Get the Version from the .txt file
	string version = EnvironmentVariable("bamboo_inject_" + packageId.Replace(".", "_") + "_version");

	if(string.IsNullOrWhiteSpace(version))
	{
		throw new Exception("Version is missing for " + packageId + ".");
	}

	// Get the path to the package
	var package = File(packageId + "." + version + ".nupkg");
			
	// Push the package
	NuGetPush(package, new NuGetPushSettings {
		Source = feedUrl,
		ApiKey = EnvironmentVariable("NUGET_API_KEY")
	});

	// Notifications
	Slack(new SlackSettings {
		ProjectName = packageId
	});
});

Task("Default")
	.IsDependentOn("Pack");

RunTarget(target);