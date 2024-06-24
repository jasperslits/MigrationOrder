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

public enum Strategy {
    FirstDayOpen,
    FirstDayClosed,
    FirstCalDay

}

public enum Month : ushort{
    September = 1,
    October = 2,
    November = 3,
    December = 4,
    January = 5
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