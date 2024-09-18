using System.Collections.Concurrent;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using BlazorWasmBrotliLoader.Models;
using Microsoft.Build.Framework;

namespace BlazorWasmBrotliLoader;

public class UpdateServiceWorkerAssetsManifestJs : Microsoft.Build.Utilities.Task
{
    [Required] public string WebRootPath { get; set; }

    [Required] public string ServiceWorkerAssetsManifestJs { get; set; }

    public bool InjectBrotliLoader { get; set; } = true;

    private class SHA256Pool : IDisposable
    {
        private readonly ConcurrentBag<SHA256> _sha256Pool = new();

        public string GetHashCode(string filePath)
        {
            var sha256 = this._sha256Pool.TryTake(out var v) ? v : SHA256.Create();
            try
            {
                using var stream = File.OpenRead(filePath);
                return "sha256-" + Convert.ToBase64String(sha256.ComputeHash(stream));
            }
            finally { this._sha256Pool.Add(sha256); }
        }

        public void Dispose() { while (this._sha256Pool.TryTake(out var sha256)) sha256.Dispose(); }
    }

    public override bool Execute()
    {
        var jsonSerializer = new DataContractJsonSerializer(typeof(AssetsManifestFile));

        // Load a "service-worker-assets.js" to a model object.
        if (!this.TryLoadAssetsManifestFile(jsonSerializer, out var assetsManifestFile)) return false;

        // Update a hash code of HTML files.
        using var sha256Poll = new SHA256Pool();
        var htmlAssets = assetsManifestFile.assets.Where(a => a.url.EndsWith(".html"));
        Parallel.ForEach(htmlAssets, htmlAsset =>
        {
            htmlAsset.hash = sha256Poll.GetHashCode(this.GetFullPath(htmlAsset));
        });

        // If the Brotli Loader is enabled, update all hash codes of asset entries that are compressed,
        // and add asset entries for brotli loader JavaScript files.
        if (this.InjectBrotliLoader)
        {
            Parallel.ForEach(assetsManifestFile.assets, asset =>
            {
                // ...but some kinds of files have to exclude.
                if (asset.url.ToLower().EndsWith(".html")) return;
                if (asset.url == "_framework/blazor.webassembly.js") return;
                if (Regex.IsMatch(asset.url, @"^_framework/dotnet(\..*)?\.js$")) return;

                var path = this.GetFullPath(asset);
                var compressedPath = path + ".br";
                if (!File.Exists(compressedPath)) return;

                asset.url += ".br";
                asset.hash = sha256Poll.GetHashCode(compressedPath);
            });

            var brotliLoaderScriptFiles = new[] { "decode.min.js", "brotliloader.min.js" };
            var brotliLoaderScriptEntries = brotliLoaderScriptFiles.Select(name =>
            {
                return new AssetsManifestFileEntry
                {
                    url = name,
                    hash = sha256Poll.GetHashCode(Path.Combine(this.WebRootPath, name))
                };
            });
            assetsManifestFile.assets = assetsManifestFile.assets.Concat(brotliLoaderScriptEntries).ToArray();
        }

        // Write back the model object to the "service-worker-assets.js"
        this.WriteAssetsManifestFile(jsonSerializer, assetsManifestFile);
        return true;
    }

    private bool TryLoadAssetsManifestFile(XmlObjectSerializer serializer, out AssetsManifestFile assetsManifestFile)
    {
        var serviceWorkerAssetsJs = File.ReadAllText(this.ServiceWorkerAssetsManifestJs);
        serviceWorkerAssetsJs = Regex.Replace(serviceWorkerAssetsJs, @"^self\.assetsManifest\s*=\s*", "");
        serviceWorkerAssetsJs = Regex.Replace(serviceWorkerAssetsJs, ";\\s*$", "");
        var serviceWorkerAssetsJsBytes = Encoding.UTF8.GetBytes(serviceWorkerAssetsJs);
        using var jsonReader = JsonReaderWriterFactory.CreateJsonReader(serviceWorkerAssetsJsBytes, XmlDictionaryReaderQuotas.Max);
        assetsManifestFile = serializer.ReadObject(jsonReader) as AssetsManifestFile;
        if (assetsManifestFile == null) return false;
        if (assetsManifestFile.assets == null) return false;
        return true;
    }

    private string GetFullPath(AssetsManifestFileEntry asset)
    {
        return Path.GetFullPath(Path.Combine(this.WebRootPath, asset.url));
    }

    private void WriteAssetsManifestFile(XmlObjectSerializer serializer, AssetsManifestFile assetsManifestFile)
    {
        using var serviceWorkerAssetsStream = File.OpenWrite(this.ServiceWorkerAssetsManifestJs);
        using var streamWriter = new StreamWriter(serviceWorkerAssetsStream, Encoding.UTF8, 50, leaveOpen: true);
        streamWriter.Write("self.assetsManifest = ");
        streamWriter.Flush();
        using var jsonWriter = JsonReaderWriterFactory.CreateJsonWriter(serviceWorkerAssetsStream, Encoding.UTF8, ownsStream: false, indent: true);
        serializer.WriteObject(jsonWriter, assetsManifestFile);
        jsonWriter.Flush();
        streamWriter.WriteLine(";");
    }
}
