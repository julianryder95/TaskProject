using System.Collections.Generic;
using UnityEngine;

public class ComponentRegistry : MonoBehaviour, IComponentRegistry
{
    [System.Serializable]
    public class ComponentMapping
    {
        public string componentId;
        public GameObject componentObject;
        public GameObject labelObject;
    }

    [SerializeField]
    private List<ComponentMapping> mappings = new();

    private Dictionary<string, GameObject> componentLookup;
    private Dictionary<string, GameObject> labelLookup;

    private void Awake()
    {
        BuildLookup();
    }

    private void BuildLookup()
    {
        componentLookup = new Dictionary<string, GameObject>();
        labelLookup = new Dictionary<string, GameObject>();

        foreach (var mapping in mappings)
        {
            if (string.IsNullOrWhiteSpace(mapping.componentId))
                continue;

            if (mapping.componentObject == null)
                continue;

            if (componentLookup.ContainsKey(mapping.componentId))
            {
                Debug.LogError(
                    $"Duplicate component mapping: {mapping.componentId}");

                continue;
            }

            componentLookup.Add(
                mapping.componentId,
                mapping.componentObject);

            if (mapping.labelObject != null)
            {
                labelLookup.Add(
                    mapping.componentId,
                    mapping.labelObject);

                mapping.labelObject.SetActive(false);
            }
        }
    }

    public GameObject GetComponentObject(string componentId)
    {
        if (TryGetComponentObject(
            componentId,
            out var componentObject))
        {
            return componentObject;
        }

        return null;
    }

    public bool TryGetComponentObject(
        string componentId,
        out GameObject componentObject)
    {
        componentObject = null;

        if (componentLookup == null)
            BuildLookup();

        return componentLookup.TryGetValue(
            componentId,
            out componentObject);
    }

    public bool TryGetComponentLabel(
        string componentId,
        out GameObject labelObject)
    {
        labelObject = null;

        if (labelLookup == null)
            BuildLookup();

        return labelLookup.TryGetValue(
            componentId,
            out labelObject);
    }
}