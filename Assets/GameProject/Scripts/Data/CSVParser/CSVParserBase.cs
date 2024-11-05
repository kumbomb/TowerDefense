using UnityEngine;

public abstract class CSVParserBase
{
    public abstract void Parse(TextAsset textAsset);
    public abstract object GetDataList();
}