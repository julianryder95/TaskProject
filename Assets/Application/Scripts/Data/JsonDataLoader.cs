using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

/// <summary>
/// Asynchronously loads and parses maintenance JSON payloads from StreamingAssets across Editor and mobile Android platforms.
/// Validates data integrity before dispatching to the procedure engine and UI controllers.
/// </summary>
public class JsonDataLoader : MonoBehaviour
{
    [SerializeField]
    private string fileName = "PayloadSteps.json";

    public MaintenanceData LoadedData { get; private set; }

    public event Action<MaintenanceData> OnDataLoaded;
    public event Action<string> OnDataLoadFailed;

    private IAppLogger logger;

    public void InitializeLogger(IAppLogger appLogger)
    {
        logger = appLogger;
    }

    public void BeginLoad()
    {
        StartCoroutine(LoadData(fileName));
    }

    public void LoadFile(string targetFileName)
    {
        if (string.IsNullOrWhiteSpace(targetFileName))
        {
            ReportLoadFailure("Cannot load data: Target file name is null or empty.");
            return;
        }

        fileName = targetFileName;
        StartCoroutine(LoadData(fileName));
    }

    private IEnumerator LoadData(string targetFile)
    {
        if (string.IsNullOrWhiteSpace(targetFile))
        {
            targetFile = "PayloadSteps.json";
        }

        string fullPath = Path.Combine(Application.streamingAssetsPath, targetFile);

        if (!fullPath.Contains("://"))
        {
            fullPath = new Uri(fullPath).AbsoluteUri;
        }

        logger?.Log($"[JsonDataLoader] Requesting payload from: {fullPath}");

        using UnityWebRequest request = UnityWebRequest.Get(fullPath);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            string error = $"[JsonDataLoader] HTTP/IO error loading '{targetFile}': {request.error}";
            ReportLoadFailure(error);
            yield break;
        }

        string jsonText = request.downloadHandler.text;
        if (string.IsNullOrWhiteSpace(jsonText))
        {
            ReportLoadFailure($"[JsonDataLoader] File '{targetFile}' contains empty or whitespace content.");
            yield break;
        }

        MaintenanceData data;
        try
        {
            data = JsonConvert.DeserializeObject<MaintenanceData>(jsonText);
        }
        catch (Exception ex)
        {
            ReportLoadFailure($"[JsonDataLoader] JSON Deserialization error in '{targetFile}': {ex.Message}");
            yield break;
        }

        if (data == null || data.equipment == null)
        {
            ReportLoadFailure($"[JsonDataLoader] Invalid schema in '{targetFile}': Missing equipment definition.");
            yield break;
        }

        LoadedData = data;
        logger?.Log($"[JsonDataLoader] Successfully loaded equipment '{LoadedData.equipment.name}' with {LoadedData.procedures?.Count ?? 0} procedure(s).");

        OnDataLoaded?.Invoke(LoadedData);
    }

    private void ReportLoadFailure(string error)
    {
        LoadedData = null;
        logger?.LogError(error);
        OnDataLoadFailed?.Invoke(error);
    }
}
