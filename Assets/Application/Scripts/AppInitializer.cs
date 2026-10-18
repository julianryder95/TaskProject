
using UnityEngine;

public class AppInitializer : MonoBehaviour
{
    [SerializeField] private JsonDataLoader dataLoader;

    public IProcedureEngine ProcedureEngine { get; private set; }

    private IAppLogger logger;

    private void Awake()
    {
        dataLoader = GetComponent<JsonDataLoader>();
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
        ProcedureEngine = new ProcedureEngine(data);
        logger.Log("Procedure engine initialized.");
    }



    private void HandleDataLoadFailed(string error)
    {
        ProcedureEngine = null;
        logger.LogError("Application initialization failed.");
    }
}
