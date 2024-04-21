namespace MigrationOrder.Enums {

public enum Operation {
    Relative,
    Absolute,
    Even
}

public enum BucketFill {
    Horizontal,
    Vertical,
}

public enum DataTypes {
    Lcc,
    Paygroup,
    HeadCount,
    CountryCount,
    EventCount,
    DocCount
}

public enum ComplexityType : ushort{
    Undefined = 0,
    Low = 10,
    Medium = 20,
    High = 50
};

}