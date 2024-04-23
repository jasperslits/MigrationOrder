namespace MigrationOrder.Logic;

using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using MigrationOrder.Models;
using OfficeOpenXml;

public class Excel
{
    private string Gcc;

    private string GccName;
    public List<Coordinates> c { get; set; }
    private ExcelWorksheet Ws { get; set; }
    private ExcelPackage Package { get; set; }

    public Excel()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        Package = new();

    }

    public void Init(string gcc, string gccName, int calendarMonth)
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

    public void Distribution(Dictionary<int, List<Score>> dist)
    {
        Ws = Package.Workbook.Worksheets.Add("Distribution");
        Package.Workbook.Worksheets.MoveToStart("Distribution");

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
        foreach (var d in dist)
        {

            foreach (Score s in d.Value)
            {
                Ws.Cells[row, 1].Value = monthNames[monthOffset + d.Key - 1];
                Ws.Cells[row, 2].Value = s.Gcc;
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
        if (MigrationConfig.OnlyGcc.Length > 0 && Gcc != MigrationConfig.OnlyGcc)
        {
            return;
        }


        c.RemoveAll(x => x.Value.Length == 0);

        if (MigrationConfig.OnlyGcc.Length > 0 && Gcc == MigrationConfig.OnlyGcc)
        {
            Debug.Assert(c.Count > 0, $"2. No PG found met {c.Count}");
        }
        foreach (Coordinates c1 in c)
        {
            var w = Ws.Cells[c1.X, c1.Y];
            w.Style.Font.Bold = true;
            w.Value = w.Value + "," + c1.Value.Remove(c1.Value.Length - 1, 1);
            w.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            w.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
        }
        Ws.Cells["A3:G7"].EntireColumn.Width = 18;
        Ws.Cells["A3:G7"].EntireRow.Height = 60;
        Ws.Cells["A3:G7"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

        Ws.Cells["A3:G7"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        Ws.Cells["A3:G7"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        Ws.Cells["A3:G7"].Style.WrapText = true;
        c.Clear();
    }

    public void Save()
    {
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

        Ws.Cells[1, 1].Value = $"Calendar for {monthname} for {Gcc} ({GccName})";
        Ws.Cells[1, 1].Style.Font.Size = 16;
        Ws.Cells["A2:G2"].Style.Font.Size = 16;

    }


    public void CreateCalendar(int month)
    {
        if (MigrationConfig.OnlyGcc.Length > 0 && Gcc != MigrationConfig.OnlyGcc)
        {
            return;
        }

        var dt = DateTime.Now;
        // int month = new DateTime(dt.Year, MigrationConfig.Month, 1).AddMonths(offset).Month;
        DateTime firstDayOfMonth = new(dt.Year, month, 1);
        Header(firstDayOfMonth);

        DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddSeconds(-1);
        int nr = lastDayOfMonth.Day;
        int col = 0;
        c = new List<Coordinates>(nr);

        int row = 1;
        for (int i = 0; i < nr; i++)
        {
            dt = firstDayOfMonth.AddDays(i);
            if (dt.DayOfWeek == DayOfWeek.Monday) { col = 1; }
            if (dt.DayOfWeek == DayOfWeek.Tuesday) { col = 2; }
            if (dt.DayOfWeek == DayOfWeek.Wednesday) { col = 3; }
            if (dt.DayOfWeek == DayOfWeek.Thursday) { col = 4; }
            if (dt.DayOfWeek == DayOfWeek.Friday) { col = 5; }
            if (dt.DayOfWeek == DayOfWeek.Saturday) { col = 6; }
            if (dt.DayOfWeek == DayOfWeek.Sunday) { col = 7; }

            // Ws.Cells
            Ws.Cells[row + 2, col].Value = dt.Day;
            c.Insert(i, new Coordinates { X = row + 2, Y = col });
            if (dt.DayOfWeek == DayOfWeek.Sunday)
            {
                row++;
            }
        }
    }
}