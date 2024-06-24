using MigrationOrder.Enums;

namespace MigrationOrder.Models;

public class PayPeriodGcc {

    public string Gcc { get; set; }

    public string PayGroup { get; set; }

    public DateTime CutOff {get;set;}
    public DateTime PayDate {get;set;}
    public DateTime Open {get;set;}
    public DateTime Close {get;set;}
}

public class ProposedDates {

    public string Gcc { get; set;}
    public DateTime DayOne {get;set;}
    public DateTime DayTwo {get;set;}

    public int ScoreDayOne {get; set;}

    public int ScoreDayTwo {get; set;}

    public double ScoreDayOnePercent {get; set;}

    public double ScoreDayTwoPercent {get; set;}

    public Strategy Usedstrategy{ get; set; }

}