using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>You must approach through `GoogleSheetManager.SO<GoogleSheetSO>()`</summary>
public class GoogleSheetSO : ScriptableObject
{
	public List<ChapterCSVData> ChapterCSVDataList;
	public List<StageCSVData> StageCSVDataList;
	public List<WaveCSVData> WaveCSVDataList;
	public List<MonsterCSVData> MonsterCSVDataList;
}

[Serializable]
public class ChapterCSVData
{
	public string Id;
	public string ChapterTitle;
	public List<int> ArrStageIdxs;
	public string ChapterImgName;
}

[Serializable]
public class StageCSVData
{
	public string Id;
	public string StageName;
	public string WaveIdx;
	public List<int> ArrStageReward;
	public string StageImgName;
	public string NeedStamina;
}

[Serializable]
public class WaveCSVData
{
	public string Id;
	public string WaveIdx;
	public string WaveType;
	public List<int> ArrMonsterIdxs;
	public string WaveTime;
	public string SummonCnt;
	public string MaxCount;
	public float MulitplyHp;
	public float MultiplySpeed;
	public int MultiplyAtsp;
}

[Serializable]
public class MonsterCSVData
{
	public int Id;
	public string Name;
	public string PrefabName;
	public string IsBoss;
	public string HP;
	public string Speed;
	public string Atk;
	public string DropExp;
	public string DropBeads;
}

