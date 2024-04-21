namespace MigrationOrder.Models;

public class MigrationConfig
{

    public readonly static int Month = 4;

    public readonly static int NrPeriods = 6;

    public readonly static bool IncludeDistribution = true;
    public readonly static bool IncludeStats = true;
    public readonly static bool IncludeGcc = true;
    public readonly static bool IncludeParams = true;

    public readonly static string ExcelFilename = "MigrationPlanning.xlsx";

    public readonly static string datadumpfile = "full_gcc.csv";
    public readonly static string paygroupfile = "full_pg.csv";

    public readonly static string OnlyGcc = "RKT";

    public static Dictionary<string,int> Predefined() {
        Dictionary<string,int> keyValuePairs = new();
        keyValuePairs.Add("ING",2);
        keyValuePairs.Add("RAB",5);
        return keyValuePairs;

    }
}