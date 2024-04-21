using MigrationOrder.Enums;

namespace MigrationOrder.Models;

public class ComplexityProxy
{
  public static Bucket GetComplexityProxy(Operation op,DataTypes type) {
    switch(op){

      case Operation.Relative:
        switch(type){
          case DataTypes.Lcc: return ComplexitySplitterRel.LccCount;
          case DataTypes.HeadCount: return ComplexitySplitterRel.HeadCount;
          case DataTypes.EventCount: return ComplexitySplitterRel.EventCount;
          case DataTypes.CountryCount: return ComplexitySplitterRel.CountryCount;
          case DataTypes.Paygroup: return ComplexitySplitterRel.PaygroupCount;
          case DataTypes.DocCount: return ComplexitySplitterRel.DocCount;
        }
      break;
      case Operation.Absolute:
      switch(type){
          case DataTypes.Lcc: return ComplexitySplitterAbs.LccCount;
          case DataTypes.HeadCount: return ComplexitySplitterAbs.HeadCount;
          case DataTypes.EventCount: return ComplexitySplitterAbs.EventCount;
          case DataTypes.CountryCount: return ComplexitySplitterAbs.CountryCount;
          case DataTypes.Paygroup: return ComplexitySplitterAbs.PaygroupCount;
          case DataTypes.DocCount: return ComplexitySplitterAbs.DocCount;
      }
      break;
          }
 
  return ComplexitySplitterRel.LccCount;
  }
  
}


public class ComplexitySplitterAbs {

    public static readonly Bucket LccCount = new() { Low = 10, Medium = 20, High = 30 };  
    public static readonly Bucket HeadCount = new() { Low = 600, Medium = 5000, High = 10000 }; 
    public static readonly Bucket EventCount = new() { Low = 900, Medium = 3600, High = 100000 }; 
    
    public static readonly Bucket CountryCount = new() { Low = 2,Medium = 10, High = 30}; 
    public static readonly Bucket PaygroupCount = new() { Low = 2,Medium = 20, High = 30}; 

    public static readonly Bucket DocCount = new() { Low = 900, Medium = 3600, High = 100000 }; 

} 

public class ComplexitySplitterRel {

    public static Bucket LccCount = new() { Low = 10, Medium = 20, High = 30 };  
    public static Bucket HeadCount = new() { Low = 2, Medium = 5, High = 10 }; 
    public static Bucket EventCount = new() { Low = 10, Medium = 10, High = 10 }; 
    
    public static Bucket CountryCount = new() { Low = 10,Medium = 10, High = 10}; 
    public static Bucket PaygroupCount = new() { Low = 2,Medium = 20, High = 30}; 

    public static Bucket DocCount = new() { Low = 2,Medium = 20, High = 30}; 

} 
