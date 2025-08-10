using System.Linq.Expressions;
using System.Reflection;
using CleanArchitecture.Domain.Attribute;
using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Infrastructure.ExternalServices.Excel;

public class TableColumn
{
    [TableColumn(columnHeading: "ColumnHeading", isTableSetting: true, columnId: 1)]
    public string ColumnHeading { get; set; }
    [TableColumn(columnHeading: "Class", isTableSetting: true, columnId: 2)]
    public string Class { get; set; }
    [TableColumn(columnHeading: "ColumnId", isTableSetting: true, columnId: 3)]
    public int ColumnId { get; set; }
    [TableColumn(columnHeading: "Show", isTableSetting: true, columnId: 4)]
    public bool Show { get; set; }
    public string ShowSortArrow { get; set; }
    public string SortDirection { get; set; }
    [TableColumn(columnHeading: "PropertyName", isTableSetting: true, columnId: 5)]
    public string PropertyName { get; set; }
    public bool SortedColumn { get; set; }
    public bool NestedTable { get; set; }
    public int SumColumn { get; set; }
    public int AverageColumn { get; set; }
    public int ColumnRowCount { get; set; }
    public ReportType ReportType { get; set; }
    public bool showReportFooter { get; set; }

    public static List<TableColumn> GetTableColumnHeadings<T>()
    {
        var tableColumns = new List<TableColumn>();

        foreach (var item in typeof(T).GetProperties())
        {
            foreach (var attr in item.GetCustomAttributes(true))
            {
                if (attr is TableColumnAttribute)
                {
                    var tableColumnAttribute = attr as TableColumnAttribute;
                    tableColumns.Add(new TableColumn()
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

    public static string GetTableColumnHeading<T>(string property)
    {
        try
        {
            var propertyInfo = typeof(T).GetProperty(property);
            if (propertyInfo != null)
            {
                var customAttributes = propertyInfo.GetCustomAttributes(true);
                if (customAttributes != null && customAttributes.Count() > 0)
                {
                    var attr = customAttributes.FirstOrDefault(x => x is TableColumnAttribute);
                    if (attr != null)
                    {
                        var tablecolumnAttribute = attr as TableColumnAttribute;
                        return tablecolumnAttribute.ColumnHeading;
                    }
                    else
                    {
                        return property;
                    }
                }
                return property;
            }
            else
            {
                return property;
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);

        }
    }

    public static MemberInfo GetPropertyInformation(Expression propertyExpression)
    {
        MemberExpression memberExpr = propertyExpression as MemberExpression;
        if (memberExpr == null)
        {
            UnaryExpression unaryExpr = propertyExpression as UnaryExpression;
            if (unaryExpr != null && unaryExpr.NodeType == ExpressionType.Convert)
            {
                memberExpr = unaryExpr.Operand as MemberExpression;
            }
        }

        if (memberExpr != null && memberExpr.Member.MemberType == MemberTypes.Property)
        {
            return memberExpr.Member;
        }

        return null;
    }

    public static string GetTableColumnHeading<T>(Expression<Func<T, object>> propertyExpression)
    {
        try
        {
            var memberInfo = GetPropertyInformation(propertyExpression.Body);
            if (memberInfo == null)
            {
                throw new ArgumentException(
                    "No property reference expression was found.",
                    "propertyExpression");
            }


            if (memberInfo != null)
            {
                var customAttributes = memberInfo.GetCustomAttributes(true);
                if (customAttributes != null && customAttributes.Length > 0)
                {
                    var attr = customAttributes.FirstOrDefault(x => x is TableColumnAttribute);
                    if (attr != null)
                    {
                        var tablecolumnAttribute = attr as TableColumnAttribute;
                        return tablecolumnAttribute.ColumnHeading;
                    }
                    else
                    {
                        return memberInfo.Name;
                    }
                }
                return memberInfo.Name;
            }
            else
            {
                return memberInfo.Name;
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);

        }
    }
    public void CalculateReportData<T>(List<T> list)
    {
        var propertyToCalculate = this.PropertyName;
        var reportType = this.ReportType;
        if (reportType == ReportType.Count)
        {
            ColumnRowCount = CalculateRowsInTableColumn(list, propertyToCalculate);
        }
        else if (reportType == ReportType.Sum)
        {
            SumColumn = CalculateSumInTableColumn(list, propertyToCalculate);
        }
        else if (reportType == ReportType.Average)
        {
            AverageColumn = CalculateAverageInTableColumn(list, propertyToCalculate);
        }
        else if (reportType == ReportType.None)
        {

        }
        else
        {

        }
    }

    private int CalculateAverageInTableColumn<T>(List<T> list, string propertyName)
    {
        var firstValue = list.FirstOrDefault().GetType().GetProperty(propertyName).GetValue(list.FirstOrDefault());

        if (ValueIsNumber(firstValue))
        {
            if (firstValue is int)
            {
                var result = list.Average(x => (int)x.GetType().GetProperty(propertyName).GetValue(x));
                return (int)result;
            }
            else if (firstValue is double)
            {
                var result = list.Average(x => (double)x.GetType().GetProperty(propertyName).GetValue(x));
                return (int)result;
            }
            else if (firstValue is decimal)
            {
                var result = list.Average(x => (double)x.GetType().GetProperty(propertyName).GetValue(x));
                return (int)result;
            }
            else if (firstValue is float)
            {
                var result = list.Average(x => (float)x.GetType().GetProperty(propertyName).GetValue(x));
                return (int)result;
            }
            else if (firstValue is ulong)
            {
                var result = list.Average(x => (float)x.GetType().GetProperty(propertyName).GetValue(x));
                return (int)result;
            }
            else if (firstValue is long)
            {
                var result = list.Average(x => (long)x.GetType().GetProperty(propertyName).GetValue(x));
                return (int)result;
            }
            else if (firstValue is uint)
            {
                var result = list.Average(x => (uint)x.GetType().GetProperty(propertyName).GetValue(x));
                return (int)result;
            }
            else if (firstValue is ushort)
            {
                var result = list.Average(x => (ushort)x.GetType().GetProperty(propertyName).GetValue(x));
                return (int)result;
            }
            else if (firstValue is short)
            {
                var result = list.Average(x => (short)x.GetType().GetProperty(propertyName).GetValue(x));
                return (int)result;
            }
            else if (firstValue is byte)
            {
                var result = list.Average(x => (byte)x.GetType().GetProperty(propertyName).GetValue(x));
                return (int)result;
            }
            else if (firstValue is sbyte)
            {
                var result = list.Average(x => (sbyte)x.GetType().GetProperty(propertyName).GetValue(x));
                return (int)result;
            }
        }
        return 0;
    }

    private int CalculateSumInTableColumn<T>(List<T> list, string propertyName)
    {
        var firstValue = list.FirstOrDefault().GetType().GetProperty(propertyName).GetValue(list.FirstOrDefault());

        if (ValueIsNumber(firstValue))
        {
            if (firstValue is int)
            {
                var result = list.Sum(x => (int)x.GetType().GetProperty(propertyName).GetValue(x));
                return (int)result;
            }
            else if (firstValue is double)
            {
                var result = list.Sum(x => (double)x.GetType().GetProperty(propertyName).GetValue(x));
                return (int)result;
            }
            else if (firstValue is decimal)
            {
                var result = list.Sum(x => (double)x.GetType().GetProperty(propertyName).GetValue(x));
                return (int)result;
            }
            else if (firstValue is float)
            {
                var result = list.Sum(x => (float)x.GetType().GetProperty(propertyName).GetValue(x));
                return (int)result;
            }
            else if (firstValue is ulong)
            {
                var result = list.Sum(x => (float)x.GetType().GetProperty(propertyName).GetValue(x));
                return (int)result;
            }
            else if (firstValue is long)
            {
                var result = list.Sum(x => (long)x.GetType().GetProperty(propertyName).GetValue(x));
                return (int)result;
            }
            else if (firstValue is uint)
            {
                var result = list.Sum(x => (uint)x.GetType().GetProperty(propertyName).GetValue(x));
                return (int)result;
            }
            else if (firstValue is ushort)
            {
                var result = list.Sum(x => (ushort)x.GetType().GetProperty(propertyName).GetValue(x));
                return (int)result;

            }
            else if (firstValue is short)
            {
                var result = list.Sum(x => (short)x.GetType().GetProperty(propertyName).GetValue(x));
                return (int)result;

            }
            else if (firstValue is byte)
            {
                var result = list.Sum(x => (byte)x.GetType().GetProperty(propertyName).GetValue(x));
                return (int)result;

            }
            else if (firstValue is sbyte)
            {
                var result = list.Sum(x => (sbyte)x.GetType().GetProperty(propertyName).GetValue(x));
                return (int)result;

            }
        }
        return 0;
    }


    private bool ValueIsNumber(object value)
    {
        return value is sbyte
               || value is byte
               || value is short
               || value is ushort
               || value is int
               || value is uint
               || value is long
               || value is ulong
               || value is float
               || value is double
               || value is decimal;
    }

    private int CalculateRowsInTableColumn<T>(List<T> list, string propertyName)
    {
        var count = 0;
        foreach (var item in list)
        {
            var value = item.GetType().GetProperty(propertyName).GetValue(item);
            if (value != null)
            {
                if (value is bool)
                {
                    var propertyValue = (bool)value;
                    if (propertyValue)
                    {
                        count++;
                    }
                }
                else if (value is string)
                {
                    var propertyValue = (string)value;
                    if (!string.IsNullOrWhiteSpace(propertyValue))
                    {
                        count++;
                    }
                }
                else
                {
                    count++;
                }
            }
        }
        return count;
    }

    //public static T MapStoredColumnValues<T>(T tableColumn, List<ColumnSetting> columnSettings)
    //{
    //    try
    //    {
    //        foreach (var setting in columnSettings)
    //        {
    //            var property = tableColumn.GetType().GetProperty(setting.Type);
    //            var isNumber = int.TryParse(setting.Value, out int resultNumber);
    //            var isBool = bool.TryParse(setting.Value, out bool resultBool);
    //            if (property != null)
    //            {
    //                if (isNumber)
    //                {
    //                    property.SetValue(tableColumn, resultNumber);
    //                }
    //                else if (isBool)
    //                {
    //                    property.SetValue(tableColumn, resultBool);
    //                }
    //                else
    //                {
    //                    property.SetValue(tableColumn, setting.Value);
    //                }
    //            }
    //        }
    //        return tableColumn;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new ApplicationException($"Exception in map row/column {ex.Message}");
    //    }
    //}

    //public static UserTableSettings MapEntityTOUserTableSettings<T>(Guid tableName, UserTableSettings userTableSettings)
    //{
    //    var tableColumns = GetTableColumnHeadings<T>();
    //    List<Column> columns = new List<Column>();
    //    foreach (var tablecolumn in tableColumns.OrderBy(x => x.ColumnId))
    //    {
    //        Column column = new Column();
    //        List<ColumnSetting> columnSettings = new List<ColumnSetting>();
    //        foreach (var property in tablecolumn.GetType().GetProperties().OrderBy(x => x.Name))
    //        {
    //            var attr = property.GetCustomAttributes(true);

    //            if (attr.FirstOrDefault() is TableColumnAttribute)
    //            {
    //                var tableColumnAttribute = attr.FirstOrDefault() as TableColumnAttribute;
    //                if (tableColumnAttribute.GetType().GetProperty("IsTableSetting").GetValue(tableColumnAttribute).ToString().ToLower() == "true" && property.Name != "ColumnHeading")
    //                {
    //                    ColumnSetting columnSetting = new ColumnSetting()
    //                    {
    //                        Type = property.Name,
    //                        Value = property.GetValue(tablecolumn).ToString()
    //                    };
    //                    columnSettings.Add(columnSetting);
    //                }

    //                if (tableColumnAttribute.GetType().GetProperty("IsTableSetting").GetValue(tableColumnAttribute).ToString().ToLower() == "true" && property.Name == "ColumnHeading")
    //                {
    //                    column.ColumnHeading = property.GetValue(tablecolumn).ToString();
    //                    columns.Add(column);
    //                }
    //            }
    //        }
    //        if (columnSettings != null)
    //        {
    //            column.ColumnSettings = new List<ColumnSetting>();
    //            column.ColumnSettings.AddRange(columnSettings);
    //        }
    //    }
    //    var tableSetting = new TableSetting()
    //    {
    //        TableName = tableName,
    //        Columns = columns
    //    };
    //    if (userTableSettings.TableSettings == null)
    //    {
    //        userTableSettings.TableSettings = new List<TableSetting>();
    //    }
    //    userTableSettings.TableSettings.Add(tableSetting);
    //    return userTableSettings;
    //}

    //public async static Task<List<TableColumn>> UpdateWithStoredTableSettings<T>(UserTableSettings userTableSettings, 
    //                                                                             Guid tableName, 
    //                                                                             List<TableColumn> tableColumns, 
    //                                                                             AuthenticationStateProvider authenticationState, 
    //                                                                             IAppSettingRepository appSettingRepository)
    //{
    //    if (userTableSettings?.UserTableSettingsId != Guid.Empty)
    //    {
    //        if (userTableSettings?.TableSettings.FirstOrDefault(x => x.TableName == tableName) != null)
    //        {
    //            foreach (var column in userTableSettings.TableSettings.FirstOrDefault(x => x.TableName == tableName).Columns)
    //            {
    //                var tableColumn = tableColumns.FirstOrDefault(x => x.ColumnHeading == column.ColumnHeading);
    //                tableColumn = MapStoredColumnValues<TableColumn>(tableColumn, column.ColumnSettings);
    //            }
    //        }
    //        else
    //        {
    //            userTableSettings.MapEntityTOUserTableSettings<T>(tableName);
    //            await userTableSettings.SaveTableSetting<T>(tableName, userTableSettings, authenticationState, appSettingRepository);
    //        }

    //    }
    //    else
    //    {
    //        userTableSettings.MapEntityTOUserTableSettings<T>(tableName);
    //        await userTableSettings.SaveTableSetting<T>(tableName, userTableSettings, authenticationState, appSettingRepository);
    //    }
    //    return tableColumns;
    //}

    //public List<T> SortOnColumn<T>(List<T> list)
    //{
    //    Console.WriteLine("In sort");
    //    if (list != null && list.Count > 1)
    //    {
    //        Console.WriteLine("sort > 1");
    //        if (SortDirection == "bottom")
    //        {
    //            Console.WriteLine("sort bottom");
    //            list = list.OrderBy(x => x.GetType().GetProperty(PropertyName).GetValue(x, null)).ToList();
    //            SortDirection = "top";
    //        }
    //        else
    //        {
    //            Console.WriteLine("sort top");
    //            list = list.OrderByDescending(x => x.GetType().GetProperty(PropertyName).GetValue(x, null)).ToList();
    //            SortDirection = "bottom";
    //        } 
    //    }
    //    return list;
    //}

    //public void ShowSortArrowInHeading(List<TableColumn> tableColumns)
    //{
    //    var oldColumn = tableColumns.FirstOrDefault(x => x.ShowSortArrow == "visible");
    //    if (oldColumn != null)
    //    {
    //        oldColumn.ShowSortArrow = "hidden";
    //    }
    //    var newColumn = tableColumns.FirstOrDefault(x => x.ColumnId == ColumnId);
    //    newColumn.ShowSortArrow = "visible";
    //}
}