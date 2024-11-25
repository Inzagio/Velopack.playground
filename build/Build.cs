using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.Git;
using Nuke.Common.Tools.NerdbankGitVersioning;
using Nuke.Common.Utilities.Collections;

using Octokit;

using Serilog;

using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;
    
    [NerdbankGitVersioning]
    readonly NerdbankGitVersioning NerdbankVersioning;
    AbsolutePath PublishDirectory => RootDirectory / "Publish";
    AbsolutePath ApiSourceDirectory => RootDirectory / "src" / "Playground.Api";
    
    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            // Log.Information("NerdbankVersioning = {Value}", NerdbankVersioning.SimpleVersion);
            PublishDirectory.DeleteDirectory();
            DotNetTasks.DotNetClean(s => s
                .SetProject(Solution)
                .SetConfiguration(Configuration));
            
            
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetTasks.DotNetRestore(s => s
                .SetProjectFile(Solution)
                .SetVerbosity(DotNetVerbosity.normal));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetTasks.DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

    Target Publish => _ => _
        .DependsOn(Compile)
        .Executes((() =>
        {
            DotNetTasks.DotNetPublish(s => s
                .SetProject(ApiSourceDirectory)
                .SetConfiguration(Configuration)
                .SetProperty("PublishIISAssets", "false")
                .SetOutput(PublishDirectory)
                .SetVerbosity(DotNetVerbosity.normal)
                .SetFramework("net9.0")
                .SetRuntime("win-x64")
                .EnableNoRestore());
        }));
}
