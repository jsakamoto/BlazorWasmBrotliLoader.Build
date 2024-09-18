using Toolbelt;

namespace BlazorWasmBrotliLoader.Build.Test.Internals;

internal static class BundleScripts
{
    public static void CopyTo(string targetDir)
    {
        var solutionDir = FileIO.FindContainerDirToAncestor("BlazorWasmBrotliLoader.Build.sln");
        var bundleScriptsDir = Path.Combine(solutionDir, "BlazorWasmBrotliLoader.Build", "bundle", "scripts");
        foreach (var scriptName in new[] { "decode.min.js", "brotliloader.min.js" })
        {
            File.Copy(
                sourceFileName: Path.Combine(bundleScriptsDir, scriptName),
                destFileName: Path.Combine(targetDir, scriptName));
        }
    }
}
