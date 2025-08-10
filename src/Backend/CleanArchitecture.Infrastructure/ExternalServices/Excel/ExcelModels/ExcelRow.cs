namespace CleanArchitecture.Infrastructure.ExternalServices.Excel.ExcelModels
{
    public class ExcelRow
    {
        public List<ExcelCell> Cells { get; set; }
        public int RowNumber { get; set; }
    }
}