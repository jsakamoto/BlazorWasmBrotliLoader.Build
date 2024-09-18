using BlazorWasmBrotliLoader.Build.Test.Internals;

namespace BlazorWasmBrotliLoader.Build.Test;

public class UpdateServiceWorkerAssetsManifestJsTest
{
    [Test]
    public void UpdateServiceWorkerAssetsManifestJs_EnableAllOptions_Test()
    {
        // Given
        using var workDir = new WorkDir("UpdateServiceWorkerAssetsManifestJsTest", "001 Enable All Options");
        BundleScripts.CopyTo(workDir.TargetDir);

        // When
        var task = new UpdateServiceWorkerAssetsManifestJs
        {
            WebRootPath = workDir.TargetDir,
            ServiceWorkerAssetsManifestJs = Path.Combine(workDir.TargetDir, "service-worker-assets.js"),
            InjectBrotliLoader = true
        };

        task.Execute().IsTrue();

        // Then
        TestAssert.FilesAreEquals(
            workDir.TargetDir, 
            workDir.ExpectedDir,
            "service-worker-assets.js", 
            false);
    }

    [Test]
    public void UpdateServiceWorkerAssetsManifestJs_DisableInjectBrotliLoader_Test()
    {
        // Given
        using var workDir = new WorkDir("UpdateServiceWorkerAssetsManifestJsTest", "002 Disable Inject Brotli Loader");

        // When
        var task = new UpdateServiceWorkerAssetsManifestJs
        {
            WebRootPath = workDir.TargetDir,
            ServiceWorkerAssetsManifestJs = Path.Combine(workDir.TargetDir, "my-assets.js"),
            InjectBrotliLoader = false
        };

        task.Execute().IsTrue();

        // Then
        TestAssert.FilesAreEquals(
            workDir.TargetDir,
            workDir.ExpectedDir,
            "my-assets.js",
            false);
    }
}