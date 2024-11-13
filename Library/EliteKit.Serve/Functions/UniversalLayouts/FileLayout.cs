namespace EliteKit.Serve.Functions.UniversalLayouts;
public readonly ref struct FileLayout
{
    public static bool IsFile(in string? path) => path.ExistFile();
    public static bool IsCsvFile(in string? path) => path.ExistFile(FileExtension.CSV);
    public static bool IsXmlFile(in string? path) => path.ExistFile(FileExtension.XML);
    public static void ClearEmptyFolders(in string path) => path.ToDeleteEmptyFolder();
    public static string GetProjectName<T>() => GetProjectName(typeof(T));
    public static string GetProjectName(in Type type) => type.Assembly.GetName().Name!;
    public static string GetProjectName<T>(in T entity) where T : notnull => GetProjectName(entity.GetType());
    public static string GetRootFolderPath(in string directoryName) => directoryName.GetFolderPath();
    public static string GetLogFolderPath(in string directoryName) => directoryName.GetFolderPath(FolderType.Log);
    public static string GetConfigFolderPath(in string directoryName) => directoryName.GetFolderPath(FolderType.Config);
    public static string GetModuleFilePath(in string fileName) => Path.Combine(ModuleFolderLocation, fileName);
    public static T? GetRootJsonFile<T>(string fileName = "appsettings.json") => ReadJsonFile<T>(RootFolderLocation, fileName);
    public static string GetFullFilePath(string directoryName, in string fileName, in string extensionName)
    {
        return Path.Combine(directoryName, $"{Path.GetFileNameWithoutExtension(fileName)}{extensionName}");
    }
    public static string GetSnowflakeId() => IdAlgorithm.Next().ToString(CultureInfo.InvariantCulture);
    public static T? ReadJsonFile<T>(in string filePath, in string fileName) => filePath.ReadJsonFile<T>(fileName);
    public static T? ReadXmlFile<T>(in string filePath, in string fileName) => filePath.ReadXmlFile<T>(fileName);
    public static string LogFolderLocation => FolderType.Log.GetFolderPath();
    public static string RootFolderLocation => FolderType.Root.GetFolderPath();
    public static string ModuleFolderLocation => FolderType.Module.GetFolderPath();
    public static string ConfigFolderLocation => FolderType.Config.GetFolderPath();
    public static string RetentionFolderLocation => FolderType.Retention.GetFolderPath();
}