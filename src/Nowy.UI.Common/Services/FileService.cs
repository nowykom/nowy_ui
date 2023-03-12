using Microsoft.Extensions.Logging;

namespace Nowy.UI.Common.Services;

public sealed record FileResult(string FileName, Func<Task<Stream>> OpenStreamAsync);

public abstract class FileService
{
    private readonly ILogger _logger;

    protected static readonly Random _random = new();

    public abstract string CreateTempFilePath(string? prefix = null);
    public abstract string GetPathLocalCache();
    public abstract string GetPathLocalDCIM();
    public abstract string GetPathLocalDatabase();
    public abstract Task<IReadOnlyList<FileResult>> TakeImageAsync();
    public abstract Task<IReadOnlyList<FileResult>> TakeVideoAsync();
    public abstract Task<IReadOnlyList<FileResult>> PickImageAsync();
    public abstract Task<IReadOnlyList<FileResult>> PickVideoAsync();
    public virtual Task<bool> ResizeImageAsync(string path_source, string path_target) => Task.FromResult(false);

    protected FileService(ILogger logger)
    {
        this._logger = logger;
    }

    public async Task CopyToFileAsync(Stream response_stream, string path_local_temp, string path_local, string where_desc)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path_local_temp)!);
        Directory.CreateDirectory(Path.GetDirectoryName(path_local)!);

        await using (Stream local_stream_download = File.Open(path_local_temp, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await response_stream.CopyToAsync(local_stream_download);
        }

        if (new FileInfo(path_local_temp).Length == 0)
        {
            throw new Exception($"Download failed {where_desc}. File is empty.");
        }

        if (File.Exists(path_local))
        {
            File.Delete(path_local);
        }

        File.Move(path_local_temp, path_local);

        for (int i = 0; i < 10; i++)
        {
            try
            {
                if (File.Exists(path_local_temp))
                {
                    File.Delete(path_local_temp);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogWarning(ex, $"Failed to delete temp file: {path_local_temp}");
            }
        }
    }
}
