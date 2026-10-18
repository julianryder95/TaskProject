using UnityEngine;

/// <summary>
/// Application bootstrap orchestrator.
/// Manages data loading lifecycle, initializes the procedure engine, and injects dependencies into UI controllers.
/// </summary>
public class AppInitializer : MonoBehaviour
{
    [SerializeField] private JsonDataLoader dataLoader;
    [SerializeField] private ChecklistController checklistController;

    public IProcedureEngine ProcedureEngine { get; private set; }
    public IAppLogger Logger => logger;

    private IAppLogger logger;

    private void Awake()
    {
        logger = new AppLogger();

        if (dataLoader == null)
        {
            dataLoader = GetComponent<JsonDataLoader>();
            if (dataLoader == null)
            {
                dataLoader = gameObject.AddComponent<JsonDataLoader>();
            }
        }

        if (checklistController == null)
        {
            checklistController = FindFirstObjectByType<ChecklistController>(FindObjectsInactive.Include);
        }

        dataLoader.InitializeLogger(logger);
        dataLoader.BeginLoad();
    }

    private void OnEnable()
    {
        if (dataLoader != null)
        {
            dataLoader.OnDataLoaded += HandleDataLoaded;
            dataLoader.OnDataLoadFailed += HandleDataLoadFailed;
        }
    }

    private void OnDisable()
    {
        if (dataLoader != null)
        {
            dataLoader.OnDataLoaded -= HandleDataLoaded;
            dataLoader.OnDataLoadFailed -= HandleDataLoadFailed;
        }
    }

    private void HandleDataLoaded(MaintenanceData data)
    {
        if (data?.procedures == null || data.procedures.Count == 0)
        {
            logger?.LogError("[AppInitializer] No valid procedures defined in loaded maintenance payload.");
            return;
        }



        // Initialize procedure engine with the first procedure
        var initialProcedure = data.procedures[0];
        ProcedureEngine = new ProcedureEngine(data);

        if (!ProcedureEngine.StartProcedure(initialProcedure.id))
        {
            logger?.LogError($"[AppInitializer] Failed to start procedure: {initialProcedure.id}");
            ProcedureEngine = null;
            return;
        }

        logger?.Log($"[AppInitializer] Initialized ProcedureEngine with procedure '{initialProcedure.name}' ({initialProcedure.steps.Count} steps).");
    }

    private void HandleDataLoadFailed(string error)
    {
        ProcedureEngine = null;
        logger?.LogError($"[AppInitializer] Application initialization failed: {error}");
    }
}
