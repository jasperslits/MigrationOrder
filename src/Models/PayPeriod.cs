namespace MigrationOrder.Models;

using CsvHelper.Configuration.Attributes;

public class PayPeriod
{
    [Index(0)]
    public string Gcc { get; set; }
    [Index(1)]
    public string Lcc { get; set; }
    [Index(2)]
    public string PayGroup { get; set; }
    [Index(3)]
    public DateTime Open { get; set; }
    [Index(4)]
    public DateTime Close { get; set; }
    [Index(4)]
    public DateTime CutOff { get; set; }
    [Index(5)]
    public DateTime PayDate { get; set; }

}