namespace Modsen.Portal.Core.Export;

public interface ICsvExporter<in TModel> : IFileExporter<TModel> where TModel : class;
