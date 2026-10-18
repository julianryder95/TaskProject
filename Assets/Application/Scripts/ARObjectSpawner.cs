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
    [SerializeField] private bool isPlaced;
    private IAppLogger logger;
    [SerializeField] private ARPlaneManager arPlaneManager;
    [SerializeField] private GameObject stepUI;

    private void Awake()
    {
        logger = new AppLogger();
        arPlaneManager = GetComponent<ARPlaneManager>();
    }

    private void Start()
    {
        if (raycastManager == null)
            raycastManager = GetComponent<ARRaycastManager>();

        HideExistingPlanes();
    }

    private void Update()
    {
        // Prevent multiple spawns once the object is placed
        if (isPlaced)
            return;

        // Ensure there is at least one active touch
        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        // Do not spawn if tapping on UI buttons/panels
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            return;

        // Only trigger on initial touch contact
        if (touch.phase != TouchPhase.Began)
            return;

        if (raycastManager.Raycast(
                touch.position,
                hits,
                TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            // Rotate by -90 degrees around X relative to the detected plane normal so the CAD model stands vertically upright on its base
            Quaternion placementRotation = hitPose.rotation * Quaternion.Euler(-90f, 0f, 0f);

            GameObject spawnedObject =
                Instantiate(
                    objectToSpawn,
                    hitPose.position,
                    placementRotation);

            isPlaced = true;

            InitializeProcedure(spawnedObject);
            HideExistingPlanes();
        }
    }

    private void InitializeProcedure(GameObject spawnedObject = null)
    {
        if (stepUI != null)
            stepUI.SetActive(true);

        if (spawnedObject == null)
            return;

        var registry = spawnedObject.GetComponent<ComponentRegistry>();
        var highlighter = spawnedObject.GetComponent<ComponentHighlighter>();

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
    /* private IEnumerator LoadAndSpawnPump(Pose pose)
     {
         var handle = Addressables.InstantiateAsync(prefabAddress, pose.position, pose.rotation);
         yield return handle;

         if (handle.Status != AsyncOperationStatus.Succeeded)
         {
             logger.LogError($"Failed to load Addressable prefab: {prefabAddress}");
             isPlaced = false;
             yield break;
         }

         GameObject spawnedObject = handle.Result;
         InitializeProcedure(spawnedObject);
     }*/
}