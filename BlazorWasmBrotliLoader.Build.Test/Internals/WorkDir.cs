using Toolbelt;

namespace BlazorWasmBrotliLoader.Build.Test.Internals;

internal class WorkDir : IDisposable
{
    private readonly WorkDirectory _workDir;

    public string ProjectDir { get; }
    public string TargetDir { get; }
    public string ExpectedDir { get; }

    public WorkDir(params string[] subDirs)
    {
        var testProjectDir = FileIO.FindContainerDirToAncestor("BlazorWasmBrotliLoader.Build.Test.csproj");
        var srcDir = Path.Combine(testProjectDir, "Fixtures", Path.Combine(subDirs));
        this._workDir = WorkDirectory.CreateCopyFrom(srcDir, item => item.Name is not "bin" and not "obj");
        this.ProjectDir = Path.Combine(this._workDir, "Project");
        this.TargetDir = Path.Combine(this._workDir, "Target");
        this.ExpectedDir = Path.Combine(this._workDir, "Expected");
    }

    public static implicit operator string(WorkDir workDirectory)
    {
        return workDirectory._workDir.Path;
    }

    //internal static WorkDirectory SetupWorkDir(params string[] subDirs)
    //{
    //    var srcDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fixtures", Path.Combine(subDirs));
    //    return SetupWorkDirCore(srcDir);
    //}

    //internal static WorkDirectory SetupWorkDir(string siteType, string protocol)
    //{
    //    var srcDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fixtures", siteType, protocol);
    //    return SetupWorkDirCore(srcDir);
    //}

    //private static WorkDirectory SetupWorkDirCore(string srcDir)
    //{
    //    var workDir = Toolbelt.WorkDirectory.CreateCopyFrom(srcDir, null);
    //    var gitDir = Path.Combine(workDir, "(.git)");
    //    if (Directory.Exists(gitDir))
    //    {
    //        Directory.Move(gitDir, Path.Combine(workDir, ".git"));
    //    }

    //    return workDir;
    //}

    public void Dispose()
    {
        this._workDir.Dispose();
    }
}
