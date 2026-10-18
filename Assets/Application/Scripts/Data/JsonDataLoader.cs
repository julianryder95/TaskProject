
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

        MaintenanceData data = JsonConvert.DeserializeObject<MaintenanceData>(
                request.downloadHandler.text);




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


}
