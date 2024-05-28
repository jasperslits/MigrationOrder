namespace MigrationOrder;

using MigrationOrder.Logic;
using MigrationOrder.Models;
using MigrationOrder.Helpers;

using Microsoft.Extensions.Configuration;
using OfficeOpenXml;

class Program
{

    static void Main(string[] args)
    {
      // var mh = new MigHelper();
      var mh = new CSVMigHelper();
      var pgdata = mh.ReadPg(MigrationConfig.paygroupfile);
      var apr = new AnalyzePayperiod(pgdata);
      string[] gccs = {"ARN","BAS","CIB","COR","EAS","ETN","HUF","IPI","JNJ","JSE","NCR","NVS","PFC","PRY","SAM","SFY","SRT","WEC"};
      foreach(var gc in gccs) {
        var x = apr.FindData(gc,1);
        if (x.Count > 0) {
        apr.Propose2(x);
        } else {
          Console.WriteLine($" No date found for {gc}");
        }
      }
 
      
     foreach(var res in apr.GetProposedDates()) {
        Console.WriteLine($"{res.Gcc},{res.DayOne},{res.DayTwo},{res.Score}");  
     }
    }
      /*
      var configuration = new ConfigurationBuilder()
    .AddInMemoryCollection(new Dictionary<string, string?>()
    {
        ["SomeKey"] = "SomeValue"
    })
    .Build();
    configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
  
      
  /*
  List<Score> scores= new List<Score>();
  Score a = new Score {
    Gcc = "KPN",
    Total = 88
  };
  scores.Add(a);
     a = new Score {
    Gcc = "RAB",
    Total = 60
  };
  scores.Add(a);
  a = new Score {
    Gcc = "SIE",
    Total = 75
  };
    scores.Add(a);
   a = new Score {
    Gcc = "SAM",
    Total = 70
  };
    scores.Add(a);
   a = new Score {
    Gcc = "JET",
    Total = 55
  };
    scores.Add(a);   
    a = new Score {
    Gcc = "JUM",
    Total = 50
  };
   scores.Add(a);   
    a = new Score {
    Gcc = "MRT",
    Total = 40
  };
   scores.Add(a);   
    a = new Score {
    Gcc = "SOL",
    Total = 40
  };
    scores.Add(a);
    scores = scores.OrderByDescending(x => x.Total).ToList();
  var d = new Distribute(3,scores,false);
  */


     //   h.ToScreen();
  //      h.ToCSV("src/Data/output/results.csv");
     //   h.Parameters("src/Data/output/parameters.csv");
    // h.Statistics("some");
    }
//}

