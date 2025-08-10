using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data;
using CleanArchitecture.Infrastructure.ExternalServices.Excel;
using Microsoft.EntityFrameworkCore;
using ExcelList = CleanArchitecture.Infrastructure.ExternalServices.Excel.ExcelModels.ExcelList;

namespace CleanArchitecture.Infrastructure.Repositories;

public class ExcelExportRepository : IExcelExportRepository
{

    private readonly ApplicationDbContext _dbContext;
    private readonly IWorkBook _workBook;

    public ExcelExportRepository(ApplicationDbContext context, IWorkBook workBook)
    {
        this._dbContext = context;
        this._workBook = workBook;

    }


    public async Task<ExcelFile> CreateExcelFileAsync(ExcelList excelList, string userId)
    {
        var path = _workBook.Create(excelList);

        var file = new ExcelFile()
        {
            Id = Guid.NewGuid(),
            Path = path,
            ApplicationUserId = userId
        };

        await _dbContext.ExcelFiles.AddAsync(file);
        return file;


    }

    public async Task<MemoryStream> GetExcelFileAsync(Guid fileId)
    {
        var data = await _dbContext.ExcelFiles.FirstOrDefaultAsync(x => x.Id == fileId);
        if (data != null)
        {
            var bytes = File.ReadAllBytes(data.Path);
            MemoryStream stream = new MemoryStream(bytes);
            return stream;
        }
        else
        {
            return null;
        }
    }
}