using MigrationOrder.Models;
using MigrationOrder.Logic;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Diagnostics;

namespace MigrationOrder.Helpers;

public class CSVMigHelper {

    public List<PayPeriod> ReadPg(string Pg) {

     var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ",",
            HeaderValidated = null,
            MissingFieldFound = null
        };
        var records = new List<PayPeriod>();
        using (var reader = new StreamReader($"src/Data/Input/{Pg}"))
        using (var csv = new CsvReader(reader, config))
        {
            csv.Context.RegisterClassMap<ServiceProvidedMap>();
            records = csv.GetRecords<PayPeriod>().ToList();

        }
        return records;
        }
}

public class ServiceProvidedMap : ClassMap<PayPeriod>
{


	public ServiceProvidedMap()
	{
        DateTime ts = DateTime.Now;

		Map(m => m.Gcc).Name("GCC");
		Map(m => m.Lcc).Name("LCC");
		Map(m => m.PayGroup).Name("PAYGROUP");
		Map(m => m.Open).Name("OPEN_DATE").Name("OPEN_DATE").TypeConverterOption.Format("yyyy-MM-dd").Default(ts);;
		Map(m => m.PayDate).Name("PAY_DATE").TypeConverterOption.Format("yyyy-MM-dd").Default(ts);;
        Map(m => m.CutOff).Name("CUTOFFDATE").TypeConverterOption.Format("yyyy-MM-dd").Default(ts);;
		Map(m => m.Close).Name("CLOSE_DATE").TypeConverterOption.Format("yyyy-MM-dd").Default(ts);
	}
}