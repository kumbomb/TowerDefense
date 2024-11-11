using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>You must approach through `GoogleSheetManager.SO<GoogleSheetSO>()`</summary>
public class GoogleSheetSO : ScriptableObject
{
	public List<ChapterData> ChapterDataList;
	public List<StageData> StageDataList;
	public List<WaveData> WaveDataList;
	public List<MonsterData> MonsterDataList;
}

[Serializable]
public class ChapterData
{
	public string Id;
	public string ChapterTitle;
	public string ArrStageIdxs;
	public string ChapterImgName;
}

[Serializable]
public class StageData
{
	public string Id;
	public string StageName;
	public string WaveIdx;
	public string ArrStageReward;
	public string StageImgName;
	public string NeedStamina;
}

[Serializable]
public class WaveData
{
	public string Id;
	public string WaveIdx;
	public string WaveType;
	public string ArrMonsterIdxs;
	public string WaveTime;
	public string SummonCnt;
	public string MaxCount;
	public float MulitplyHp;
	public float MultiplySpeed;
	public int MultiplyAtsp;
}

[Serializable]
public class MonsterData
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

