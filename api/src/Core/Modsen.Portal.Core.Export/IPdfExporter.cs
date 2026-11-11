namespace Modsen.Portal.Core.Export;

public interface IPdfExporter<in TModel> : IFileExporter<TModel> where TModel : class;
