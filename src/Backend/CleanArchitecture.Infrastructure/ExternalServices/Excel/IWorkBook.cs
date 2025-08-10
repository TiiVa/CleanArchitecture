
using CleanArchitecture.Infrastructure.ExternalServices.Excel.ExcelModels;

namespace CleanArchitecture.Infrastructure.ExternalServices.Excel
{
    public interface IWorkBook
    {
        string Create(ExcelList list);
    }
}