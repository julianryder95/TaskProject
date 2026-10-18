
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

    }

    private void OnEnable()
    {
        dataLoader.OnDataLoaded += HandleDataLoaded;
        dataLoader.OnDataLoadFailed += HandleDataLoadFailed;
    }

    private void Start()
    {
        dataLoader.BeginLoad();
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

        logger?.Log($"Started procedure: {procedure.name}");
        checklistController.Initialize(data, logger);
    }



    private void HandleDataLoadFailed(string error)
    {
        ProcedureEngine = null;
        logger.LogError("Application initialization failed.");
    }
}
