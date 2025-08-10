using ClosedXML.Excel;
using CleanArchitecture.Infrastructure.ExternalServices.Excel.ExcelModels;

namespace CleanArchitecture.Infrastructure.ExternalServices.Excel
{
    public class WorkBook : IWorkBook
    {
        public WorkBook()
        {
            xlWorkBook = new XLWorkbook();
            xlWorkSheet = xlWorkBook.Worksheets.Add("EventSphere Excel report");
        }

        private IXLWorkbook xlWorkBook;
        private IXLWorksheet xlWorkSheet;

        public string Create(ExcelList list)
        {
            string path = $@"c:\eventSphere\excel\";
            Directory.CreateDirectory(path);
            path = Path.Combine(path, $"dataExport{Guid.NewGuid()}.xlsx");

            WriteToCells(list);

            AddWorkBookStyle(list);

            xlWorkBook.SaveAs(path);
            return path;
        }

        private void AddWorkBookStyle(ExcelList list)
        {
            SetStyleOnHeading(1, 1, list.Headings.Count(), XLColor.LightBlue, true, XLBorderStyleValues.Thin, XLColor.Black);
            AdjustColumnSizeToContent(list);
            xlWorkSheet.SheetView.FreezeRows(1);
        }

        private void AdjustColumnSizeToContent(ExcelList list)
        {
            var firstColumn = xlWorkSheet.FirstColumnUsed().ColumnNumber();
            var lastColumn = xlWorkSheet.LastColumnUsed().ColumnNumber();
            xlWorkSheet.Columns(firstColumn, lastColumn).AdjustToContents();
        }

        private void SetStyleOnHeading(int row, int firstCell, int lastCell, XLColor cellColor, bool fontWeigth, XLBorderStyleValues borderStyle, XLColor borderColor)
        {
            var range = xlWorkSheet.Range(row, firstCell, row, lastCell);
            range.Style.Fill.BackgroundColor = cellColor;
            range.Style.Font.Bold = fontWeigth;
            range.Style.Border.BottomBorder = borderStyle;
            range.Style.Border.BottomBorderColor = borderColor;
        }

        private void WriteToCells(ExcelList list)
        {
            bool nestedTable = false;
            var headings = list.Headings.OrderBy(x => x.ColumnId).ToList();
            for (int i = 0; i < headings.Count; i++)
            {
                xlWorkSheet.Cell(1, i + 1).Value = headings[i].HeadingName;
            }
            var jRow = 0;
            for (int j = 0; j < list.ExcelRows.Count; j++)
            {
                ExcelRow row = list.ExcelRows[j];
                jRow = 1 + jRow;
                var values = row.Cells.Where(x => x.Value != null).OrderBy(x => x.ColumnId).ToList();
                var nestedTables = row.Cells.Where(x => x.NestedTable != null).ToList();
                for (int i = 0; i < values.Count(); i++)
                {
                    if (j == 0 && jRow == 1)
                    {
                        jRow++;
                        xlWorkSheet.Cell(jRow, values[i].ColumnId).Value = values[i].Value;
                    }
                    else
                    {
                        xlWorkSheet.Cell(jRow, values[i].ColumnId).Value = values[i].Value;
                    }
                }
                if (nestedTables.Count > 0)
                {
                    nestedTable = true;
                    jRow++;
                    for (int i = 0; i < nestedTables.Count(); i++)
                    {
                        jRow = WriteNestedTableInCells(nestedTables[i].NestedTable, jRow);
                    }
                }

            }
            if (nestedTable)
            {
                //worksheet.Columns["A:A"].Delete();
            }

        }

        private int WriteNestedTableInCells(ExcelList nestedTable, int jRow)
        {
            var firstRow = jRow;

            foreach (var heading in nestedTable.Headings.OrderBy(x => x.ColumnId))
            {
                xlWorkSheet.Cell(jRow, heading.ColumnId + 1).Value = heading.HeadingName;
            }

            SetStyleOnHeading(jRow, nestedTable.Headings.Min(x => x.ColumnId) + 1, nestedTable.Headings.Max(x => x.ColumnId) + 1, XLColor.FromArgb(216, 228, 188), true, XLBorderStyleValues.Thin, XLColor.Black);

            for (int j = 0; j < nestedTable.ExcelRows.Count; j++)
            {
                jRow = jRow + 1;
                ExcelRow row = nestedTable.ExcelRows[j];
                var values = row.Cells.Where(x => x.Value != null).OrderBy(x => x.ColumnId).ToList();
                var nestedTables = row.Cells.Where(x => x.NestedTable != null).ToList();
                for (int i = 0; i < values.Count(); i++)
                {
                    xlWorkSheet.Cell(jRow, values[i].ColumnId + 1).Value = values[i].Value;
                }
                if (nestedTables.Count > 0)
                {
                    jRow++;
                    for (int i = 0; i < nestedTables.Count(); i++)
                    {
                        jRow = WriteNestedTableInCells(nestedTables[i].NestedTable, jRow);
                    }
                }
            }
            var lastRow = jRow;
            SetStyleOnNestedTableRows(nestedTable, firstRow, lastRow);
            return jRow;
        }

        private void SetStyleOnNestedTableRows(ExcelList nestedTable, int firstRow, int lastRow)
        {
            xlWorkSheet.Rows(firstRow, lastRow).Group();
            xlWorkSheet.Rows(firstRow, lastRow).Collapse();
            xlWorkSheet.Range(lastRow, nestedTable.Headings.Min(x => x.ColumnId), lastRow, nestedTable.Headings.Max(x => x.ColumnId) + 1).Style.Border.BottomBorder = XLBorderStyleValues.Double;
            xlWorkSheet.Range(firstRow, nestedTable.Headings.Min(x => x.ColumnId), lastRow, nestedTable.Headings.Min(x => x.ColumnId)).Style.Fill.BackgroundColor = XLColor.FromArgb(238, 236, 225);
            xlWorkSheet.Range(firstRow, nestedTable.Headings.Min(x => x.ColumnId), firstRow, nestedTable.Headings.Min(x => x.ColumnId)).Style.Border.TopBorder = XLBorderStyleValues.Dotted;
            xlWorkSheet.Range(firstRow, nestedTable.Headings.Min(x => x.ColumnId), lastRow, nestedTable.Headings.Min(x => x.ColumnId)).Style.Border.RightBorder = XLBorderStyleValues.Dotted;
        }
        
    }
}
