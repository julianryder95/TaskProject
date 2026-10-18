
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class JsonDataLoader : MonoBehaviour
{
    [SerializeField]
    private string fileName;

    public MaintenanceData LoadedData;

    public event Action<MaintenanceData> OnDataLoaded;
    public event Action<string> OnDataLoadFailed;

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
        string path = Path.Combine(
            Application.streamingAssetsPath,
            fileName);

        path = new Uri(path).AbsoluteUri;

        using UnityWebRequest request = UnityWebRequest.Get(path);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            string error = $"JSON loading failed: {request.error}";
            ReportLoadFailure(error);
            yield break;
        }

        MaintenanceData data


             = JsonConvert.DeserializeObject<MaintenanceData>(
                request.downloadHandler.text);


        if (!ValidateData(data))
        {
            ReportLoadFailure("Maintenance JSON validation failed.");
            yield break;
        }

        LoadedData = data;

        logger?.Log($"Loaded equipment: {LoadedData.equipment.name}");
        logger?.Log($"Procedures: {LoadedData.procedures.Count}");

        OnDataLoaded?.Invoke(LoadedData);
    }

    private void ReportLoadFailure(string error)
    {
        LoadedData = null;

        logger?.LogError(error);
        OnDataLoadFailed?.Invoke(error);
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

        foreach (var component in data.equipment.components)
        {
            if (component == null ||
                string.IsNullOrWhiteSpace(component.id) ||
                string.IsNullOrWhiteSpace(component.name) ||
                !componentIds.Add(component.id))
            {
                return false;
            }
        }

        foreach (var item in data.checklistItems)
        {
            if (item == null ||
                string.IsNullOrWhiteSpace(item.id) ||
                string.IsNullOrWhiteSpace(item.description) ||
                !checklistIds.Add(item.id))
            {
                return false;
            }
        }

        foreach (var procedure in data.procedures)
        {
            if (procedure == null ||
                string.IsNullOrWhiteSpace(procedure.id) ||
                string.IsNullOrWhiteSpace(procedure.name) ||
                procedure.steps == null ||
                procedure.steps.Count == 0 ||
                !procedureIds.Add(procedure.id))
            {
                return false;
            }

            foreach (var step in procedure.steps)
            {
                if (step == null ||
                    string.IsNullOrWhiteSpace(step.id) ||
                    string.IsNullOrWhiteSpace(step.instruction) ||
                    !stepIds.Add(step.id))
                {
                    return false;
                }

                if (step.componentIds == null ||
                    step.componentIds.Count == 0)
                {
                    return false;
                }

                var stepComponentIds = new HashSet<string>();

                foreach (var componentId in step.componentIds)
                {
                    if (string.IsNullOrWhiteSpace(componentId) ||
                        !componentIds.Contains(componentId) ||
                        !stepComponentIds.Add(componentId))
                    {
                        return false;
                    }
                }

                if (string.IsNullOrWhiteSpace(step.checklistItemId) ||
                    !checklistIds.Contains(step.checklistItemId))
                {
                    return false;
                }
            }
        }

        return true;
    }
}
