namespace MigrationOrder.Logic;

using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using MigrationOrder.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style.XmlAccess;

public class Excel
{
    private string Gcc;

    private string GccName;
    public List<PayPeriodGcc> C { get; set; }
    private ExcelWorksheet Ws { get; set; }
    private ExcelPackage Package { get; set; }

    private ExcelNamedStyleXml FormattedStyle {get;set;}

    private int CalRowCount { get; set; }

    public Excel()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        Package = new();
        C = new();

    }

    public void Init(string gcc, string gccName, DateTime calendarMonth)
    {
        Gcc = gcc;
        GccName = gccName;
      
        CreateCalendar(calendarMonth);
    }

    public void Parameters(List<string> ls)
    {
        Ws = Package.Workbook.Worksheets.Add("Parameters");

        string[] fields;
        int rowNr = 3;
        int colNr;
        foreach (string row in ls)
        {
            fields = row.Split(";");
            colNr = 1;
            foreach (string field in fields)
            {
                Ws.Cells[rowNr, colNr].Value = field;
                colNr++;
            }
            rowNr++;
        }

        Ws.Cells[1, 1].Value = "Parameters";
        Ws.Cells[1, 1].Style.Font.Size = 16;
        FormatTable(Ws.Cells["A3:E3"]);
        Ws.Cells["A3:E3"].Style.Font.Bold = true;
        Ws.Cells["A1:E3"].EntireColumn.Width = 18;
        Ws.Cells["A3:E9"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
    }

    public void Statistics(List<string> ls)
    {
        Ws = Package.Workbook.Worksheets.Add("Statistics");
        string[] fields;
        int rowNr = 3;
        int colNr;
        int FieldValue;
        foreach (string row in ls)
        {
            fields = row.Split(";");
            colNr = 1;
            foreach (string field in fields)
            {
                Ws.Cells[rowNr, colNr].Value = field;
                if (colNr > 2 && field.Length <= 2) { 
                    FieldValue = Int32.Parse(field);
                    if (FieldValue <  MigrationConfig.LowerStat || FieldValue >  MigrationConfig.UpperStat ) {
                        Ws.Cells[rowNr, colNr].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    }
                }
                colNr++;
            }
            rowNr++;
        }

        Ws.Cells[1, 1].Value = "Statistics";
        Ws.Cells[1, 1].Style.Font.Size = 16;
        Ws.Cells["A3:I3"].Style.Font.Bold = true;
        FormatTable(Ws.Cells["A3:D3"]);
        Ws.Cells["A3:D7"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        Ws.Cells["A1:D3"].EntireColumn.Width = 18;
    }

    private void FormatLink() {
        var namedStyle = Package.Workbook.Styles.CreateNamedStyle("HyperLink");  
        namedStyle.Style.Font.UnderLine = true;
        namedStyle.Style.Font.Color.SetColor(System.Drawing.Color.Blue);
        FormattedStyle = namedStyle;
    }

    public void Distribution(Dictionary<int, List<Score>> dist)
    {
        Ws = Package.Workbook.Worksheets.Add("Distribution");
        Package.Workbook.Worksheets.MoveToStart("Distribution");
        FormatLink();
        string[] monthNames = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;

        int col = 1;
        string[] fields = { "Month", "GCC", "GCC Name", "Total score", "Headcount", "Docs", "LCC", "Pay group", "Country", "Events" };
        foreach (var field in fields)
        {
            Ws.Cells[3, col].Value = field;
            col++;
        }
        int row = 4;
        int monthOffset = MigrationConfig.Month;

        foreach(StaticGcc g in StaticGCCs.GetStatic()) {
            Ws.Cells[row, 1].Value = monthNames[g.Month-1];
            Ws.Cells[row, 2].Value = g.Gcc;
            Ws.Cells[row, 3].Value = g.GccName;
            Ws.Cells[row, 4].Value = "N/A (Manually added)";
            row++;
        }

        int monthKey;

        foreach (var d in dist)
        {
            foreach (Score s in d.Value)
            {
               
                monthKey = monthOffset + d.Key - 1;
                if (monthKey == 12) monthKey = 0;

                Ws.Cells[row, 1].Value = monthNames[monthKey];
                Ws.Cells[row, 2].Value = s.Gcc;
                Ws.Cells[row, 2].Hyperlink = new Uri($"#'{s.Gcc}'!A1", UriKind.Relative);
                Ws.Cells[row, 2].StyleName = "HyperLink";
                Ws.Cells[row, 3].Value = s.GccName;
                Ws.Cells[row, 4].Value = s.Total;
                Ws.Cells[row, 5].Value = s.Headcount;
                Ws.Cells[row, 6].Value = s.DocCount;
                Ws.Cells[row, 7].Value = s.LccCount;
                Ws.Cells[row, 8].Value = s.PayPeriodCount;
                Ws.Cells[row, 9].Value = s.Countrycount;
                Ws.Cells[row, 10].Value = s.Eventcount;
                row++;
            }
        }

        FormatTable(Ws.Cells["A3:J3"]);
        Ws.Cells["A1:J3"].EntireColumn.Width = 18;
        Ws.Cells["C1:C1"].EntireColumn.Width = 30;
        Ws.Cells[1, 1].Value = $"Distribution of GCCs";
        Ws.Cells[1, 1].Style.Font.Size = 16;
        Ws.Cells["A3:J3"].Style.Font.Bold = true;
    }



    // .Style.WrapText = true;

    private void FormatTable(ExcelRange w)
    {
        w.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        w.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkOrange);
        w.Style.Font.Bold = true;
        w.Style.Font.Size = 16;
    }

    public void FillFile()
    {
    

       // C.RemoveAll(x => x.Value.Length == 0);
        Console.WriteLine($"{Gcc} has {C.Count}");
        if (C.Count == 0) {
            var cell = Ws.Cells["A10:A10"];
            cell.Value = $"No payroll calendar found for {Gcc} for selected month";
            cell.Style.Font.Bold = true;
            cell.Style.Font.Size = 16;
            cell.Style.Font.Color.SetColor(System.Drawing.Color.Red);
        }

        int row = 4;
        int dayone = 1;
        int daytwo = 2;
        foreach (PayPeriodGcc c1 in C)
        {
            Ws.Cells[row, 1].Value = c1.PayGroup;
            dayone = c1.Open.Day;
            daytwo = c1.Close.Day;
            if (dayone >= daytwo) {
                Console.WriteLine($" PG = {c1.PayGroup}, from = {dayone} to {daytwo}");
                daytwo = 31;
            }
            
            
            // [FromRow, FromCol, ToRow, ToCol]
            var w = Ws.Cells[row, dayone+1,row,daytwo+1];
           // w.Style.Font.Bold = true;
            w.Value = "";
            w.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            w.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

            row++;
        }
        Ws.Cells["A3:G"+CalRowCount].EntireColumn.Width = 18;
        Ws.Cells["A3:G"+CalRowCount].EntireRow.Height = 60;
        Ws.Cells["A3:G"+CalRowCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

        Ws.Cells["A3:G"+CalRowCount].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        Ws.Cells["A3:G"+CalRowCount].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        Ws.Cells["A3:G"+CalRowCount].Style.WrapText = true;
        C.Clear();
    }

    public void Save()
    {
        Ws = Package.Workbook.Worksheets["Distribution"];
        Ws.View.SetTabSelected();
        Package.SaveAs("src/Data/Output/" + MigrationConfig.ExcelFilename);

        Console.WriteLine($"File written to {MigrationConfig.ExcelFilename} ");
    }

    private void Header(DateTime dt)
    {

        Ws = Package.Workbook.Worksheets.Add(Gcc);
        Ws.Cells[2, 1].Value = "Monday";
        Ws.Cells[2, 2].Value = "Tuesday";
        Ws.Cells[2, 3].Value = "Wednesday";
        Ws.Cells[2, 4].Value = "Thursday";
        Ws.Cells[2, 5].Value = "Friday";
        Ws.Cells[2, 6].Value = "Saturday";
        Ws.Cells[2, 7].Value = "Sunday";
        var w = Ws.Cells[2, 1, 2, 7];
        FormatTable(w);
        var monthname = dt.ToString("MMMM", new System.Globalization.CultureInfo("en-GB"));

        Ws.Cells[1, 1].Value = $"Calendar for {monthname} {dt.Year} for {Gcc} ({GccName})";
        Ws.Cells[1, 1].Style.Font.Size = 16;
        Ws.Cells["A2:G2"].Style.Font.Size = 16;
        Ws.Cells["E1"].Value = "Back";
        Ws.Cells["E1"].Hyperlink = new Uri($"#'Distribution'!A1", UriKind.Relative);
        Ws.Cells["E1"].StyleName = "HyperLink";

    }


    public void CreateCalendar(DateTime period)
    {

        DateTime dt;
       // int month = new DateTime(dt.Year, MigrationConfig.Month, 1).AddMonths(offset).Month;
        DateTime firstDayOfMonth = new(period.Year, period.Month, 1);
        Header(firstDayOfMonth);

        DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddSeconds(-1);
        int nr = lastDayOfMonth.Day;
        int col = 0;
    
        int row = 1;
        for (int i = 1; i <= nr; i++)
        {
            dt = firstDayOfMonth.AddDays(i);
            Ws.Cells[2,i].Value = dt.DayOfWeek;
            Ws.Cells[3,i].Value = i;
      //      C.Insert(i, new Coordinates { X = 2, Y = nr });
          
        }
        CalRowCount = row+=2;
        Console.WriteLine($"GCC = {Gcc}, Cal row count {CalRowCount}");
    } 

  
}