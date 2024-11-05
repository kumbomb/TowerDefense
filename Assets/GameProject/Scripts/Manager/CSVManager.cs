using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using BaseEnum;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class CSVManager : Singleton<CSVManager>
{
    [Header("[ == CSV Scriptable Object == ]")]
    [SerializeField] TableDataObj csvDataList;

    bool isInit = false;

    #region 데이터
    // 각 데이터 타입별로 파서와 데이터를 보관하는 딕셔너리
    [SerializeField]private Dictionary<TABLE_TYPE, CSVParserBase> parserDictionary = new Dictionary<TABLE_TYPE, CSVParserBase>();
    [SerializeField] private Dictionary<TABLE_TYPE, object> dataDictionary = new Dictionary<TABLE_TYPE, object>();
    #endregion

    public async UniTask InitCSV()
    {
        if (isInit) return;
        isInit = true;

        InitializeParsers();
        await LoadCSVData();
    }

    // Reflection 반영
    private void InitializeParsers()
    {
        // 현재 어셈블리에서 CSVParserBase를 상속받은 모든 타입을 찾습니다.
        var parserTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsSubclassOf(typeof(CSVParserBase)) && !t.IsAbstract);

        foreach (var type in parserTypes)
        {
            // TableTypeAttribute가 적용된 타입만 선택합니다.
            var attribute = type.GetCustomAttribute<TableTypeAttribute>();
            if (attribute != null)
            {
                var tableType = attribute.TableType;

                // 파서 인스턴스 생성
                var parserInstance = Activator.CreateInstance(type) as CSVParserBase;
                if (parserInstance != null)
                {
                    parserDictionary[tableType] = parserInstance;
                }
                else
                {
                    Debug.LogWarning($"'{type.Name}' 파서 인스턴스를 생성할 수 없습니다.");
                }
            }
            else
            {
                Debug.LogWarning($"'{type.Name}'에 TableTypeAttribute가 적용되어 있지 않습니다.");
            }
        }
    }

    private async UniTask LoadCSVData()
    {
        var parseTasks = new List<UniTask>();
        foreach (var tableDataInfo in csvDataList.dataList)
        {
            if (parserDictionary.TryGetValue(tableDataInfo.tableType, out var parser))
            {
                // 파싱 작업을 비동기로 처리
                parser.Parse(tableDataInfo.textAsset);
                dataDictionary[tableDataInfo.tableType] = parser.GetDataList();
            }
            else
            {
                Debug.LogWarning($"TABLE_TYPE '{tableDataInfo.tableType}'에 대한 파서를 찾을 수 없습니다.");
            }
        }
    }

    // 데이터에 접근하기 위한 제네릭 메서드
    public List<T> GetDataList<T>(TABLE_TYPE tableType)
    {
        if (dataDictionary.TryGetValue(tableType, out var data))
        {
            return data as List<T>;
        }
        Debug.LogWarning($"TABLE_TYPE '{tableType}'에 대한 데이터를 찾을 수 없습니다.");
        return null;
    }

    #region 데이터 가져가는 예시 

    //챕터 데이터 가져오기
    //var chapters = csvManager.GetDataList<ChapterData>(TABLE_TYPE.TABLE_CHAPTER);
    //foreach (var chapter in chapters)
    //{
    //  Debug.Log($"챕터 ID: {chapter.Id}, 제목: {chapter.ChapterTitle}");
    //}

    //스테이지 데이터 가져오기
    //var stages = csvManager.GetDataList<StageData>(TABLE_TYPE.TABLE_STAGE);
    //foreach (var stage in stages)
    //{
    //    Debug.Log($"스테이지 ID: {stage.Id}, 이름: {stage.StageName}");
    //}

    #endregion

}

