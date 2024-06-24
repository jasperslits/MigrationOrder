namespace MigrationOrder;

using MigrationOrder.Logic;
using MigrationOrder.Models;
using MigrationOrder.Helpers;
using MigrationOrder.Enums;

class Program
{

    static void Main(string[] args)
    {
      // var mh = new MigHelper();
      var mh = new CSVMigHelper();
      var pgdata = mh.ReadPg(MigrationConfig.paygroupfile);
      var apr = new AnalyzePayperiod(pgdata);
      var pr = new PLanningReader();
      List<Planning> p = pr.GetPlanning();
    // p.Clear();
    //  p.Add(new Planning {Gcc = "ALC",Month = 0});
      foreach(var planning in p) {
  
        var x = apr.FindData(planning.Gcc,planning.Month);
        if (x.Count > 0) {
        apr.Propose2(x);
        } else {
          Console.WriteLine($" No date found for {planning.Gcc}");
        }
      }
 
      Console.WriteLine("======");
      List<string> ls = new();
     foreach(var res in apr.GetProposedDates()) {
      ls.Add($"{res.Usedstrategy},{res.Gcc},{res.DayOne},{res.DayOne.DayOfWeek},{res.ScoreDayOne},{res.ScoreDayOnePercent}%,{res.DayTwo},{res.ScoreDayTwo},{res.ScoreDayTwoPercent}%");  
     }
     File.WriteAllLines("src/Data/Output/nw.csv", ls); 

    }
} 

