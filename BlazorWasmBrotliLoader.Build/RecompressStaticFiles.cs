using System.IO.Compression;
using BrotliSharpLib;
using Microsoft.Build.Framework;

namespace BlazorWasmBrotliLoader;

public class RecompressStaticFiles : Microsoft.Build.Utilities.Task
{
    [Required]
    public ITaskItem[] Files { get; set; }

    public override bool Execute()
    {
        Parallel.ForEach(this.Files, file =>
        {
            this.Recompress(file.ItemSpec);
        });

        return true;
    }

    private void Recompress(string srcFilePath)
    {
        using var srcStream = System.IO.File.OpenRead(srcFilePath);

        this.Recompress(srcFilePath, ".gz", srcStream, targetStream => new GZipStream(targetStream, CompressionLevel.Optimal));

        this.Recompress(srcFilePath, ".br", srcStream, targetStream =>
        {
            var compressingStream = new BrotliStream(targetStream, CompressionMode.Compress);
            compressingStream.SetQuality(11);
            compressingStream.SetWindow(22);
            return compressingStream;
        });
    }

    private void Recompress(string srcFilePath, string extension, Stream srcStream, Func<Stream, Stream> createStream)
    {
        var compressedFilePath = srcFilePath + extension;
        if (!File.Exists(compressedFilePath)) return;

        using var targetStream = System.IO.File.Create(compressedFilePath);
        using var compressingStream = createStream(targetStream);
        srcStream.Seek(0, SeekOrigin.Begin);
        srcStream.CopyTo(compressingStream);
    }
}
