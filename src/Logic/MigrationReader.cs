namespace MigrationOrder.Logic;
using MigrationOrder.Models;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Diagnostics;

public class MigrationReader
{
    
    private List<PayPeriod> PayPeriods = new();

    public string DatadumpFile { get; set; }

    public string PayGroupFile { get; set; }   


    public MigrationReader()   {

    }

    public List<PayPeriod> GetPayPeriods() {


        return PayPeriods;
    }

     public List<DataDump> ReadDataDump()
    {

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ";",
            HeaderValidated = null,
            MissingFieldFound = null
        };
        var records = new List<DataDump>();
        using (var reader = new StreamReader($"src/Data/Input/{DatadumpFile}"))
        using (var csv = new CsvReader(reader, config))
        {
            records = csv.GetRecords<DataDump>().ToList();

        }

        if (MigrationConfig.OnlyGcc.Length > 0 )
        {
            records.RemoveAll(x => x.Gcc != MigrationConfig.OnlyGcc);
        }

        ReadPayPeriods();
        Debug.Assert(records.Count() != 0,$"List records is empty after reading {DatadumpFile}");
        foreach( var record in records) {
            record.Countries = PayPeriods.Where(c => c.Gcc == record.Gcc).DistinctBy(x => x.Lcc.Substring(0, 2)).Count();
            record.LCCs = PayPeriods.Where(c => c.Gcc == record.Gcc).DistinctBy(x => x.Lcc).Count();
            record.PayGroups = PayPeriods.Where(c => c.Gcc == record.Gcc).DistinctBy(x => x.PayGroup).Count();
        }

       PayPeriods = PayPeriods.GroupBy(x => ( x.Gcc,x.PayGroup,x.Open,x.Close)).Select(g => g.First()).ToList();
   
        records.RemoveAll(x => x.Gcc.StartsWith("ZZ"));
        records.RemoveAll(x => x.LCCs == 0);
        Debug.Assert(records.Count() != 0,"List records is empty");
        return records;
    }



      public void ReadPayPeriods()
    {

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
        };

        using (var reader = new StreamReader($"src/Data/Input/{PayGroupFile}"))
        using (var csv = new CsvReader(reader, config))
        {
            var records = csv.GetRecords<PayPeriod>().ToList();

            PayPeriods = records;
        }
    }
}