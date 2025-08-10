using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Application.Interfaces.ServiceInterfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.ExternalServices.Excel;
using ExcelExportExtensionMethods = CleanArchitecture.Infrastructure.ExternalServices.Excel.ExcelExportExtensionMethods;
using ExcelList = CleanArchitecture.Infrastructure.ExternalServices.Excel.ExcelModels.ExcelList;

namespace CleanArchitecture.Infrastructure.ExternalServices;

public class ExcelExportService : IExcelExportService
{
    private readonly IUnitOfWork _uow;
    private readonly ISerilogLogger _logger;


    public ExcelExportService(IUnitOfWork uow, ISerilogLogger logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<FileResultDto> GetExcelFile(Guid fileId)
    {

        try
        {
            MemoryStream fileStream = await _uow.CreateRepository<IExcelExportRepository>().GetExcelFileAsync(fileId);

            if (fileStream == null)
            {
                _logger.LogInformation($"Could not find file with ID: {fileId}");
                return null;

            }

            string content = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            _logger.LogInformation("File found");
            return new FileResultDto
            {
                FileStream = fileStream,
                ContentType = content

            };
      
           
        }
        catch (Exception e)
        {
            _logger.LogError("Error",e);
            throw;
        }
      
    }

    public async Task<ExcelFile> CreateExcelFileGoodsReceiveOrder(List<object> list, string userId, string listType)
    {

        try
        {
            if (listType == typeof(EventRegistrationFormDto).Name)
            {
                _logger.LogInformation("Excel file created for form.");

                return await CreateFile<EventRegistrationFormDto>(list, userId);
            }
            _logger.LogInformation("No Excel file created for form.");
            return null;
        }
        catch (Exception e)
        {
            _logger.LogError("Error", e);

            throw;
        }


       
    }
    public async Task<ExcelFile> CreateFile<T>(List<object> list, string userId)
    {

        try
        {
            ExcelList excelList = new ExcelList();
            excelList = ExcelExportExtensionMethods.CreateList<T>(list);
            var excelObj = await _uow.CreateRepository<IExcelExportRepository>().CreateExcelFileAsync(excelList, userId);
            if (excelObj == null)
            {
                _logger.LogInformation("Excel object are null, No file created");
                return new ExcelFile();
            }
            await _uow.CompleteAsync();
            _logger.LogInformation("Excel file created");
            return excelObj;
          
        }
        catch (Exception e)
        {
            _logger.LogError("Error", e);
            throw;
        }
    
    }
    
}