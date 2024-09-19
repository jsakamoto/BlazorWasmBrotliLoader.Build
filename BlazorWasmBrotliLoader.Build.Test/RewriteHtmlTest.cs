using BlazorWasmBrotliLoader.Build.Test.Internals;

namespace BlazorWasmBrotliLoader.Build.Test;

public class RewriteHtmlTest
{
    [Test]
    public void RewriteHtml_EnableAllOptions_Test()
    {
        // Given
        using var workDir = new WorkDir("RewriteHtmlTest", "001 Enable All Options");

        // When
        var task = new RewriteHtml
        {
            WebRootPath = workDir.TargetDir,
            FileSearchPatterns = "*.html",
            Recursive = true,
            InjectBrotliLoader = true,
            RewriteBaseHref = true,
            BaseHref = "/foo/bar/",
        };

        task.Execute().IsTrue();

        // Then: 1. files are rewrited expectedly.
        TestAssert.FilesAreEquals(workDir.TargetDir, workDir.ExpectedDir);

        // Then: 2. The output parameter shows files that are rewrited.
        task.RewrittenFiles.Select(item => Path.GetRelativePath(workDir.TargetDir, item.ItemSpec)).Order().Is(
            $"normal.html",
            $"sub{Path.DirectorySeparatorChar}has-autostart-true.html",
            $"sub{Path.DirectorySeparatorChar}no-blazor.html");
    }

    [Test]
    public void RewriteHtml_DisableRecursive_Test()
    {
        // Given
        using var workDir = new WorkDir("RewriteHtmlTest", "002 Disable Recursive");

        // When
        var task = new RewriteHtml
        {
            WebRootPath = workDir.TargetDir,
            FileSearchPatterns = "*.html",
            Recursive = false,
            InjectBrotliLoader = true,
            RewriteBaseHref = true,
            BaseHref = "/foo/bar/",
        };

        task.Execute().IsTrue();

        // Then: 1. files are rewrited expectedly.
        TestAssert.FilesAreEquals(workDir.TargetDir, workDir.ExpectedDir);

        // Then: 2. The output parameter shows files that are rewrited.
        task.RewrittenFiles.Select(item => Path.GetRelativePath(workDir.TargetDir, item.ItemSpec)).Order().Is(
            $"normal.html");
    }

    [Test]
    public void RewriteHtml_DisableInjectBrotliLoader_Test()
    {
        // Given
        using var workDir = new WorkDir("RewriteHtmlTest", "003 Disable Inject Brotli Loader");

        // When
        var task = new RewriteHtml
        {
            WebRootPath = workDir.TargetDir,
            FileSearchPatterns = "*.html",
            Recursive = true,
            InjectBrotliLoader = false,
            RewriteBaseHref = true,
            BaseHref = "/foo/bar/",
        };

        task.Execute().IsTrue();

        // Then: 1. files are rewrited expectedly.
        TestAssert.FilesAreEquals(workDir.TargetDir, workDir.ExpectedDir);

        // Then: 2. The output parameter shows files that are rewrited.
        task.RewrittenFiles.Select(item => Path.GetRelativePath(workDir.TargetDir, item.ItemSpec)).Order().Is(
            $"normal.html",
            $"sub{Path.DirectorySeparatorChar}has-autostart-true.html",
            $"sub{Path.DirectorySeparatorChar}no-blazor.html");
    }

    [Test]
    public void RewriteHtml_DisableRewriteBaseHref_Test()
    {
        // Given
        using var workDir = new WorkDir("RewriteHtmlTest", "004 Disable Rewrite Base Href");

        // When
        var task = new RewriteHtml
        {
            WebRootPath = workDir.TargetDir,
            FileSearchPatterns = "*.html",
            Recursive = true,
            InjectBrotliLoader = true,
            RewriteBaseHref = false,
            BaseHref = "/foo/bar/",
        };

        task.Execute().IsTrue();

        // Then: 1. files are rewrited expectedly.
        TestAssert.FilesAreEquals(workDir.TargetDir, workDir.ExpectedDir);

        // Then: 2. The output parameter shows files that are rewrited.
        task.RewrittenFiles.Select(item => Path.GetRelativePath(workDir.TargetDir, item.ItemSpec)).Order().Is(
            $"normal.html",
            $"sub{Path.DirectorySeparatorChar}has-autostart-true.html");
    }
}