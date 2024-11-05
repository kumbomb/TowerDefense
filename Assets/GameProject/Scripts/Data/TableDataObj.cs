using UnityEngine;
using BaseEnum;
using System;
using System.Collections.Generic;

[Serializable]
public class TableDataInfo
{
    public TABLE_TYPE tableType;
    public TextAsset textAsset;
}


[CreateAssetMenu(fileName = "TableDataObj", menuName = "Scriptable Objects/TableDataObj")]
public class TableDataObj : ScriptableObject
{
    public List<TableDataInfo> dataList = new();
}

