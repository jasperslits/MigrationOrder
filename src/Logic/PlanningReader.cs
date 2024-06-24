namespace MigrationOrder.Logic;

using MigrationOrder.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style.XmlAccess;
public class PLanningReader {

    private List<Planning> plannings {get;set;} = new();

    public List<Planning> GetPlanning() {
        return plannings;
    }

    public PLanningReader() {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        var package = new ExcelPackage(new FileInfo("src/Data/Input/Planning2.xlsx"));
        ExcelWorksheet sheet = package.Workbook.Worksheets[0];
     
        int rowCount = sheet.Dimension.End.Row;     //get row count
    
        for (int row = 2; row <= rowCount; row++)
        {
          int month = DateTime.FromOADate(double.Parse(sheet.Cells[row,1].Value.ToString())).Month - MigrationConfig.Month;
          plannings.Add(new Planning{ Gcc = sheet.Cells[row,2].Value.ToString(), Month = month});

            
        }

    }
}