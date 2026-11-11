namespace Modsen.Portal.Core.Export;

public interface IFileExporter<in TModel> where TModel : class
{
    Task<byte[]> Export(TModel model, CancellationToken token);
}
