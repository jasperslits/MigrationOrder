namespace MigrationOrder.Models;
using CsvHelper.Configuration.Attributes;
public class DataDump {
    [Index(0)]
    public string Gcc {get;set;}
    [Index(1)]
    public int Documents {get;set;}
    [Index(2)]
    public int BODs {get;set;}
    [Index(3)]
    public int Users {get;set;}

     public int LCCs { get; set; }

    public int Countries { get; set; }

    public int PayGroups { get; set; }

    public Dictionary<String,int> Features {get;set;}

    public string GccName {get;set;}

}