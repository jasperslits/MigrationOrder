
namespace MigrationOrder.Logic;

using MigrationOrder.Enums;
using MigrationOrder.Models;
using System.Diagnostics;



public class Distribute
{

    public double[] Predefined(double[] dist)
    {
        var keyValuePairs = MigrationConfig.Predefined();
        Score s;
        foreach(var kvp in keyValuePairs) {
            s = Scores.Find(x => x.Gcc == kvp.Key);
            dist[kvp.Value]-=s.Total;
            DistScores[kvp.Value].Add(s);
            Scores.RemoveAll(x => x.Gcc == kvp.Key);
        }
        return dist;
        


    }

    public Dictionary<int, List<Score>> DistScores = [];
    private List<Score> Scores {get;set;}

    public Distribute( List<Score> scores,BucketFill bf)
    {
        int buckets = MigrationConfig.NrPeriods;
        Scores = scores;
        Console.WriteLine($"Distribution even is {bf}");
        double totalGcc = scores.Count;
        for (int i = 0; i < buckets; i++)
        {
            DistScores[i] = [];
        }

        int BucketSize = (int)Math.Round(totalGcc / buckets, 0);

        int j = 0;
        if (bf == BucketFill.Horizontal)
        {
            foreach (Score score in scores)
            {
                if (j == buckets) j = 0;
                DistScores[j].Add(score);
                j++;
            }
        }
        else
        {
            double s = scores.Sum(x => x.Total) / buckets;
            double[] array = new double[buckets];
            Array.Fill(array,s);

            array = Predefined(array);

            bool hasSpace;
            foreach (Score score in scores)
            {
                for(int i = 0; i < buckets;i++) {
                    hasSpace = array[i] - score.Total > 0;
                //    Console.WriteLine($"Has space {hasSpace} in bucket {i} for {score.Gcc} with remaining size: {score.Total} with {array[i]}");
                    if (hasSpace || (i == buckets-1)) {
                        array[i] -= score.Total;
                        DistScores[i].Add(score);
                        break;
                    }
                }
           //     Console.WriteLine($"GCC {score.Gcc}");
              
            }

        }

        int control = 0;
        foreach (var d in DistScores)
        {
            control += d.Value.Count();
        }
        Debug.Assert(control == totalGcc,$"Missing GCCs {control} vs {totalGcc}");
    }


    public Dictionary<int, List<Score>> GetDistScores()
    {
        return DistScores;
    }
}