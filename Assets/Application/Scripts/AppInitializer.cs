
using UnityEngine;

public class AppInitializer : MonoBehaviour
{
    [SerializeField] private JsonDataLoader dataLoader;
    [SerializeField] private ChecklistController checklistController;
    public IProcedureEngine ProcedureEngine { get; private set; }

    private IAppLogger logger;

    private void Awake()
    {
        dataLoader = GetComponent<JsonDataLoader>();
        checklistController = GetComponent<ChecklistController>();
        logger = new AppLogger();
        dataLoader.InitializeLogger(logger);
        dataLoader.BeginLoad();

    }

    private void OnEnable()
    {
        dataLoader.OnDataLoaded += HandleDataLoaded;
        dataLoader.OnDataLoadFailed += HandleDataLoadFailed;
    }



    private void OnDisable()
    {
        dataLoader.OnDataLoaded -= HandleDataLoaded;
        dataLoader.OnDataLoadFailed -= HandleDataLoadFailed;
    }

    private void HandleDataLoaded(MaintenanceData data)
    {
        var procedure = data.procedures[0];

        ProcedureEngine = new ProcedureEngine(data);

        if (!ProcedureEngine.StartProcedure(procedure.id))
        {
            ProcedureEngine = null;
            logger?.LogError($"Failed to start procedure: {procedure.id}");
            return;
        }

        checklistController.Initialize(data, ProcedureEngine, logger);
    }



    private void HandleDataLoadFailed(string error)
    {
        ProcedureEngine = null;
        logger.LogError("Application initialization failed.");
    }
}
