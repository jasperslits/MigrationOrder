namespace MigrationOrder.Models;

public class MigrationConfig
{

    public readonly static int Month = 8;

    public readonly static int NrPeriods = 5;

    public readonly static bool IncludeDistribution = true;
    public readonly static bool IncludeStats = true;
    public readonly static bool IncludeGcc = true;
    public readonly static bool IncludeParams = true;

    public readonly static Enums.BucketFill DistributionOrder = Enums.BucketFill.Horizontal;

    public readonly static string ExcelFilename = "MigrationPlanning.xlsx";

    public readonly static string datadumpfile = "full_gcc.csv";
    public readonly static string paygroupfile = "full_pg.csv";

    public readonly static int LowerStat = 8;
    public readonly static int UpperStat = 50;

    public readonly static string OnlyGcc = "";

    public static Dictionary<string,int> Predefined() {
        Dictionary<string,int> Offsets = new()
        {
            { "ING", 2 },
            { "RAB", 3 }
        };
        return Offsets;

    }
}