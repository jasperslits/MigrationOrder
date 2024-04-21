namespace MigrationOrder.Logic;
using MigrationOrder.Models;

public class AnalyzePayperiod
{

    public List<PayPeriodGcc> payPeriodGccs { get; set; } = new();
    private List<PayPeriod> _lp { get; set; } = new();

    public AnalyzePayperiod(List<PayPeriod> lp)
    {
        _lp = lp;
    }

    public (DateTime,DateTime) Propose(DateTime open,DateTime close) {
      
        DateTime dayone = open;
        DateTime daytwo = dayone.AddDays(1);

        if (open.DayOfWeek == DayOfWeek.Saturday) {
       //     Console.WriteLine($"Date {open} is a Saturday");
            dayone = open.AddDays(2);
            daytwo = dayone.AddDays(1);
        }
        if (open.DayOfWeek == DayOfWeek.Sunday) {
       //     Console.WriteLine($"Date {open} is a Sunday");
             dayone = open.AddDays(1);
            daytwo = dayone.AddDays(1);
        }
        if (open.DayOfWeek == DayOfWeek.Friday) {
         //   Console.WriteLine($"Date {open} is a Friday");
            dayone = open;
            daytwo = dayone.AddDays(3);
        }
        if (open.DayOfWeek == DayOfWeek.Thursday) {
          //  Console.WriteLine($"Date {open} is a Thursday");
            dayone = open;
            daytwo = dayone.AddDays(1);
        }




        return (dayone,daytwo);
    }

    public List<PayPeriodGcc> FindData(string Gcc, int bucket) {
        List<PayPeriodGcc> ppg = new();

        DateTime TargetPeriodStart = DateTime.Now.AddMonths(bucket);
        TargetPeriodStart = new DateTime(TargetPeriodStart.Year, TargetPeriodStart.Month, 1);
        DateTime TargetPeriodEnd = TargetPeriodStart.AddMonths(1).AddSeconds(-1);
        List<PayPeriod> res = _lp.Where(x => x.Gcc == Gcc).Where(x => x.Open > TargetPeriodStart && x.Open < TargetPeriodEnd).OrderBy(x => x.Open).DistinctBy(x => x.PayGroup).ToList();
        foreach(PayPeriod f in res) {     
       
        var (dayone,daytwo) = Propose(f.Open,f.Close);
        if (Gcc == "RKT" && f.PayGroup == "S0") {
               
            Console.WriteLine($"Open = {f.Open} ({dayone.DayOfWeek}). Day {dayone} ({dayone.DayOfWeek}), Day two {daytwo} ({daytwo.DayOfWeek}). Bucket {bucket}"); 
        }
         var _ppg = new PayPeriodGcc {
                    Gcc = Gcc,
                    PayGroup = f.PayGroup,
                    Dayone = dayone,
                    Daytwo = daytwo
                };
            ppg.Add(_ppg);
        }
        return ppg;
    }

    public PayPeriodGcc FindBest(List<PayPeriodGcc> ppg) {
        return ppg.Last();
    }

    public void FindSlot(List<PayPeriod> lp) {
       
        var grouped = lp.GroupBy(x => x.Gcc);
        foreach (IGrouping<string, PayPeriod> group in grouped) {
            List<PayPeriod> list = group.ToList();
            foreach(PayPeriod p in list) {
               var (dayone,daytwo) = Propose(p.Open,p.Close);
                var ppg = new PayPeriodGcc {
                    Gcc = p.Gcc,
                    PayGroup = p.PayGroup,
                    Dayone = dayone,
                    Daytwo = daytwo
                };

                payPeriodGccs.Add(ppg);

            }
        }
            

        }  
    
}