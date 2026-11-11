namespace Bugger.Core;

// Sound files live in a sounds folder beside the executable. Only plain .wav file names
// are accepted so task records can never point outside that folder.
public static class SoundLibrary
{
    public const string FolderName = "sounds";
    public static string Folder(string baseDirectory) => Path.Combine(baseDirectory, FolderName);

    public static string[] Names(string baseDirectory)
    {
        var folder = Folder(baseDirectory);
        if (!Directory.Exists(folder)) return [];
        return Directory.EnumerateFiles(folder)
            .Where(f => string.Equals(Path.GetExtension(f), ".wav", StringComparison.OrdinalIgnoreCase))
            .Select(Path.GetFileName)
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Select(n => n!)
            .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public static bool ValidName(string? name) => !string.IsNullOrWhiteSpace(name)
        && name == Path.GetFileName(name)
        && name.IndexOfAny(['/', '\\']) < 0
        && name.EndsWith(".wav", StringComparison.OrdinalIgnoreCase);

    public static string? Resolve(string baseDirectory, string? name)
    {
        if (!ValidName(name)) return null;
        var path = Path.Combine(Folder(baseDirectory), name!);
        return File.Exists(path) ? path : null;
    }
}
