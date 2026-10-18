using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ARObjectSpawner : MonoBehaviour
{
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private GameObject objectToSpawn;

    [SerializeField] private AppInitializer appInitializer;
    [SerializeField] private ProcedureStepController procedureStepController;

    private static readonly List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private bool isPlaced;
    private IAppLogger logger;
    [SerializeField] private ARPlaneManager arPlaneManager;
    [SerializeField]
    private GameObject stepUI;

    private void Awake()
    {
        logger = new AppLogger();
    }

    private void Start()
    {
        if (raycastManager == null)
            raycastManager = GetComponent<ARRaycastManager>();

        HideExistingPlanes();
    }

    private void Update()
    {

        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            return;
        }

        if (touch.phase != TouchPhase.Began)
            return;

        if (isPlaced)
            return;

        if (raycastManager.Raycast(
                touch.position,
                hits,
                TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            GameObject spawnedObject =
                Instantiate(
                    objectToSpawn,
                    hitPose.position,
                    new Quaternion(-90, 0, 0, 0));

            InitializeProcedure(spawnedObject);
            HideExistingPlanes();
            isPlaced = true;
        }
    }

    private void InitializeProcedure(GameObject spawnedObject = null)
    {
        stepUI.SetActive(true);
        var registry =
            spawnedObject.GetComponent<ComponentRegistry>();

        var highlighter =
            spawnedObject.GetComponent<ComponentHighlighter>();

        if (registry == null || highlighter == null)
        {
            logger.LogError(
                "ARObjectSpawner: ComponentRegistry or " +
                "ComponentHighlighter missing from spawned pump.");
            return;
        }

        procedureStepController.Initialize(appInitializer.ProcedureEngine, registry, highlighter, logger);
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
        }
    }
}