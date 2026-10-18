
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;


public class JsonDataLoader : MonoBehaviour
{
    [SerializeField]
    private string fileName;

    public MaintenanceData LoadedData;

    public event System.Action<MaintenanceData> OnDataLoaded;
    public event System.Action<string> OnDataLoadFailed;
    private IAppLogger logger;

    public void InitializeLogger(IAppLogger appLogger)
    {
        logger = appLogger;
    }

    public void BeginLoad()
    {
        StartCoroutine(LoadData());
    }

    private IEnumerator LoadData()
    {
        // as i use mac , i need to use Sytem.IO , not directly Application.streamingAssetsPath
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);

        path = new System.Uri(path).AbsoluteUri;

        using UnityWebRequest request = UnityWebRequest.Get(path);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            string error = $"JSON loading failed: {request.error}";
            logger.LogError(error);
            OnDataLoadFailed?.Invoke(error);
            yield break;
        }

        string json = request.downloadHandler.text;

        try
        {
            LoadedData =
                JsonConvert.DeserializeObject<MaintenanceData>(json);
        }
        catch (JsonException ex)
        {
            LoadedData = null;

            string error = $"JSON parsing failed: {ex.Message}";
            logger.LogError(error);
            OnDataLoadFailed?.Invoke(error);
            yield break;
        }

        if (!ValidateData(LoadedData))
        {
            LoadedData = null;

            string error = "Maintenance JSON validation failed.";
            logger.LogError(error);
            OnDataLoadFailed?.Invoke(error);
            yield break;
        }

        logger.Log($"Loaded equipment: {LoadedData.equipment.name}");
        logger.Log($"Procedures: {LoadedData.procedures.Count}");

        OnDataLoaded?.Invoke(LoadedData);
    }

    private bool ValidateData(MaintenanceData data)
    {
        if (data == null || data.equipment == null)
            return false;

        if (string.IsNullOrWhiteSpace(data.equipment.id) ||
            string.IsNullOrWhiteSpace(data.equipment.name))
            return false;

        if (data.equipment.components == null ||
            data.equipment.components.Count == 0 ||
            data.procedures == null ||
            data.procedures.Count == 0 ||
            data.checklistItems == null)
            return false;

        var componentIds = new HashSet<string>();
        var procedureIds = new HashSet<string>();
        var stepIds = new HashSet<string>();
        var checklistIds = new HashSet<string>();

        foreach (var c in data.equipment.components)
        {
            if (c == null ||
                string.IsNullOrWhiteSpace(c.id) ||
                string.IsNullOrWhiteSpace(c.name) ||
                !componentIds.Add(c.id))
                return false;
        }

        foreach (var item in data.checklistItems)
        {
            if (item == null ||
                string.IsNullOrWhiteSpace(item.id) ||
                string.IsNullOrWhiteSpace(item.description) ||
                !checklistIds.Add(item.id))
                return false;
        }

        foreach (var p in data.procedures)
        {
            if (p == null ||
                string.IsNullOrWhiteSpace(p.id) ||
                string.IsNullOrWhiteSpace(p.name) ||
                p.steps == null ||
                p.steps.Count == 0 ||
                !procedureIds.Add(p.id))
                return false;

            foreach (var step in p.steps)
            {
                if (step == null ||
                    string.IsNullOrWhiteSpace(step.id) ||
                    string.IsNullOrWhiteSpace(step.instruction) ||
                    !stepIds.Add(step.id))
                    return false;

                if (!componentIds.Contains(step.componentId))
                    return false;

                if (!checklistIds.Contains(step.checklistItemId))
                    return false;
            }
        }

        return true;
    }
}
