using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace Nowy.UI.Common.Services;

public class WasmFileService : FileService
{
    private readonly IJSRuntime? _js;
    private readonly ILogger<WasmFileService> _logger;

    public WasmFileService(IJSRuntime js, ILogger<WasmFileService> logger)
        : base(logger)
    {
        this._js = js;
        this._logger = logger;
    }

    public override string CreateTempFilePath(string? prefix = null)
    {
        string path_target = Path.Combine("temp", $"{UnixTimestamp.Now.FormatInternational().ToAlphaNumericOnly()}_{prefix ?? "null"}_{_random.NextLong()}");
        Directory.CreateDirectory(Path.GetDirectoryName(path_target)!);
        return path_target;
    }

    public override string GetPathLocalCache()
    {
        return Path.Combine("Download");
    }

    public override string GetPathLocalDCIM()
    {
        return Path.Combine("DCIM");
    }

    public override string GetPathLocalDatabase()
    {
        return Path.Combine("Database");
    }

    public override async Task<IReadOnlyList<FileResult>> TakeImageAsync()
    {
        IJSRuntime? js = this._js;
        if (js is null)
        {
            this._logger.LogError($"{nameof(IJSRuntime)} is null.");
            return Array.Empty<FileResult>();
        }

        await js.InvokeVoidAsync("file_input_trigger", args: Array.Empty<object?>());

        return Array.Empty<FileResult>();
    }

    public override async Task<IReadOnlyList<FileResult>> TakeVideoAsync()
    {
        IJSRuntime? js = this._js;
        if (js is null)
        {
            this._logger.LogError($"{nameof(IJSRuntime)} is null.");
            return Array.Empty<FileResult>();
        }

        await js.InvokeVoidAsync("file_input_trigger", args: Array.Empty<object?>());

        return Array.Empty<FileResult>();
    }

    public override async Task<IReadOnlyList<FileResult>> PickImageAsync()
    {
        IJSRuntime? js = this._js;
        if (js is null)
        {
            this._logger.LogError($"{nameof(IJSRuntime)} is null.");
            return Array.Empty<FileResult>();
        }

        await js.InvokeVoidAsync("file_input_trigger", args: Array.Empty<object?>());

        return Array.Empty<FileResult>();
    }

    public override async Task<IReadOnlyList<FileResult>> PickVideoAsync()
    {
        IJSRuntime? js = this._js;
        if (js is null)
        {
            this._logger.LogError($"{nameof(IJSRuntime)} is null.");
            return Array.Empty<FileResult>();
        }

        await js.InvokeVoidAsync("file_input_trigger", args: Array.Empty<object?>());

        return Array.Empty<FileResult>();
    }
}
