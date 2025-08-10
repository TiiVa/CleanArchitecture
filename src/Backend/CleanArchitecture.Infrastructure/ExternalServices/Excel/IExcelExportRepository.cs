using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.ExternalServices.Excel.ExcelModels;

namespace CleanArchitecture.Infrastructure.ExternalServices.Excel;

public interface IExcelExportRepository
{
    Task<ExcelFile> CreateExcelFileAsync(ExcelList excelList, string userId);
    Task<MemoryStream> GetExcelFileAsync(Guid fileId);
}