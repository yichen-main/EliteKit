namespace EliteKit.Serve.Expansions.CommonExamples;
internal static class ArchiveExpand
{
    const string upperPath = "..";
    public static void ToDeleteEmptyFolder(this string directoryPath)
    {
        if (directoryPath.IsEmptyFolder()) directoryPath.DeleteFolder();
    }
    public static bool IsEmptyFolder(this string directoryPath) =>
        Directory.GetDirectories(directoryPath).Length == default && Directory.GetFiles(directoryPath).Length == default;
    public static bool ExistFile(this string? path) => File.Exists(path);
    public static bool ExistFile(this string? path, in string? extension) => Path.GetExtension(path).IsMatch(extension);
    public static void DeleteFolder(this string directoryPath)
    {
        var entries = Directory.GetFileSystemEntries(directoryPath);
        for (var i = (int)default; i < entries.Length; i++)
        {
            var path = entries[i];
            if (File.Exists(path))
            {
                var attributes = File.GetAttributes(path);
                if ((attributes & FileAttributes.ReadOnly) is FileAttributes.ReadOnly)
                {
                    File.SetAttributes(path, attributes & ~FileAttributes.ReadOnly);
                }
                File.Delete(path);
            }
            else path.DeleteFolder();
        }
        Directory.Delete(directoryPath);
    }
    public static T? ReadJsonFile<T>(this string filePath, in string fileName)
    {
        var builder = new ConfigurationBuilder().SetBasePath(filePath).AddJsonFile(fileName, optional: true, reloadOnChange: true);
        return builder.AddEnvironmentVariables().Build().GetSection(typeof(T).Name).Get<T>()!;
    }
    public static T? ReadXmlFile<T>(this string filePath, in string fileName)
    {
        try
        {
            using var reader = XmlReader.Create(Path.Combine(filePath, fileName));
            XmlSerializer serializer = new(typeof(T));
            return (T?)serializer.Deserialize(reader);
        }
        catch (Exception)
        {
            return default;
        }
    }
    public static string GetFolderPath(this FolderType type) => Directory.CreateDirectory(type switch
    {
        FolderType.Log => Path.Combine(FolderType.Root.GetFolderPath(), upperPath, nameof(FolderType.Log)),
        FolderType.Module => Path.Combine(FolderType.Root.GetFolderPath(), nameof(FolderType.Module)),
        FolderType.Config => Path.Combine(FolderType.Root.GetFolderPath(), upperPath, nameof(FolderType.Config)),
        FolderType.Retention => Path.Combine(FolderType.Config.GetFolderPath(), nameof(FolderType.Retention)),
        _ => AppContext.BaseDirectory,
    }).FullName;
    public static string GetFolderPath(this string directoryName, in FolderType type = FolderType.Root) => Directory.CreateDirectory(type switch
    {
        FolderType.Log => Path.Combine(FolderType.Log.GetFolderPath(), directoryName),
        FolderType.Module => Path.Combine(FolderType.Root.GetFolderPath(), nameof(FolderType.Module), directoryName),
        FolderType.Config => Path.Combine(FolderType.Config.GetFolderPath(), directoryName),
        FolderType.Retention => Path.Combine(FolderType.Config.GetFolderPath(), nameof(FolderType.Retention), directoryName),
        _ => Path.Combine(FolderType.Root.GetFolderPath(), directoryName),
    }).FullName;
    public static Task RecordAsync(in string directoryName, in string message, in string format = "yyyyMMdd", in int offset = 8)
    {
        var banner = new StringBuilder().AppendLine(message).ToString();
        var fileName = $"{DateTime.UtcNow.AddHours(offset).ToString(format, CultureInfo.InvariantCulture)}{FileExtension.Log}";
        return File.AppendAllTextAsync(Path.Combine(directoryName.GetFolderPath(FolderType.Log), fileName), banner, Encoding.UTF8);
    }
    public static void ExecutionScavenger(in int retentionDays = 10, in string timeFormat = "yyyyMMdd")
    {
        if (Directory.Exists(FileLayout.LogFolderLocation))
        {
            foreach (var directoryInfo in new DirectoryInfo(FileLayout.LogFolderLocation).GetDirectories())
            {
                foreach (var filePath in Directory.GetFiles(directoryInfo.FullName))
                {
                    var fileName = Path.GetFileNameWithoutExtension(filePath);
                    if (DateTime.TryParseExact(fileName, timeFormat, default, DateTimeStyles.None, out var date))
                    {
                        if (Timestamp.Subtract(date).TotalDays > retentionDays) File.Delete(filePath);
                    }
                    else File.Delete(filePath);
                }
                if (Directory.GetFiles(directoryInfo.FullName).Length == default) Directory.Delete(directoryInfo.FullName);
            }
            FileLayout.ClearEmptyFolders(FileLayout.LogFolderLocation);
        }
    }
    public static DateTime Timestamp { get { return DateTime.UtcNow.AddHours(8); } }
}