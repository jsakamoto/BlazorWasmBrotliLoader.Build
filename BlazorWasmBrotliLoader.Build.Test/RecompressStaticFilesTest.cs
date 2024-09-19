using BlazorWasmBrotliLoader.Build.Test.Internals;
using Microsoft.Build.Utilities;

namespace BlazorWasmBrotliLoader.Build.Test;

public class RecompressStaticFilesTest
{
    [Test]
    public void RecompressStaticFiles_EnableAllOptions_Test()
    {
        // Given
        using var workDir = new WorkDir("RecompressStaticFilesTest", "001 Enable All Options");

        // When
        var task = new RecompressStaticFiles
        {
            Files = [
                new TaskItem(Path.Combine(workDir.TargetDir, "index.html")),
                new TaskItem(Path.Combine(workDir.TargetDir, "404.html"))
            ]
        };

        task.Execute().IsTrue();

        // Then: files are recompressed expectedly.
        TestAssert.FilesAreEquals(workDir.TargetDir, workDir.ExpectedDir);
    }

    [Test]
    public void RecompressStaticFiles_DisableGzip_Test()
    {
        // Given
        using var workDir = new WorkDir("RecompressStaticFilesTest", "002 Disable Gzip");

        // When
        var task = new RecompressStaticFiles
        {
            Files = [
                new TaskItem(Path.Combine(workDir.TargetDir, "index.html")),
            ]
        };

        task.Execute().IsTrue();

        // Then: files are recompressed expectedly.
        TestAssert.FilesAreEquals(workDir.TargetDir, workDir.ExpectedDir);
    }

    [Test]
    public void RecompressStaticFiles_DisableBrotli_Test()
    {
        // Given
        using var workDir = new WorkDir("RecompressStaticFilesTest", "003 Disable Brotli");

        // When
        var task = new RecompressStaticFiles
        {
            Files = [
                new TaskItem(Path.Combine(workDir.TargetDir, "index.html")),
                new TaskItem(Path.Combine(workDir.TargetDir, "404.html"))
            ]
        };

        task.Execute().IsTrue();

        // Then: files are recompressed expectedly.
        TestAssert.FilesAreEquals(workDir.TargetDir, workDir.ExpectedDir);
    }
}
