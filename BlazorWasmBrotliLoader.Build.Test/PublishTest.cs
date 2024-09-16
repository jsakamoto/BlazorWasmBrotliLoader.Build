using BlazorWasmBrotliLoader.Build.Test.Internals;
using Toolbelt;
using static Toolbelt.Diagnostics.XProcess;

namespace BlazorWasmBrotliLoader.Build.Test;

[Parallelizable(ParallelScope.Children)]
public class PublishTest
{
    private static void CopyBundleScriptsTo(string targetDir)
    {
        var solutionDir = FileIO.FindContainerDirToAncestor("BlazorWasmBrotliLoader.Build.sln");
        var bundleScriptsDir = Path.Combine(solutionDir, "BlazorWasmBrotliLoader.Build", "bundle", "scripts");
        foreach (var scriptName in new[] {  "decode.min.js", "brotliloader.min.js" })
        {
            File.Copy(
                sourceFileName: Path.Combine(bundleScriptsDir, scriptName),
                destFileName: Path.Combine(targetDir, scriptName));
        }
    }

    [Test]
    public async Task Publish_InjectBrotliLoader_Test()
    {
        // Given
        using var workDir = new WorkDir("PublishTest", "001 SampleApp");
        CopyBundleScriptsTo(workDir.ExpectedDir);
        File.Exists(Path.Combine(workDir.ProjectDir, "decode.min.js")).IsFalse();
        File.Exists(Path.Combine(workDir.ProjectDir, "brotliloader.min.js")).IsFalse();

        // When
        using var publishCommand = Start("dotnet", $"publish -c Release -o \"{workDir.TargetDir}\"", workDir.ProjectDir);
        await publishCommand.WaitForExitAsync();
        publishCommand.ExitCode.Is(0, message: publishCommand.Output);

        // Then
        TestAssert.FilesAreEquals(
            targetDir: Path.Combine(workDir.TargetDir, "wwwroot"),
            expectedDir: workDir.ExpectedDir,
            patterns: "*.html;*.js",
            recursive: false
        );
    }

    [Test]
    public async Task Publish_WithNoCompression_Test()
    {
        // Given
        using var workDir = new WorkDir("PublishTest", "002 SampleApp with no compression");

        // When
        using var publishCommand = Start("dotnet", $"publish -p CompressionEnabled=false -c Release -o \"{workDir.TargetDir}\"", workDir.ProjectDir);
        await publishCommand.WaitForExitAsync();
        publishCommand.ExitCode.Is(0, message: publishCommand.Output);

        // Then
        TestAssert.FilesAreEquals(
            targetDir: Path.Combine(workDir.TargetDir, "wwwroot"),
            expectedDir: workDir.ExpectedDir,
            patterns: "*.html;*.js",
            recursive: false
        );
    }

    [Test]
    public async Task Publish_WithRewriteBaseHref_Test()
    {
        // Given
        using var workDir = new WorkDir("PublishTest", "003 SampleApp with rewrite base href");
        CopyBundleScriptsTo(workDir.ExpectedDir);
        File.Exists(Path.Combine(workDir.ProjectDir, "decode.min.js")).IsFalse();
        File.Exists(Path.Combine(workDir.ProjectDir, "brotliloader.min.js")).IsFalse();

        // When
        using var publishCommand = Start("dotnet", $"publish -p BroltiLoaderRewriteBaseHref=true -p BroltiLoaderBaseHref=/fizz/buzz/ -c Release -o \"{workDir.TargetDir}\"", workDir.ProjectDir);
        await publishCommand.WaitForExitAsync();
        publishCommand.ExitCode.Is(0, message: publishCommand.Output);

        // Then
        TestAssert.FilesAreEquals(
            targetDir: Path.Combine(workDir.TargetDir, "wwwroot"),
            expectedDir: workDir.ExpectedDir,
            patterns: "*.html;*.js",
            recursive: false
        );
    }
}
