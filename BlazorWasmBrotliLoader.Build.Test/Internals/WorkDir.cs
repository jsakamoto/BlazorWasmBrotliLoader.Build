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

    public static implicit operator string(WorkDir workDirectory) => workDirectory._workDir.Path;

    public void Dispose() => this._workDir.Dispose();
}
