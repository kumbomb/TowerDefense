using BaseStruct;
using System;
using System.Collections.Generic;
using UnityEngine;

[TableType(BaseEnum.TABLE_TYPE.TABLE_CHAPTER)]
public class ChapterDataParser : CSVParserBase
{
    public List<ChapterData> ChapterDataList { get; private set; } = new List<ChapterData>();

    public override object GetDataList()
    {
        return ChapterDataList;
    }
    public override void Parse(TextAsset textAsset)
    {
        var lines = textAsset.text.Split('\n');
        for (int i = 1; i < lines.Length; i++) // 첫 줄은 헤더이므로 1부터 시작
        {
            var values = lines[i].Split(',');

            if (string.IsNullOrEmpty(values[0]))
                break;

            var chapterData = new ChapterData
            {
                Id = int.Parse(values[0]),
                ChapterTitle = values[1],
                StageIdxs = Array.ConvertAll(values[2].Split('/'), int.Parse),
                ChapterImgName = values[3]
            };

            ChapterDataList.Add(chapterData);
        }
    }
}

[TableType(BaseEnum.TABLE_TYPE.TABLE_STAGE)]
public class StageDataParser : CSVParserBase
{
    public List<StageData> StageDataList { get; private set; } = new List<StageData>();

    public override object GetDataList()
    {
        return StageDataList;
    }
    public override void Parse(TextAsset textAsset)
    {
        var lines = textAsset.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            var values = lines[i].Split(',');

            if (string.IsNullOrEmpty(values[0]))
                break;

            var stageData = new StageData
            {
                Id = int.Parse(values[0]),
                StageName = values[1],
                WaveIdx = int.Parse(values[2]),
                rewardStructs = ParseRewardStructs(values[3]),
                StageImgName = values[4],
                NeedStamina = int.Parse(values[5]),
                SceneName = values[6],
                LevelPrefabName = values[7]
            };

            StageDataList.Add(stageData);
        }
    }
    private RewardStruct[] ParseRewardStructs(string input)
    {
        if(string.IsNullOrEmpty(input)) return null;
        var rewardStrings = input.Split('|');
        var rewardList = new List<RewardStruct>();

        foreach (var rewardStr in rewardStrings)
        {
            var rewardValues = rewardStr.Split('/');
            if (rewardValues.Length != 2) continue;

            var reward = new RewardStruct
            {
                Idx = int.Parse(rewardValues[0]),
                Amount = int.Parse(rewardValues[1])
            };

            rewardList.Add(reward);
        }

        return rewardList.ToArray();
    }
}

[TableType(BaseEnum.TABLE_TYPE.TABLE_WAVE)]
public class WaveDataParser : CSVParserBase
{
    public List<WaveData> WaveDataList { get; private set; } = new List<WaveData>();

    public override object GetDataList()
    {
        return WaveDataList;
    }
    public override void Parse(TextAsset textAsset)
    {
        var lines = textAsset.text.Split('\n');
        for (int i = 1; i < lines.Length; i++) // 첫 줄은 헤더이므로 1부터 시작
        {
            var values = lines[i].Split(',');

            if (string.IsNullOrEmpty(values[0]))
                break;

            var waveData = new WaveData
            {
                Id = int.Parse(values[0]),
                WaveIdx = int.Parse(values[1]),
                WaveType = int.Parse(values[2]),
                MonsterIdxs = Array.ConvertAll(values[3].Split('/'), int.Parse),
                WaveTime = float.Parse(values[4]),
                SummonCnt = int.Parse(values[5]),
                MaxCount = int.Parse(values[6]),
                MultiplyHp = float.Parse(values[7]),
                MultiplySpeed = float.Parse(values[8]),
                MultiplyAtsp = float.Parse(values[9])
            };

            WaveDataList.Add(waveData);
        }
    }
}

[TableType(BaseEnum.TABLE_TYPE.TABLE_MONSTER)]
public class MonsterDataParser : CSVParserBase
{
    public List<MonsterData> MonsterDataList { get; private set; } = new List<MonsterData>();

    public override object GetDataList()
    {
        return MonsterDataList;
    }
    public override void Parse(TextAsset textAsset)
    {
        var lines = textAsset.text.Split('\n');
        for (int i = 1; i < lines.Length; i++) // 첫 줄은 헤더이므로 1부터 시작
        {
            var values = lines[i].Split(',');

            if (string.IsNullOrEmpty(values[0]))
                break;

            var monsterData = new MonsterData
            {
                Id = int.Parse(values[0]),
                Name = values[1],
                PrefabName = values[2],
                IsBoss = int.Parse(values[3]),
                HP = int.Parse(values[4]),
                Speed = float.Parse(values[5]),
                Atk = int.Parse(values[6]),
                DropExp = int.Parse(values[7]),
                DropBeads = int.Parse(values[8])
            };

            MonsterDataList.Add(monsterData);
        }
    }
}