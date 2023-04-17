using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.Docker;
using static Nuke.Common.Tools.Docker.DockerTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using Octokit;
using System.IO.Compression;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Collections.Generic;
using NuGet.Common;
using System.Collections;
using Nuke.Common.CI.GitHubActions;

[GitHubActions(
    "compile",
    GitHubActionsImage.UbuntuLatest,
    On = new[] { GitHubActionsTrigger.Push },
    InvokedTargets = new[] { nameof(Compile) })]
[GitHubActions(
    "deploy",
    GitHubActionsImage.UbuntuLatest,
    On = new[] { GitHubActionsTrigger.WorkflowDispatch },
    ImportSecrets = new[] {nameof(WebAppPassword), nameof(ConnectionString) },
    InvokedTargets = new[] { nameof(Deploy) }, AutoGenerate = false)]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    string DockerPrefix = "WebAPI";
    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath PublishDirectory => RootDirectory / "publish";
    AbsolutePath ArtifactDirectory => RootDirectory / "artifact";

    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    string SQLPort = "1033";
    string SQLPassword = "Sqlserver123$";

    Target RunOrStartSQLServer => _ => _
    .Executes(() =>
    {
        try
        {
            DockerRun(x => x
            .SetName($"{DockerPrefix}-sqlserver")
            .AddEnv("ACCEPT_EULA=Y", $"MSSQL_SA_PASSWORD={SQLPassword}")
            .SetImage("mcr.microsoft.com/mssql/server:2019-CU14-ubuntu-20.04")
            .EnableDetach()
            .SetPublish($"{SQLPort}:1433")
            );
        }
        catch (Exception)
        {
            DockerStart(x => x
            .AddContainers($"{DockerPrefix}-sqlserver")
            );
        }
    });

    string SEQPort = "5342";

    Target RunOrStartSeq => _ => _
        .Executes(() =>
        {
            try
            {
                DockerRun(x => x
                .SetName($"{DockerPrefix}-seq")
                .AddEnv("ACCEPT_EULA=Y")
                .SetRestart("unless-stopped")
                .SetImage("datalust/seq:latest")
                .EnableDetach()
                .SetPublish($"{SEQPort}:80")
                );
            }
            catch (Exception)
            {
                DockerStart(x => x
                .AddContainers($"{DockerPrefix}-seq")
                );
            }
        });

    string RabbitMQUser = "admin";
    string RabbitMQPassword = "Rabbitmq123$";
    string RabbitMQAdminPort = "15671";
    string RabbitMQPort = "5671";

    Target RunOrStartRabbitMQ => _ => _
        .Executes(() =>
        {
            try
            {
                DockerRun(x => x
                .SetName($"{DockerPrefix}-rabbitmq")
                .SetHostname($"{DockerPrefix}-host")
                .AddEnv($"RABBITMQ_DEFAULT_USER={RabbitMQUser}", $"RABBITMQ_DEFAULT_PASS={RabbitMQPassword}")
                .SetImage("rabbitmq:3-management")
                .EnableDetach()
                .AddPublish($"{RabbitMQAdminPort}:15672")
                .AddPublish($"{RabbitMQPort}:5672")
                );
            }
            catch (Exception)
            {
                DockerStart(x => x
                .AddContainers($"{DockerPrefix}-rabbitmq")
                );
            }
        });

    Target StartEnv => _ => _
        .DependsOn(RunOrStartSQLServer)
        .DependsOn(RunOrStartSeq)
        .DependsOn(RunOrStartRabbitMQ)
        .Executes(() =>
        {
            Serilog.Log.Information("Development env started");
        });

    Target StopSQLServer => _ => _
        .Executes(() =>
        {
            DockerStop(x => x
            .AddContainers($"{DockerPrefix}-sqlserver")
            );
        });

    Target StopSeq => _ => _
        .Executes(() =>
        {
            DockerStop(x => x
            .AddContainers($"{DockerPrefix}-seq")
            );
        });

    Target StopRabbitMQ => _ => _
        .Executes(() =>
        {
            DockerStop(x => x
            .AddContainers($"{DockerPrefix}-rabbitmq")
            );
        });

    Target StopEnv => _ => _
        .DependsOn(StopSQLServer)
        .DependsOn(StopSeq)
        .DependsOn(StopRabbitMQ)
        .Executes(() =>
        {
            Serilog.Log.Information("Development env stoped");
        });

    Target RemoveSQLServer => _ => _
        .DependsOn(StopSQLServer)
        .Executes(() =>
        {
            DockerRm(x => x
            .AddContainers($"{DockerPrefix}-sqlserver")
            );
        });

    Target RemoveSeq => _ => _
        .DependsOn(StopSeq)
        .Executes(() =>
        {
            DockerRm(x => x
            .AddContainers($"{DockerPrefix}-seq")
            );
        });

    Target RemoveRabbitMQ => _ => _
        .DependsOn(StopRabbitMQ)
        .Executes(() =>
        {
            DockerRm(x => x
            .AddContainers($"{DockerPrefix}-rabbitmq")
            );
        });

    Target RemoveEnv => _ => _
        .DependsOn(RemoveSQLServer)
        .DependsOn(RemoveSeq)
        .DependsOn(RemoveRabbitMQ)
        .Executes(() =>
        {
            Serilog.Log.Information("Development env removed");
        });

    string MigratorProject = "Migrator";

    string WebAPIProject = "WebAPI";

    string TestsProject = "Tests";

    [Parameter()]
    public string WebAppUser;

    [Parameter()]
    public string WebAppPassword;

    [Parameter]
    public string WebAppName;

    [Parameter]
    public string ConnectionString;

    Target CleanMigrator => _ => _
        .Executes(() =>
        {
            EnsureCleanDirectory(PublishDirectory / MigratorProject);
        });

    Target CompileMigrator => _ => _
        .DependsOn(CleanMigrator)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(SourceDirectory / MigratorProject)
                .SetConfiguration(Configuration));
        });

    Target PublishMigrator => _ => _
        .DependsOn(CompileMigrator)
        .Executes(() =>
        {
            DotNetPublish(s => s
                .SetProject(SourceDirectory / MigratorProject)
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .EnableNoRestore()
                .SetOutput(PublishDirectory / MigratorProject));
        });

    Target RunMigrator => _ => _
        .DependsOn(PublishMigrator)
        .Executes(() =>
        {
            var defaultEnvironmentVariables = Environment.GetEnvironmentVariables()
            .Cast<DictionaryEntry>().ToDictionary(entry=>entry.Key.ToString(), entry=>entry.Value.ToString());

            var environmentVariables = new Dictionary<string, string>(defaultEnvironmentVariables);

            if (!string.IsNullOrEmpty(ConnectionString))
            {
                environmentVariables.Add("DbConnectionString", ConnectionString);
            }

            DotNet(PublishDirectory / MigratorProject / $"{MigratorProject}.dll", environmentVariables: environmentVariables);
        });

    Target Clean => _ => _
        .Executes(() =>
        {
            EnsureCleanDirectory(ArtifactDirectory);
            EnsureCleanDirectory(PublishDirectory / WebAPIProject);
        });

    Target Compile => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(SourceDirectory / WebAPIProject)
                .SetConfiguration(Configuration));
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile(TestsDirectory / TestsProject)
                .SetConfiguration(Configuration));
        });

    Target Publish => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetPublish(s => s
                .SetProject(SourceDirectory / WebAPIProject)
                .SetConfiguration(Configuration)
                .SetOutput(PublishDirectory / WebAPIProject)
                .EnableNoRestore()
                .EnableNoBuild());
        });

    Target Zip => _ => _
        .Produces(ArtifactDirectory / "*.zip")
        .DependsOn(Publish)
        .Executes(() =>
        {
            ZipFile.CreateFromDirectory(PublishDirectory / WebAPIProject, ArtifactDirectory / "deployment.zip");
        });

    Target Deploy => _ => _
        .DependsOn(RunMigrator)
        .DependsOn(Zip)
        .Requires(() => ConnectionString)
        .Requires(() => WebAppUser)
        .Requires(() => WebAppPassword)
        .Requires(() => WebAppName)
        .Executes(async () =>
        {
            Serilog.Log.Information(WebAppUser);
            var base64Auth = Convert.ToBase64String(Encoding.Default.GetBytes($"{WebAppUser}:{WebAppPassword}"));
            using (var memStream = new MemoryStream(File.ReadAllBytes(ArtifactDirectory / "deployment.zip")))
            {
                memStream.Position = 0;
                var content = new StreamContent(memStream);
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64Auth);
                var requestUrl = $"https://{WebAppName}.scm.azurewebsites.net/api/zipdeploy";
                var response = await httpClient.PostAsync(requestUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    Assert.Fail("Deployment returned status code: " + response.StatusCode);
                }
            }
        });
}
