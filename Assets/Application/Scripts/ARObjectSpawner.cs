using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;

public class ARObjectSpawner : MonoBehaviour
{

    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private ARPlaneManager arPlaneManager;
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private Camera arCamera;

    [SerializeField] private AppInitializer appInitializer;
    [SerializeField] private ProcedureStepController procedureStepController;
    [SerializeField] private GameObject stepUI;


    [SerializeField] private bool isPlaced;

    private static readonly List<ARRaycastHit> hits = new();
    private IAppLogger logger;
    private ComponentRegistry spawnedRegistry;
    private ComponentHighlighter spawnedHighlighter;
    private GameObject spawnedObject;

    private void Awake()
    {
        logger = new AppLogger();
        Application.targetFrameRate = 60;

        if (arPlaneManager == null)
            arPlaneManager = GetComponent<ARPlaneManager>();

        if (raycastManager == null)
            raycastManager = GetComponent<ARRaycastManager>();

        if (arCamera == null)
            arCamera = Camera.main;
    }

    private void Start()
    {
        if (raycastManager == null)
            raycastManager = GetComponent<ARRaycastManager>();
    }

    private void Update()
    {
        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        // Discard touches over UI elements
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            return;

        if (touch.phase != TouchPhase.Began)
            return;

        // Stage 1: Initial Equipment Placement
        if (!isPlaced)
        {
            HandlePlacementTouch(touch.position);
            return;
        }

        // Stage 2: Interactive Component Selection via AR Raycast
        HandleComponentSelectionTouch(touch.position);
    }

    private void HandlePlacementTouch(Vector2 touchPosition)
    {
        if (raycastManager == null || objectToSpawn == null)
            return;

        if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            // Rotate by -90 degrees around X relative to the detected surface plane normal
            // so the industrial CAD model stands upright on its mounting base
            Quaternion placementRotation = hitPose.rotation * Quaternion.Euler(-90f, 0f, 0f);

            spawnedObject = Instantiate(objectToSpawn, hitPose.position, placementRotation);
            isPlaced = true;

            InitializeProcedure(spawnedObject);
            HideExistingPlanes();
        }
    }

    private void HandleComponentSelectionTouch(Vector2 touchPosition)
    {
        if (spawnedRegistry == null || arCamera == null)
            return;

        Ray ray = arCamera.ScreenPointToRay(touchPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 20f))
        {
            if (spawnedRegistry.TryGetComponentIdFromObject(hit.collider.gameObject, out string componentId))
            {
                logger?.Log($"[ARObjectSpawner] Component tapped in AR: {componentId}");

                var compData = FindComponentData(componentId);
                if (compData != null && procedureStepController != null)
                {
                    procedureStepController.DisplayComponentDetails(componentId, compData);
                }
            }
        }
    }

    private ComponentData FindComponentData(string componentId)
    {
        var maintenanceData = appInitializer?.GetComponent<JsonDataLoader>()?.LoadedData;
        if (maintenanceData?.equipment?.components == null)
            return null;

        return maintenanceData.equipment.components.Find(c => string.Equals(c.id, componentId, System.StringComparison.OrdinalIgnoreCase));
    }

    private void InitializeProcedure(GameObject targetObject)
    {
        if (stepUI != null)
        {
            stepUI.SetActive(true);
        }

        spawnedRegistry = targetObject.GetComponent<ComponentRegistry>();
        spawnedHighlighter = targetObject.GetComponent<ComponentHighlighter>();

        if (spawnedRegistry == null || spawnedHighlighter == null)
        {
            logger?.LogError("[ARObjectSpawner] ComponentRegistry or ComponentHighlighter missing from spawned prefab.");
            return;
        }

        var engine = appInitializer != null ? appInitializer.ProcedureEngine : null;
        procedureStepController.Initialize(engine, spawnedRegistry, spawnedHighlighter, logger);
    }

    public void HideExistingPlanes()
    {
        if (arPlaneManager != null)
        {
            foreach (var plane in arPlaneManager.trackables)
            {
                plane.gameObject.SetActive(false);
            }
            arPlaneManager.enabled = false;
            logger?.Log("[ARObjectSpawner] AR Planes hidden and ARPlaneManager disabled for mobile performance.");
        }
    }
}