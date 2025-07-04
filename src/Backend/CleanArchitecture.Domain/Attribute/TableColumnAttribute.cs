using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Domain.Attribute;

public class TableColumnAttribute : System.Attribute
{
    public TableColumnAttribute(string columnHeading,
       int columnId,
       int headerOrder = 0,
       bool initialSort = false,
       TagType tagType = TagType.Text,
       bool nestedTable = false,
       bool calculateReport = false,
       ReportType reportType = ReportType.None,
       bool isTableSetting = false,
       bool showInHeader = false,
       bool enableInHeader = false,
       bool showInRollToRollHeading = false,
       bool showInRollToSheetHeading = false,
       bool showInSheetToSheetHeading = false,
       bool showInRollToCoilHeading = false)
    {
        this.columnHeading = columnHeading;
        this.columnId = columnId;
        this.initialSort = initialSort;
        this.tagType = tagType;
        this.nestedTable = nestedTable;
        this.calculateReport = calculateReport;
        this.reportType = reportType;
        this.isTableSetting = isTableSetting;
        this.showInHeader = showInHeader;
        this.headerOrder = headerOrder;
        this.enableInHeader = enableInHeader;
        this.showInRollToRollHeading = showInRollToRollHeading;
        this.showInRollToSheetHeading = showInRollToSheetHeading;
        this.showInSheetToSheetHeading = showInSheetToSheetHeading;
        this.showInRollToCoilHeading = showInRollToCoilHeading;
    }

    private string columnHeading;

    public string ColumnHeading
    {
        get { return columnHeading; }
        set { columnHeading = value; }
    }

    private int columnId;

    public int ColumnId
    {
        get { return columnId; }
        set { columnId = value; }
    }

    private bool initialSort;

    public bool InitialSort
    {
        get { return initialSort; }
        set { initialSort = value; }
    }

    private bool nestedTable;

    public bool NestedTable
    {
        get { return nestedTable; }
        set { nestedTable = value; }
    }


    private TagType tagType;

    public TagType TagType
    {
        get { return tagType; }
        set { tagType = value; }
    }

    private bool calculateReport;

    public bool CalculateReport
    {
        get { return calculateReport; }
        set { calculateReport = value; }
    }

    private ReportType reportType;

    public ReportType ReportType
    {
        get { return reportType; }
        set { reportType = value; }
    }

    private bool isTableSetting;

    public bool IsTableSetting
    {
        get { return isTableSetting; }
        set { isTableSetting = value; }
    }

    private bool showInHeader;

    public bool ShowInHeader
    {
        get { return showInHeader; }
        set { showInHeader = value; }
    }

    private int headerOrder;

    public int HeaderOrder
    {
        get { return headerOrder; }
        set { headerOrder = value; }
    }

    private bool enableInHeader;

    public bool EnableInHeader
    {
        get { return enableInHeader; }
        set { enableInHeader = value; }
    }

    private bool showInRollToRollHeading;

    public bool ShowInRollToRollHeading
    {
        get { return showInRollToRollHeading; }
        set { showInRollToRollHeading = value; }
    }

    private bool showInRollToSheetHeading;

    public bool ShowInRollToSheetHeading
    {
        get { return showInRollToSheetHeading; }
        set { showInRollToSheetHeading = value; }
    }

    private bool showInSheetToSheetHeading;

    public bool ShowInSheetToSheetHeading
    {
        get { return showInSheetToSheetHeading; }
        set { showInSheetToSheetHeading = value; }
    }

    private bool showInRollToCoilHeading;

    public bool ShowInRollToCoilHeading
    {
        get { return showInRollToCoilHeading; }
        set { showInRollToCoilHeading = value; }
    }

   
}