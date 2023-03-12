namespace Nowy.UI.Common.Services;

public interface IWebAssetReferenceService
{
    public string? InitialPageTitle { get; set; } // .WebAssemblyEntryAssembly?.GetName().Name

    public IReadOnlyList<string> GetCssPaths();
    public IReadOnlyList<string> GetJavascriptPaths();
}
