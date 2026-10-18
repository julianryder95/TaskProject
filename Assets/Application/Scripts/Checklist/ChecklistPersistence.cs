using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class ChecklistPersistence
{
    private string filePath;
    private IAppLogger logger;

    public ChecklistPersistence(IAppLogger appLogger)
    {
        logger = appLogger;
        filePath = Path.Combine(Application.persistentDataPath, "ChecklistState.json");
    }

    public void Save(ChecklistState state)
    {


        string json = JsonConvert.SerializeObject(state, Formatting.Indented);
        File.WriteAllText(filePath, json);

        logger?.Log("Checklist state saved.");

    }

    public ChecklistState Load()
    {
        if (!File.Exists(filePath))
            return new ChecklistState();


        string json = File.ReadAllText(filePath);

        return JsonConvert.DeserializeObject<ChecklistState>(json);

    }

    public void Clear()
    {
        if (!File.Exists(filePath))
            return;

        File.Delete(filePath);
    }
}