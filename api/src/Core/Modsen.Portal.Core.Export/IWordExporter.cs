namespace Modsen.Portal.Core.Export;

public interface IWordExporter<in TModel> : IFileExporter<TModel> where TModel : class;
