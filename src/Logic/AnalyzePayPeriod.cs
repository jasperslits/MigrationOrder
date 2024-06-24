namespace MigrationOrder.Logic;
using MigrationOrder.Models;
using MigrationOrder.Enums;
using OfficeOpenXml;

public class AnalyzePayperiod
{


    private List<PayPeriod> _lp { get; set; } = new();

    private List<ProposedDates> pd { get; set; } = new();

    public AnalyzePayperiod(List<PayPeriod> lp)
    {
        _lp = lp;
    }

    public double Details(int pgCount, double score) {
       // int maxScore = _lp.Where(x => x.Gcc == Gcc).DistinctBy(x => x.PayGroup).Count()*4;
        int maxScore = pgCount*4;
        double r = (score *1.0 ) / maxScore;
        double res = Math.Round(r*100);
        Console.WriteLine($"Calculate with score = {score} and maxScore = {maxScore},r = {r}");
        return res;
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

    public void Cal(Dictionary<int,int> cal, List<PayPeriodGcc> pg, Strategy s ) {
        int offset = 0;

        var first = pg.First();
        string Gcc = first.Gcc;
        bool FoundSlot = false;
        int ScoreDayOne = 0;
        int ScoreDayTwo = 0;
        
        foreach(var i in pg) {

            if (s == Strategy.FirstDayOpen) {
                offset = i.Open.Day;
            }
            if (s == Strategy.FirstCalDay) {
                offset = 1;
            }
            if (s == Strategy.FirstDayClosed) {
                offset = i.Close.AddDays(1).Day;
            }

            for(int j = offset;j<cal.Count;j++) {
                
                    if (cal[j] >= 0 && cal[j+1] >= 0) {
                        cal[j] += 4;
                        ScoreDayOne +=4;
                        cal[j+1] += 4;
                        ScoreDayTwo +=4;
                   //      Console.WriteLine($"Data found for {Gcc} {i.PayGroup} with strategy {s} using start date {offset}");
                        FoundSlot = true;
                        break;
                    }
            }
            if (!FoundSlot) {
                 Console.WriteLine($"No data found for {Gcc} {i.PayGroup} with strategy {s} using start date {offset}");
            }
        }

          foreach(var x in cal) {
     //       Console.WriteLine($"Day {x.Key}:{x.Value}");
        }

         var r = cal.Where(x => x.Value > 0).OrderByDescending(x => x.Value).Take(2).ToArray();
        if (r.Count() > 0) {
         int dayone = (r[1].Key < r[0].Key) ? r[1].Key : r[0].Key;
          int daytwo = (r[1].Key < r[0].Key) ? r[0].Key : r[1].Key;
            DateTime Dayone = new (2024,first.Open.Month,dayone);
            DateTime Daytwo = new (2024,first.Open.Month,daytwo);
        pd.Add(new ProposedDates{ 
            Gcc = Gcc, 
            DayOne = Dayone, 
            DayTwo = Daytwo, 
            ScoreDayOne = ScoreDayOne,
            ScoreDayTwo = ScoreDayTwo ,
            ScoreDayOnePercent = Details(pg.Count,ScoreDayOne),
            ScoreDayTwoPercent = Details(pg.Count,ScoreDayTwo),
            Usedstrategy = s  
            });        //    CheckDate(pglist,Dayone);
        } else {
            Console.WriteLine($"No data found for {Gcc} with strategy {s} using start date {offset}");
            r = cal.Where(x => x.Value < 0).OrderByDescending(x => x.Value).Take(2).ToArray();
            int dayone = (r[0].Key > r[1].Key) ? r[0].Key : r[1].Key;
            int daytwo = (r[0].Key > r[1].Key) ? r[1].Key : r[0].Key;
            DateTime Dayone = new (2024,first.Open.Month,dayone);
            DateTime Daytwo = new (2024,first.Open.Month,daytwo);
             pd.Add(new ProposedDates{Gcc = Gcc, DayOne = Dayone, DayTwo = Daytwo, ScoreDayOne = r[0].Value,ScoreDayTwo = r[1].Value,Usedstrategy = s   });

        }
    
     
    }

    public void ScoreCalendar() {

    }

    public void Propose2(List<PayPeriodGcc> pglist) {
      
        var first = pglist.First();
        string Gcc = first.Gcc;
        DateTime TargetPeriodStart = new DateTime(2024,first.Open.Month,1);
        DateTime TargetPeriodEnd = TargetPeriodStart.AddMonths(1).AddSeconds(-1);
        Dictionary<int,int> cal = new();

        
        for(int i = 1; i <= TargetPeriodEnd.Day;i++) {
            cal[i] = 0;

            if (TargetPeriodStart.AddDays(i-1).DayOfWeek == DayOfWeek.Friday) {
              //  Console.WriteLine("Skipping Friday");
   //             cal[i] = -1;
            }
         
            if (TargetPeriodStart.AddDays(i-1).DayOfWeek == DayOfWeek.Saturday) {
              //  Console.WriteLine("Skipping Saturday");
                cal[i] = -50;
            }
            if (TargetPeriodStart.AddDays(i-1).DayOfWeek == DayOfWeek.Sunday) {
             //   Console.WriteLine("Skipping Sunday");
                cal[i] = -100;
            }
        }

        if (TargetPeriodStart.Month == 12) {
            for(int i = 21;i<=31;i++) {
                cal[i] = -50;
            }
        }

        // Blacklist
        foreach(var p in pglist) {
            cal[p.PayDate.Day] = -1;
        //    cal[p.PayDate.AddDays(1).Day] = -1;
            if (p.CutOff.Month == TargetPeriodEnd.Month) {
                 cal[p.CutOff.Day] -= 2;
            } else {
                Console.WriteLine($"Cut-off {p.CutOff} for {p.PayGroup} is in different month than {TargetPeriodEnd} for {Gcc}");
            }
            
        }

        foreach(var x in cal) {
    //        Console.WriteLine($"Day {x.Key}:{x.Value}");
        }


            foreach(int s in Enum.GetValues(typeof(Strategy))) {
                Cal(cal,pglist,(Strategy)s);
            }
            // Pick best result

            var res2 = pd.Where( x => x.Gcc == Gcc ).OrderByDescending(x => x.ScoreDayOnePercent);
            // .Select(x => new  { Strategy = x.Usedstrategy, Score = x.ScoreDayOnePercent});
            foreach(var res in res2) {
                Console.WriteLine($"{res.Usedstrategy},{res.Gcc},{res.DayOne},{res.DayOne.DayOfWeek},{res.ScoreDayOne},{res.ScoreDayOnePercent}%,{res.DayTwo},{res.ScoreDayTwo},{res.ScoreDayTwoPercent}%");  
                if (res.ScoreDayOnePercent >= 90 && res.Usedstrategy == Strategy.FirstDayClosed) {
        //            pd.RemoveAll(x => x.Gcc == Gcc && x.Usedstrategy != Strategy.FirstDayClosed);
                } else {
                    if (res.ScoreDayOnePercent >= 90 && res.Usedstrategy == Strategy.FirstDayOpen) {
          //              pd.RemoveAll(x => x.Gcc == Gcc && x.Usedstrategy != Strategy.FirstDayOpen);
                    }
                }

                
            }
            res2 = pd.Where( x => x.Gcc == Gcc ).OrderByDescending(x => x.ScoreDayOnePercent);
            if (res2.Count() > 0) {
                var Highest = res2.First();
                pd.RemoveAll(x => x.Gcc == Gcc && x.Usedstrategy != Highest.Usedstrategy);
            }
        

       
      //  }
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