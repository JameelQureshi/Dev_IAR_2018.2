using System;
using System.Collections.Generic;

public class ARReportInfo
{
    public string message;
    public string response;
    public ARReportInfo(string head, string val)
    {
        this.message = head;
        this.response = val;
    }
}

public class ARReportInfo1
{
    public List<ColumnValue> rowDetails;
    public ARReportInfo1()
    {

    }
    public ARReportInfo1(List<ColumnValue> rdetail)
    {
        this.rowDetails = rdetail;
    }
}

public class ColumnValue
{
    public string columnName;
    public string columnValue;
    public ColumnValue(string colName, string colValue)
    {
        this.columnName = colName;
        this.columnValue = colValue;
    }
}
