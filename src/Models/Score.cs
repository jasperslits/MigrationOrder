namespace MigrationOrder.Models;

using MigrationOrder.Enums;

public static class ScoreWeights {
    public static double HCWeight = 1.0;
    public static double CountryWeight = 0.8;
    public static double EventWeight = 1.0;
    public static double LccWeight = 1.0;

    public static double DocWeight = 1.0;

    public static double PayPeriodWeight = 1.0;

}


public class Score {
    
    public string Gcc {get;set;}

    public string GccName {get;set;}

    public ComplexityType Headcount {get; set;} = ComplexityType.Undefined;

    public ComplexityType Countrycount {get; set;} = ComplexityType.Undefined;

    public ComplexityType Eventcount {get; set;} = ComplexityType.Undefined;

    public ComplexityType LccCount {get; set;} = ComplexityType.Undefined;

    public ComplexityType DocCount {get; set;} = ComplexityType.Undefined;

    public ComplexityType PayPeriodCount {get; set;} = ComplexityType.Undefined;

    public double Total {get;set;}

    public void Calculate() {
        Total = (double)Headcount * ScoreWeights.HCWeight + 
        (double)Countrycount * ScoreWeights.CountryWeight + 
        (double)Eventcount * ScoreWeights.EventWeight + 
        (double)LccCount * ScoreWeights.LccWeight + 
        (double)PayPeriodCount * ScoreWeights.PayPeriodWeight;
    }

}