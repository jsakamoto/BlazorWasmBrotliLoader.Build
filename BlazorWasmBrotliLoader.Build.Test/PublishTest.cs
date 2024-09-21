using BlazorWasmBrotliLoader.Build.Test.Internals;
using static Toolbelt.Diagnostics.XProcess;

namespace BlazorWasmBrotliLoader.Build.Test;

[Parallelizable(ParallelScope.Children)]
public class PublishTest
{
    [Test]
    public async Task Publish_InjectBrotliLoader_Test()
    {
        // Given
        using var workDir = new WorkDir("PublishTest", "001 SampleApp");
        BundleScripts.CopyTo(workDir.ExpectedDir);
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
            patterns: "*.html;*.html.gz;*.html.br;decode.min.js;brotliloader.min.js"
        );
    }

    [Test]
    public async Task Publish_with_NoCompression_Test()
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
            patterns: "*.html;*.html.gz;*.html.br;decode.min.js;brotliloader.min.js"
        );
    }

    [Test]
    public async Task Publish_PWA_with_RewriteBaseHref_Test()
    {
        // Given
        using var workDir = new WorkDir("PublishTest", "003 Sample PWA with rewrite base href");
        BundleScripts.CopyTo(workDir.ExpectedDir);
        File.Exists(Path.Combine(workDir.ProjectDir, "decode.min.js")).IsFalse();
        File.Exists(Path.Combine(workDir.ProjectDir, "brotliloader.min.js")).IsFalse();

        // When
        using var publishCommand = Start("dotnet", $"publish -p BrotliLoaderRewriteBaseHref=true -p BrotliLoaderBaseHref=/fizz/buzz/ -c Release -o \"{workDir.TargetDir}\"", workDir.ProjectDir);
        await publishCommand.WaitForExitAsync();
        publishCommand.ExitCode.Is(0, message: publishCommand.Output);

        // Then
        TestAssert.FilesAreEquals(
            targetDir: Path.Combine(workDir.TargetDir, "wwwroot"),
            expectedDir: workDir.ExpectedDir,
            patterns: "*.html;*.js;*.html.gz;*.html.br",
            recursive: false,
            filter: args => args.ExpectedContentLine.Contains("(*ignore*)") ? false : true
        );
    }
}
