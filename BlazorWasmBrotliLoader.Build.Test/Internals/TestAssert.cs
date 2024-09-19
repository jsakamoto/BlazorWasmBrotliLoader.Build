
using System.IO.Compression;

namespace BlazorWasmBrotliLoader.Build.Test.Internals;

internal static class TestAssert
{
    private static IEnumerable<(string Path, string RelativePath)> GetFiles(string dir, string patterns, bool recursive)
    {
        return patterns
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .SelectMany(pattern => Directory.GetFiles(dir, pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
            .Order()
            .Select(path => (Path: path, Relativeath: Path.GetRelativePath(dir, path)))
            .ToArray();
    }

    internal static void FilesAreEquals(
        string targetDir,
        string expectedDir,
        string patterns = "*.*",
        bool recursive = true,
        Func<(string RelativePath, string TargetContentLine, string ExpectedContentLine), bool>? filter = null
    )
    {
        var targetFiles = GetFiles(targetDir, patterns, recursive);
        var expectedFiles = GetFiles(expectedDir, patterns, recursive);

        targetFiles
            .Select(f => f.RelativePath)
            .Is(expectedFiles.Select(f => f.RelativePath));

        foreach (var targetFile in targetFiles)
        {
            var expectedFile = expectedFiles.First(f => f.RelativePath == targetFile.RelativePath);

            var targetFileContent = ReadAllLines(targetFile.Path).ToList();
            var expectedFileContent = ReadAllLines(expectedFile.Path).ToList();
            var maxLineCount = Math.Max(targetFileContent.Count, expectedFileContent.Count);
            targetFileContent.AddRange(Enumerable.Repeat("", maxLineCount - targetFileContent.Count));
            expectedFileContent.AddRange(Enumerable.Repeat("", maxLineCount - expectedFileContent.Count));

            var unmatchLineIndex = -1;
            for (var i = 0; i < maxLineCount; i++)
            {
                var targetContentline = targetFileContent[i];
                var expectedContentline = expectedFileContent[i];
                if (filter != null && filter((targetFile.RelativePath, targetContentline, expectedContentline)) == false) continue;

                if (targetContentline != expectedContentline)
                {
                    unmatchLineIndex = i;
                    break;
                }
            }

            if (unmatchLineIndex != -1)
            {
                var lineNumberWidth = maxLineCount.ToString().Length;
                var details = targetFileContent
                    .Select((content, index) => $"{index + 1}".PadLeft(lineNumberWidth) + " " + content)
                    .Select((content, index) => (unmatchLineIndex == index ? "> " : "  ") + content)
                    .ToList();
                var expectedContentLine = expectedFileContent[unmatchLineIndex];
                details.Insert(unmatchLineIndex + 1, "<-".PadRight(lineNumberWidth + 3) + expectedContentLine);
                var detail = details.Aggregate((current, next) => current + "\r\n" + next);

                Assert.Fail(message:
                    $"The file content of \"{targetFile.RelativePath}\", line number {unmatchLineIndex + 1} is not as expected.\r\n\r\n" +
                    detail);
            }
        }
    }

    private static IEnumerable<string> ReadAllLines(string filePath)
    {
        var fileStream = File.OpenRead(filePath);
        using var stream =
            filePath.EndsWith(".gz") ? new GZipStream(fileStream, CompressionMode.Decompress) as Stream :
            filePath.EndsWith(".br") ? new BrotliStream(fileStream, CompressionMode.Decompress) as Stream :
            fileStream;

        var reader = new StreamReader(stream);
        while (!reader.EndOfStream)
        {
            yield return reader.ReadLine() ?? "";
        }
    }
}
