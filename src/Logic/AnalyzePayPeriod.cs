namespace MigrationOrder.Logic;
using MigrationOrder.Models;

public class AnalyzePayperiod
{


    private List<PayPeriod> _lp { get; set; } = new();

    private List<ProposedDates> pd { get; set; } = new();

    public AnalyzePayperiod(List<PayPeriod> lp)
    {
        _lp = lp;
    }

    public List<ProposedDates>  GetProposedDates() {
        return pd;
    }

    private void CheckDate(List<PayPeriodGcc> lp, DateTime proposed) {
        
      List<PayPeriodGcc> res = lp.Where(x => x.Open >= proposed && x.Close <= proposed).OrderBy(x => x.Open).DistinctBy(x => x.PayGroup).ToList();
   //   Console.WriteLine($"Proposed = {proposed}, {res.Count}" );
      foreach(var i in res) {
            Console.WriteLine($"Foo {i.Open}, {i.Close} {proposed}");
        }
    }

    public int FindNextDate(DateTime Dayone, Dictionary<int,int> cal) {
        int day = Dayone.Day + 1;
        for(int i = day;i < cal.Count;i++) {
            if(cal[i] != -1) {
                return i;
            }
        }
        return Dayone.Day;
    }

    public void Propose2(List<PayPeriodGcc> pglist) {
      
        var first = pglist.First();
        string Gcc = first.Gcc;
        DateTime TargetPeriodStart = new DateTime(2024,first.Open.Month,1);
        DateTime TargetPeriodEnd = TargetPeriodStart.AddMonths(1).AddSeconds(-1);
        Dictionary<int,int> cal = new();

        
        for(int i = 1; i <= TargetPeriodEnd.Day;i++) {
            cal[i] = 0;
         
            if (TargetPeriodStart.AddDays(i-1).DayOfWeek == DayOfWeek.Saturday) {
              //  Console.WriteLine("Skipping Saturday");
                cal[i] = -1;
            }
            if (TargetPeriodStart.AddDays(i-1).DayOfWeek == DayOfWeek.Sunday) {
             //   Console.WriteLine("Skipping Sunday");
                cal[i] = -1;
            }
        }
        // Blacklist
        foreach(var p in pglist) {
            cal[p.PayDate.Day] = -1;
            cal[p.PayDate.AddDays(1).Day] = -1;
            cal[p.CutOff.Day] = -1;
        }

        foreach(var i in pglist) {
            var day = i.Open.Day;
            bool FoundSlot = false;
            for(int j = day;j<=TargetPeriodEnd.Day;j++) {
                if (cal[j] != -1) {
                    cal[j] += 4;
            //        Console.WriteLine($"Found slot = {day} for PG = {i.PayGroup}");
                    FoundSlot  = true;
                    break;
                }
            }
            if (! FoundSlot) {
                Console.WriteLine($"{Gcc}: No slot found for {i.PayGroup}. Open {i.Open} en {i.Close}");
            }
        }

        var r = cal.Where(x => x.Value > 0).OrderByDescending(x => x.Value).Take(2);
        foreach(var i in r) {
         
            DateTime Dayone = new DateTime(2024,first.Open.Month,i.Key);
            int d = FindNextDate(Dayone,cal);
            DateTime Daytwo = new DateTime(2024,first.Open.Month,d);
            pd.Add(new ProposedDates{Gcc = Gcc, DayOne = Dayone, DayTwo = Daytwo, Score = i.Value  });
            CheckDate(pglist,Dayone);
        }
      
  
    }



    public List<PayPeriodGcc> FindData(string Gcc, int bucket) {
        List<PayPeriodGcc> ppg = new();

        DateTime TargetPeriodStart = new DateTime(2024,MigrationConfig.Month,1).AddMonths(bucket);
        if (TargetPeriodStart.Month == 12) {
            TargetPeriodStart.AddMonths(1);
        }

      //  TargetPeriodStart = new DateTime(TargetPeriodStart.Year, TargetPeriodStart.Month, 1);
        DateTime TargetPeriodEnd = TargetPeriodStart.AddMonths(1).AddSeconds(-1);
        List<PayPeriod> res = _lp.Where(x => x.Gcc == Gcc).Where(x => x.Open > TargetPeriodStart && x.Open < TargetPeriodEnd).OrderBy(x => x.Open).DistinctBy(x => x.PayGroup).ToList();
        if (res.Count == 0) {
            Console.WriteLine($"No data found for {Gcc} with {TargetPeriodStart.ToString("yyyy-MM-dd")}");
        } else {
    //        Console.WriteLine($"Found for {Gcc} with {TargetPeriodStart.ToString("yyyy-MM-dd")} with bucket {bucket}");
        }
        foreach(PayPeriod f in res) {     
       
      //  var (dayone,daytwo) = Propose(f.Open,f.Close);
       // var (dayone,daytwo) = Propose(f.Open,f.Close);
       DateTime dayone = f.Open;
       DateTime daytwo = f.Close;
         var _ppg = new PayPeriodGcc {
                    Gcc = Gcc,
                    PayGroup = f.PayGroup,
                    CutOff = f.CutOff,
                    PayDate = f.PayDate,
                    Open = f.Open,
                    Close = f.Close
                };
            ppg.Add(_ppg);
        }
        return ppg;
    }

    public PayPeriodGcc FindBest(List<PayPeriodGcc> ppg) {
        return ppg.Last();
    }


    
}