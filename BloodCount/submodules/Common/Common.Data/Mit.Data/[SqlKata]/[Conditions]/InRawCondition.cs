using SqlKata;

namespace Common.Data.SqlKata;

/// <summary>Represents a raw "is in" condition.</summary>
public class InRawCondition : AbstractCondition
{
    public string Column { get; set; }

    public string Value { get; set; }

    public override AbstractClause Clone()
    {
        var inRawCondition = new InRawCondition
        {
            Engine = Engine,
            Column = Column,
            Value = Value,
            IsOr = IsOr,
            IsNot = IsNot,
            Component = Component
        };
        return inRawCondition;
    }
}