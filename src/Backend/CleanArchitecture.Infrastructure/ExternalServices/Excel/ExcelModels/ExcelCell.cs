namespace CleanArchitecture.Infrastructure.ExternalServices.Excel.ExcelModels
{
    public class ExcelCell
    {
        public string Value { get; set; }
        public int ColumnId { get; set; }
        public ExcelList NestedTable { get; set; }
        public int NestedLevel { get; set; }
    }
}
