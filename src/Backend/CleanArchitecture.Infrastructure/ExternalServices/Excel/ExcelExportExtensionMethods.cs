using CleanArchitecture.Domain.Attribute;
using CleanArchitecture.Infrastructure.ExternalServices.Excel.ExcelModels;

namespace CleanArchitecture.Infrastructure.ExternalServices.Excel;

public static class ExcelExportExtensionMethods
{
    public static ExcelList CreateList<T>(List<object> list)
    {
        ExcelList excelList;
        List<T> castedList = CastList<T>(list);
        excelList = CreateExcelList(castedList);
        return excelList;
    }
    private static List<TableColumn> GetColumns<T>()
    {
        var tableColumns = new List<TableColumn>();

        foreach (var item in typeof(T).GetProperties())
        {
            foreach (var attr in item.GetCustomAttributes(true))
            {
                if (attr is TableColumnAttribute)
                {
                    var tableColumnAttribute = attr as TableColumnAttribute;
                    tableColumns.Add(new TableColumn
                    {
                        ColumnHeading = tableColumnAttribute.ColumnHeading,
                        Class = "showColumn",
                        ColumnId = tableColumnAttribute.ColumnId,
                        Show = true,
                        ShowSortArrow = "hidden",
                        SortDirection = "top",
                        PropertyName = item.Name,
                        SortedColumn = tableColumnAttribute.InitialSort,
                        NestedTable = tableColumnAttribute.NestedTable,
                        showReportFooter = tableColumnAttribute.CalculateReport,
                        ReportType = tableColumnAttribute.ReportType
                    });
                    break;
                }
            }
        }
        return tableColumns.OrderBy(x => x.ColumnId).ToList();
    }

    private static ExcelList CreateExcelList<T>(List<T> list)
    {
        List<TableColumn> tableColumns = GetColumns<T>();

        ExcelList excelList = new ExcelList()
        {
            Headings = new List<Heading>(),
            ExcelRows = new List<ExcelRow>()
            
        };
        foreach (var column in tableColumns)
        {
            excelList.Headings.Add(new Heading() { ColumnId = column.ColumnId, HeadingName = column.ColumnHeading });
        }
        foreach (var package in list)
        {
            ExcelRow excelRow = new ExcelRow()
            {
                Cells = new List<ExcelCell>(),
            };

            var properties = package.GetType().GetProperties()
                .Where(p => p.Name != "Id" && p.Name != "Event") // Exclude unwanted properties
                .Take(12) // Ensure max 12 properties
                .ToList();

            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(true);
                if (attributes != null && attributes.Any())
                {
                    foreach (var attribute in attributes)
                    {
                        if (attribute.GetType().Name == "TableColumnAttribute")
                        {
                            var column = attribute as TableColumnAttribute;
                            var value = package.GetType().GetProperty(property.Name).GetValue(package);
                            if (value != null)
                            {
                                if (!column.NestedTable)
                                {
                                    excelRow.Cells.Add(new ExcelCell()
                                    {
                                        Value = value.ToString(),
                                        ColumnId = column.ColumnId
                                    });
                                }
                            }
                            else
                            {
                                excelRow.Cells.Add(new ExcelCell()
                                {
                                    Value = "Empty",
                                    ColumnId = column.ColumnId
                                });
                            }
                        }
                    }
                }
            }
            excelRow.Cells.OrderBy(x => x.ColumnId);
            excelList.ExcelRows.Add(excelRow);
        }
        return excelList;
    }

    private static List<T> CastList<T>(List<object> list)
    {
        List<T> castedList = new List<T>();
        foreach (var item in list)
        {
            castedList.Add((T)item);
        }

        return castedList;
    }
}