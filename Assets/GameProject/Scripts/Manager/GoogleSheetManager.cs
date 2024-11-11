using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Linq;
using Cysharp.Threading.Tasks;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GoogleSheetManager :  Singleton<GoogleSheetManager>
{
    [Tooltip("true: google sheet, false: local json")]
    [SerializeField] bool isAccessGoogleSheet = true;

    public bool IsAccessGoogleSheet{
        get{
            return isAccessGoogleSheet;
        }
        set{
            isAccessGoogleSheet = value;
        }
    }

    [Tooltip("Google sheet appsscript webapp url")]
    [SerializeField] string googleSheetUrl;
    [Tooltip("Google sheet avail sheet tabs. separate with `/`. For example `Sheet1/Sheet2`")]
    [SerializeField] string availSheets = "Sheet1/Sheet2";
    [Tooltip("For example `/GenerateGoogleSheet`")]
    [SerializeField] string generateFolderPath = "/GenerateGoogleSheet";
    [Tooltip("You must approach through `GoogleSheetManager.SO<GoogleSheetSO>()`")]
    public ScriptableObject googleSheetSO;

    string JsonPath => $"{Application.dataPath}{generateFolderPath}/GoogleSheetJson.json";
    string ClassPath => $"{Application.dataPath}{generateFolderPath}/GoogleSheetClass.cs";
    string SOPath => $"Assets{generateFolderPath}/GoogleSheetSO.asset";

    string[] availSheetArray;
    string json;
    bool refreshTrigger;

    public T SO<T>() where T : ScriptableObject
    {
        if (googleSheetSO == null)
        {
            Debug.Log($"googleSheetSO is null");
            return null;
        }

        return googleSheetSO as T;
    }

    [ContextMenu("FetchGoogleSheet")]
    public async UniTask FetchGoogleSheet()
   {
        #if UNITY_EDITOR
        // Init
        availSheetArray = availSheets.Split('/');

        if (isAccessGoogleSheet)
        {
            Debug.Log($"Loading from Google Sheet...");
            json = await LoadDataGoogleSheet(googleSheetUrl);
        }
        else
        {
            Debug.Log($"Loading from local JSON...");
            json = LoadDataLocalJson();
        }
        if (json == null) return;

        bool isJsonSaved = SaveFileOrSkip(JsonPath, json);
        string allClassCode = GenerateCSharpClass(json);
        bool isClassSaved = SaveFileOrSkip(ClassPath, allClassCode);

        if (isJsonSaved || isClassSaved)
        {
            refreshTrigger = true;
            #if UNITY_EDITOR
            AssetDatabase.Refresh();
            await UniTask.WaitUntil(() => !EditorApplication.isCompiling && !EditorApplication.isUpdating);
            #endif
            CreateGoogleSheetSO();
            Debug.Log($"Fetch done.");
        }
        else
        {
            CreateGoogleSheetSO();
            Debug.Log($"Fetch done.");
        }
        #else
        LoadGeneratedData();
        await UniTask.CompletedTask;
        #endif
    }
    public void LoadGeneratedData()
    {
        // 로컬에 저장된 ScriptableObject를 로드
        if (googleSheetSO == null)
        {
            googleSheetSO = Resources.Load<ScriptableObject>("GoogleSheetSO");
            if (googleSheetSO == null)
            {
                Debug.LogError("GoogleSheetSO.asset을 Resources 폴더에 위치시키거나 경로를 확인하세요.");
            }
        }
    }
    #region 데이터 로드

    // 구글 스프레드시트에서 데이터 받아오기
    async Task<string> LoadDataGoogleSheet(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                byte[] dataBytes = await client.GetByteArrayAsync(url);
                return Encoding.UTF8.GetString(dataBytes);
            }
            catch (HttpRequestException e)
            {
                Debug.LogError($"Request error: {e.Message}");
                return null;
            }
        }
    }

    // 로컬 내부에 있는 데이터 사용
    string LoadDataLocalJson()
    {
        if (File.Exists(JsonPath))
        {
            return File.ReadAllText(JsonPath);
        }

        Debug.Log($"File not exist.\n{JsonPath}");
        return null;
    }

    #endregion

    // 파일 저장 (내용이 같으면 저장하지 않음)
    bool SaveFileOrSkip(string path, string contents)
    {
        string directoryPath = Path.GetDirectoryName(path);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        if (File.Exists(path) && File.ReadAllText(path).Equals(contents))
            return false;

        File.WriteAllText(path, contents);
        return true;
    }

    // 사용 가능한 시트인지 확인
    bool IsExistAvailSheets(string sheetName)
    {
        return Array.Exists(availSheetArray, x => x == sheetName);
    }

    // 받아온 JSON 데이터로 C# 클래스 생성
    string GenerateCSharpClass(string jsonInput)
    {
        JObject jsonObject = JObject.Parse(jsonInput);
        StringBuilder classCode = new();

        // ScriptableObject 클래스 생성
        classCode.AppendLine("using System;");
        classCode.AppendLine("using System.Collections.Generic;");
        classCode.AppendLine("using UnityEngine;\n");
        classCode.AppendLine("/// <summary>You must approach through `GoogleSheetManager.SO<GoogleSheetSO>()`</summary>");
        classCode.AppendLine("public class GoogleSheetSO : ScriptableObject");
        classCode.AppendLine("{");

        foreach (var sheet in jsonObject)
        {
            string className = sheet.Key;
            if (!IsExistAvailSheets(className))
                continue;

            classCode.AppendLine($"\tpublic List<{className}> {className}List;");
        }
        classCode.AppendLine("}\n");

        // 데이터 클래스 생성
        foreach (var jObject in jsonObject)
        {
            string className = jObject.Key;

            if (!IsExistAvailSheets(className))
                continue;

            var items = (JArray)jObject.Value;
            classCode.AppendLine($"[Serializable]");
            classCode.AppendLine($"public class {className}");
            classCode.AppendLine("{");

            // 각 필드의 타입 결정
            Dictionary<string, string> fieldTypes = new Dictionary<string, string>();

            foreach (var item in items)
            {
                foreach (var property in ((JObject)item).Properties())
                {
                    string propertyName = property.Name;
                    string propertyType = GetCSharpType(property.Value);

                    if (fieldTypes.ContainsKey(propertyName))
                    {
                        fieldTypes[propertyName] = GetHigherType(fieldTypes[propertyName], propertyType);
                    }
                    else
                    {
                        fieldTypes[propertyName] = propertyType;
                    }
                }
            }

            // 클래스 코드 생성
            foreach (var field in fieldTypes)
            {
                string propertyName = field.Key;
                string propertyType = field.Value;
                classCode.AppendLine($"\tpublic {propertyType} {propertyName};");
            }

            classCode.AppendLine("}\n");
        }

        return classCode.ToString();
    }

    string GetCSharpType(JToken token)
    {
        switch (token.Type)
        {
            case JTokenType.Integer:
                return "int";
            case JTokenType.Float:
                return "float";
            case JTokenType.Boolean:
                return "bool";
            case JTokenType.Array:
                // 배열의 요소 타입 결정
                var firstElement = token.First;
                if (firstElement != null)
                {
                    string elementType = GetCSharpType(firstElement);
                    return $"List<{elementType}>";
                }
                else
                {
                    // 빈 배열인 경우 object 타입 사용
                    return "List<object>";
                }
            default:
                return "string";
        }
    }

    // 두 타입 중 더 넓은 타입을 반환하는 메서드
    string GetHigherType(string type1, string type2)
    {
        if (type1 == type2)
            return type1;

        if (type1.StartsWith("List<") || type2.StartsWith("List<"))
            return "string"; // 배열과 다른 타입이 섞이면 string으로 처리

        if (type1 == "string" || type2 == "string")
            return "string";
        if (type1 == "float" || type2 == "float")
            return "float";
        if (type1 == "int" || type2 == "int")
            return "int";
        return "string";
    }

    bool CreateGoogleSheetSO()
    {
        #if UNITY_EDITOR
        Type googleSheetSOType = GetTypeByName("GoogleSheetSO");
        if (googleSheetSOType == null)
        {
            Debug.LogError("GoogleSheetSO 타입을 찾을 수 없습니다.");
            return false;
        }

        googleSheetSO = ScriptableObject.CreateInstance(googleSheetSOType);
        JObject jsonObject = JObject.Parse(json);
        try
        {
            foreach (var jObject in jsonObject)
            {
                string className = jObject.Key;
                if (!IsExistAvailSheets(className))
                    continue;

                Type classType = GetTypeByName(className);
                if (classType == null)
                {
                    Debug.LogError($"{className} 타입을 찾을 수 없습니다.");
                    continue;
                }

                Type listType = typeof(List<>).MakeGenericType(classType);
                IList listInst = (IList)Activator.CreateInstance(listType);
                var items = (JArray)jObject.Value;

                foreach (var item in items)
                {
                    object classInst = Activator.CreateInstance(classType);
                    JObject itemObj = (JObject)item;

                    foreach (var property in itemObj.Properties())
                    {
                        FieldInfo fieldInfo = classType.GetField(property.Name);
                        if (fieldInfo == null)
                        {
                            Debug.LogWarning($"{className}에 {property.Name} 필드가 없습니다.");
                            continue;
                        }

                        object value = GetValueFromJToken(property.Value, fieldInfo.FieldType);
                        fieldInfo.SetValue(classInst, value);
                    }

                    listInst.Add(classInst);
                }

                googleSheetSO.GetType().GetField($"{className}List").SetValue(googleSheetSO, listInst);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"CreateGoogleSheetSO error: {e.Message}");
        }
        Debug.Log("CreateGoogleSheetSO 완료");
        AssetDatabase.CreateAsset(googleSheetSO, SOPath);
        AssetDatabase.SaveAssets();
        #endif
        return true;
    }

    // JToken에서 특정 타입으로 변환하여 값을 가져오는 메서드
    object GetValueFromJToken(JToken token, Type targetType)
    {
        if (token.Type == JTokenType.Null)
        {
            return null;
        }

        if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(List<>))
        {
            Type elementType = targetType.GetGenericArguments()[0];
            var listType = typeof(List<>).MakeGenericType(elementType);
            var list = (IList)Activator.CreateInstance(listType);

            foreach (var childToken in token)
            {
                object elementValue = GetValueFromJToken(childToken, elementType);
                list.Add(elementValue);
            }

            return list;
        }
        else
        {
            // 숫자 타입 처리
            if (targetType == typeof(int))
            {
                return token.Value<int>();
            }
            else if (targetType == typeof(float))
            {
                return token.Value<float>();
            }
            else if (targetType == typeof(bool))
            {
                return token.Value<bool>();
            }
            else
            {
                return token.ToString();
            }
        }
    }

    // 문자열로부터 Type 객체를 가져오는 메서드 (현재 어셈블리에서 검색)
    Type GetTypeByName(string typeName)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            Type type = assembly.GetType(typeName);
            if (type != null)
                return type;
        }
        return null;
    }

    void OnValidate()
    {
        if (refreshTrigger)
        {
            bool isCompleted = CreateGoogleSheetSO();
            if (isCompleted)
            {
                refreshTrigger = false;
                Debug.Log($"Fetch done.");
            }
        }
    }
}
