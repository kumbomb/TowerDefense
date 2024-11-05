using BaseEnum;
using System;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class TableTypeAttribute : Attribute
{
    public TABLE_TYPE TableType { get; }

    public TableTypeAttribute(TABLE_TYPE tableType)
    {
        TableType = tableType;
    }
}
