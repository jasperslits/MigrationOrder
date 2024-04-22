namespace MigrationOrder.Logic;

using MigrationOrder.Enums;
using MigrationOrder.Models;

public class MigHelper
{

    private Dictionary<int, List<Score>> DistResults { get; set; }

    private MigrationReader Mr = new();

    private AnalyzePayperiod Apr;

    private Excel E { get; set; }

    public MigHelper()
    {
        Mr.DatadumpFile = MigrationConfig.datadumpfile;
        Mr.PayGroupFile = MigrationConfig.paygroupfile;
        E = new Excel();
        Run();

    }

    private void Run()
    {

        EC();
        PG();
      
        if (MigrationConfig.IncludeParams) {
            E.Parameters(Parameters());
        }
         if (MigrationConfig.IncludeStats) {
            E.Statistics(Statistics());
        }
         WriteOutput();
        if (MigrationConfig.IncludeDistribution) {
            E.Distribution(DistResults);
        }
        E.Save();
       
        
    }
    public void PG()
    {
        Apr = new AnalyzePayperiod(Mr.GetPayPeriods());
    }

    public void EC()
    {
        List<DataDump> dd = Mr.ReadDataDump();
        Console.WriteLine($"Rows = {dd.Count}");
        Analyze a = new(dd);
        a.EventCount(Enums.Operation.Even);
        a.HeadCount();
        a.LccCount();
        a.CountryCount();
        a.DocCount();
        a.PayGroupCount();

        var scores = a.GetScores();
        Distribute d = new(scores, BucketFill.Vertical);
        DistResults = d.GetDistScores();
    }

    public List<String> Parameters()
    {
        List<String> ls = new() { "Operation;Field;Low;Medium;High" };
        ls.Add($"Absolute;LCC Count;{ComplexitySplitterAbs.LccCount.Low};{ComplexitySplitterAbs.LccCount.Medium};{ComplexitySplitterAbs.LccCount.High};");
        ls.Add($"Absolute;Country Count;{ComplexitySplitterAbs.CountryCount.Low};{ComplexitySplitterAbs.CountryCount.Medium};{ComplexitySplitterAbs.CountryCount.High};");
        ls.Add($"Absolute;Event Count;{ComplexitySplitterAbs.EventCount.Low};{ComplexitySplitterAbs.EventCount.Medium};{ComplexitySplitterAbs.EventCount.High};");
        ls.Add($"Absolute;Head Count;{ComplexitySplitterAbs.HeadCount.Low};{ComplexitySplitterAbs.HeadCount.Medium};{ComplexitySplitterAbs.HeadCount.High};");
        ls.Add($"Absolute;Pay group Count;{ComplexitySplitterAbs.PaygroupCount.Low};{ComplexitySplitterAbs.PaygroupCount.Medium};{ComplexitySplitterAbs.PaygroupCount.High};");
        ls.Add($"Absolute;Doc Count;{ComplexitySplitterAbs.DocCount.Low};{ComplexitySplitterAbs.DocCount.Medium};{ComplexitySplitterAbs.DocCount.High};");

        return ls;
    }


    public List<String> Statistics()
    {
        List<String> ls = new() { "Data point;Low;Medium;High" };
    
        Dictionary<string, int[]> ls2 = new();
        ls2["CountryCount"] = new int[3];
        ls2["CountryCount"] = new int[3];
        ls2["DocCount"] = new int[3];
        ls2["EventCount"] = new int[3];
        ls2["EmployeeCount"] = new int[3];

        

        foreach (var result in DistResults)
        {
          //  var r2 = result.Value.GroupBy(x => x.Countrycount).Select(x => new {count = x.Count(), type = x.Key } );
            
            ls2["CountryCount"][0] += result.Value.Where(x => x.Countrycount == ComplexityType.Low).Count();
            ls2["CountryCount"][1] += result.Value.Where(x => x.Countrycount == ComplexityType.Medium).Count(); 
            ls2["CountryCount"][2] += result.Value.Where(x => x.Countrycount == ComplexityType.High).Count();
            ls2["DocCount"][0] += result.Value.Where(x => x.DocCount == ComplexityType.Low).Count();
            ls2["DocCount"][1] += result.Value.Where(x => x.DocCount == ComplexityType.Medium).Count(); 
            ls2["DocCount"][2] += result.Value.Where(x => x.DocCount == ComplexityType.High).Count();
            ls2["EventCount"][0] += result.Value.Where(x => x.Eventcount == ComplexityType.Low).Count();
            ls2["EventCount"][1] += result.Value.Where(x => x.Eventcount == ComplexityType.Medium).Count(); 
            ls2["EventCount"][2] += result.Value.Where(x => x.Eventcount == ComplexityType.High).Count();
            ls2["EmployeeCount"][0] += result.Value.Where(x => x.Headcount == ComplexityType.Low).Count();
            ls2["EmployeeCount"][1] += result.Value.Where(x => x.Headcount == ComplexityType.Medium).Count(); 
            ls2["EmployeeCount"][2] += result.Value.Where(x => x.Headcount == ComplexityType.High).Count();
        
        }
        


        foreach (var x in ls2)
        {
            ls.Add($"{x.Key};{x.Value[0]};{x.Value[1]};{x.Value[2]}");
        }
        return ls;
    }

    public void WriteOutput()
    {

        List<String> pg = new() { "GCC;Month offset;Pay group; Date one;Date two" };
        List<PayPeriodGcc> ppglist;
        PayPeriodGcc ppg;

        foreach (var result in DistResults)
        {
            foreach (var obj in result.Value)
            {
                if (MigrationConfig.OnlyGcc.Length > 0 && obj.Gcc != MigrationConfig.OnlyGcc)
                {
                    continue;
                }

                
              

                ppglist = Apr.FindData(obj.Gcc, result.Key);
                if (ppglist.Count == 0)
                {
                    Console.WriteLine($"No PG data found for {obj.Gcc}");
                    continue;
                }
                E.Init(obj.Gcc, obj.GccName, ppglist[0].Dayone.Month);

                if (ppglist.Count == 1)
                {
                    ppg = ppglist.First();
                    pg.Add($"{ppg.Gcc};{result.Key + 1};{ppg.PayGroup};{ppg.Dayone.ToString("yyyy-MM-dd")};{ppg.Daytwo.ToString("yyyy-MM-dd")}");

                    E.c.ElementAt(ppg.Dayone.Day - 1).Value += ppg.PayGroup + ",";

                }
                else
                {
                    // Multiple pay periods
                    ppg = Apr.FindBest(ppglist);
                    pg.Add($"{ppg.Gcc};{result.Key + 1};{ppg.PayGroup};{ppg.Dayone.ToString("yyyy-MM-dd")};{ppg.Daytwo.ToString("yyyy-MM-dd")}");

                    foreach (var ppg2 in ppglist)
                    {
                        E.c.ElementAt(ppg2.Dayone.Day - 1).Value += ppg2.PayGroup + ",";
                        E.c.ElementAt(ppg2.Daytwo.Day - 1).Value += ppg2.PayGroup + ",";

                    }
                    E.FillFile();
                }

            }
        }

        }
    }
