namespace Modsen.Portal.Core.Export;

public interface IExcelExporter<in TModel> : IFileExporter<TModel> where TModel : class;
