using System.Data;

namespace MigrationOrder.Logic;

using MigrationOrder.Models;
using MigrationOrder.Enums;
using Microsoft.VisualBasic;
using System.Diagnostics;


public class Analyze
{
    public List<DataDump> Drl { get; set; }


    public List<Score> Scores { get; set; } = new();

    public Analyze(List<DataDump> dataRows)
    {
        Drl = dataRows;

        Score sc = new();
        foreach (var gcc in Drl)
        {
            
            sc = new() { Gcc = gcc.Gcc, GccName = gcc.GccName };
            Scores.Add(sc);
        }
    }

       public List<Score> GetScores()
    {
        Scores.ForEach(x => x.Calculate());
        return Scores.OrderByDescending(x => x.Total).ToList();
    }

    public int Report(List<DataElement> de)
    {
        var x = de.GroupBy(x => x.Ct).Select(group => new
        {
            ct = group.Key,
            Count = group.Count()

        });
        int lower = 8;
        int upper = 58;
        int total = 0;
        double Pct;

        foreach (var j in x)
        {
            Pct = 100 * (double)(j.Count / (double)de.Count);
            if (j.Count < lower || j.Count > upper)
            {
                Console.WriteLine($"Inbalance found for type {j.ct} with count {j.Count}. Pct = {Pct}.");
            }

            else
            {
                //    Console.WriteLine($"Type {j.ct} with count {j.Count}. Pct = {Pct}");
            }
        }

        return x.Count();
    }

    public List<DataElement> Calculate(Operation op, DataTypes dt, List<DataElement> dr)
    {
        dr = dr.OrderByDescending(x => x.Number).ToList();
        var total = dr.Sum(x => x.Number);
        var min = dr.Min(x => x.Number);
        var max = dr.Max(x => x.Number);
        var three = (max - min) / 3;
        var records = dr.Count;
        Bucket b = ComplexityProxy.GetComplexityProxy(op, dt);
        //   Console.WriteLine($"Operation = {op}, dt = {dt}. Low = {b.Low}, med = {b.Medium}, high = {b.High}");
        //   Console.WriteLine($"Min = {min}, max = {max}");
        //  DataElement ing = dr.Find(x => x.Gcc == "ING");
        // Console.WriteLine($"ING = {ing.Number}");
        double[] buckets = new double[4];
        double from = min, to = min;
        List<DataElement> de = new();

        if (op == Operation.Even)
        {
            three = records / 3;
            de = dr.Skip(0).Take(three).ToList();
            de.ForEach(x => { x.Ct = ComplexityType.Low; });
            var a = dr.Skip(three).Take(three).ToList();
            a.ForEach(x => { x.Ct = ComplexityType.Medium; });
            de.AddRange(a);
            a = dr.Skip(three + three).Take(records - (three * 2)).ToList();
            a.ForEach(x => { x.Ct = ComplexityType.High; });
            de.AddRange(a);

            Debug.Assert(dr.Count() == de.Count());

            dr = de;
        }

        if (op == Operation.Relative)
        {
            foreach (DataElement d in dr)
            {
                d.Number = d.Number / total * 100;

                if (d.Number <= b.Low) { d.Ct = ComplexityType.Low; continue; };
                if (d.Number > b.Low && d.Number <= b.Medium)
                {
                    d.Ct = ComplexityType.Medium;
                }
                else
                {
                    d.Ct = ComplexityType.High;
                }

               
            }
        }
        if (op == Operation.Absolute)
        {


            foreach (DataElement d in dr)
            {
         

                if (d.Number <= b.Low) { d.Ct = ComplexityType.Low; continue; };
                if (d.Number > b.Low && d.Number <= b.Medium)
                {
                    d.Ct = ComplexityType.Medium;
                }
                else
                {
                    d.Ct = ComplexityType.High;
                }
            

            
            }
        }

        var cnt = Report(dr);
        if (cnt != 3 && records > 1)
        {
            // throw new Exception($"DT: {dt}, Op: {op}, count: {cnt}");
        }
        return dr;
    }


    public void PayGroupCount(Operation op = Operation.Absolute)
    {
        List<DataElement> Arr = Drl.Select(x => new DataElement { Gcc = x.Gcc, Number = x.PayGroups }).ToList();
        Arr = Calculate(op, DataTypes.Paygroup, Arr);
        foreach (Score s in Scores)
        {

            s.PayPeriodCount = Arr.Where(d => d.Gcc == s.Gcc).Select(x => x.Ct).SingleOrDefault(ComplexityType.Undefined);
        }
    }

    public void DocCount(Operation op = Operation.Absolute)
    {
        
        List<DataElement> Arr = Drl.Select(x => new DataElement { Gcc = x.Gcc, Number = x.Documents }).ToList();
        Arr = Calculate(op, DataTypes.DocCount, Arr);
        foreach (Score s in Scores)
        {
            
            s.DocCount = Arr.Where(d => d.Gcc == s.Gcc).Select(x => x.Ct).SingleOrDefault(ComplexityType.Undefined);
          
        }
    }

    public void LccCount(Operation op = Operation.Absolute)
    {
        List<DataElement> Arr = Drl.Select(x => new DataElement { Gcc = x.Gcc, Number = x.LCCs }).ToList();
        Arr = Calculate(op, DataTypes.Lcc, Arr);
        foreach (Score s in Scores)
        {
            s.LccCount = Arr.Where(d => d.Gcc == s.Gcc).Select(x => x.Ct).SingleOrDefault(ComplexityType.Undefined);
        }
    }

    public void CountryCount(Operation op = Operation.Absolute)
    {
        List<DataElement> Arr = Drl.Select(x => new DataElement { Gcc = x.Gcc, Number = x.Countries }).ToList();
        Arr = Calculate(op, DataTypes.CountryCount, Arr);
        foreach (Score s in Scores)
        {
            s.Countrycount = Arr.Where(d => d.Gcc == s.Gcc).Select(x => x.Ct).SingleOrDefault(ComplexityType.Undefined);
        }
    }

 

    public void EventCount(Operation op = Operation.Absolute)
    {
        List<DataElement> Arr = Drl.Select(x => new DataElement { Gcc = x.Gcc, Number = x.BODs }).ToList();
        Arr = Calculate(op, DataTypes.EventCount, Arr);
        foreach (Score s in Scores)
        {
            s.Eventcount = Arr.Where(d => d.Gcc == s.Gcc).Select(x => x.Ct).SingleOrDefault(ComplexityType.Undefined);
        
        }
    }

    public void HeadCount(Operation op = Operation.Absolute)
    {
        List<DataElement> Arr = Drl.Select(x => new DataElement { Gcc = x.Gcc, Number = x.Users }).ToList();
        Arr = Calculate(op, DataTypes.HeadCount, Arr);
        foreach (Score s in Scores)
        {
            s.Headcount = Arr.Where(d => d.Gcc == s.Gcc).Select(x => x.Ct).SingleOrDefault(ComplexityType.Undefined);
        }
    }

}