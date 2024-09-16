
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


    internal static void FilesAreEquals(string targetDir, string expectedDir, string patterns = "*.*", bool recursive = true)
    {
        var targetFiles = GetFiles(targetDir, patterns, recursive);
        var expectedFiles = GetFiles(expectedDir, patterns, recursive);

        targetFiles
            .Select(f => f.RelativePath)
            .Is(expectedFiles.Select(f => f.RelativePath));

        foreach (var targetFile in targetFiles)
        {
            var expectedFile = expectedFiles.First(f => f.RelativePath == targetFile.RelativePath);

            var targetFileContent = File.ReadAllLines(targetFile.Path).ToList();
            var expectdFileContent = File.ReadAllLines(expectedFile.Path).ToList();
            var maxLineCount = Math.Max(targetFileContent.Count, expectdFileContent.Count);
            targetFileContent.AddRange(Enumerable.Repeat("", maxLineCount - targetFileContent.Count));
            expectdFileContent.AddRange(Enumerable.Repeat("", maxLineCount - expectdFileContent.Count));

            var unmatchLineIndex = -1;
            for (var i = 0; i < maxLineCount; i++)
            {
                if (targetFileContent[i] != expectdFileContent[i])
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
                var expectedContentLine = expectdFileContent[unmatchLineIndex];
                details.Insert(unmatchLineIndex + 1, "<-".PadRight(lineNumberWidth + 3) + expectedContentLine);
                var detail = details.Aggregate((current, next) => current + "\r\n" + next);

                Assert.Fail(message:
                    $"The file content of \"{targetFile.RelativePath}\", line number {unmatchLineIndex + 1} is not as expected.\r\n\r\n" +
                    detail);
            }
        }
    }
}
