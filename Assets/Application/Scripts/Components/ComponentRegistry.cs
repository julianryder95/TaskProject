using System.Collections.Generic;
using UnityEngine;

public class ComponentRegistry : MonoBehaviour, IComponentRegistry
{
    [System.Serializable]
    public class ComponentMapping
    {
        public string componentId;
        public GameObject componentObject;
    }

    [SerializeField]
    private List<ComponentMapping> mappings = new List<ComponentMapping>();

    private Dictionary<string, GameObject> componentLookup = new Dictionary<string, GameObject>();

    private IAppLogger logger;

    private void Awake()
    {
        logger = new AppLogger();
        BuildLookup();
    }

    private void BuildLookup()
    {
        componentLookup = new Dictionary<string, GameObject>();

        foreach (var mapping in mappings)
        {
            if (string.IsNullOrWhiteSpace(mapping.componentId))
                continue;

            if (mapping.componentObject == null)
                continue;

            if (componentLookup.ContainsKey(mapping.componentId))
            {
                logger.LogError($"Duplicate component mapping: {mapping.componentId}");
                continue;
            }

            componentLookup.Add(mapping.componentId, mapping.componentObject);
        }
    }

    public GameObject GetComponentObject(string componentId)
    {
        if (TryGetComponentObject(componentId, out var componentObject))
            return componentObject;

        return null;
    }


    //for unit testing
    public bool TryGetComponentObject(string componentId, out GameObject componentObject)
    {

        if (componentLookup == null)
            BuildLookup();

        return componentLookup.TryGetValue(componentId, out componentObject);
    }
}