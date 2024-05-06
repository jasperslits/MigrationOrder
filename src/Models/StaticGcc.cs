namespace MigrationOrder.Models;

public struct StaticGcc {
    
    public string Gcc {get;set;}

    public string GccName {get;set;}

    public int Month {get;set;}
}

public class StaticGCCs {

    public static List<StaticGcc> GetStatic() {
            List<StaticGcc> Static = new() {
            new StaticGcc { Gcc = "RDS", GccName = "Shell", Month = 4 },
            new StaticGcc { Gcc = "NIQ", GccName = "NielsenIQ/GFK", Month = 4 },
            new StaticGcc { Gcc = "EXX", GccName = "Exxon", Month = 5 },
            new StaticGcc { Gcc = "ALA",GccName = "Alsa", Month = 6},
            new StaticGcc { Gcc = "GSK",GccName = "GSK", Month = 6},
            new StaticGcc { Gcc = "GEA",GccName = "GE Aerospace", Month = 6},
            new StaticGcc { Gcc = "GEV",GccName = "GE Vernova", Month = 6}
        };

        return Static;

    }

}