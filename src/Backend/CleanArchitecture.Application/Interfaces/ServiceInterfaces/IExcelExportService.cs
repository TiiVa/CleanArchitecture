using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Interfaces.ServiceInterfaces;

public interface IExcelExportService
{
    Task<FileResultDto> GetExcelFile(Guid fileId);
    Task<ExcelFile> CreateExcelFileGoodsReceiveOrder(List<object> list, string userId, string listType);
    Task<ExcelFile> CreateFile<T>(List<object> list, string userId);
}