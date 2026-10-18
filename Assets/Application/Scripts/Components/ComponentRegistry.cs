using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Runtime registry that maps JSON component IDs to physical 3D model transforms and annotation labels.
/// Automatically prepares mesh colliders for interactive AR touch selection.
/// </summary>
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
    private Dictionary<GameObject, string> reverseLookup;

    public IReadOnlyList<ComponentMapping> Mappings => mappings;

    private void Awake()
    {
        BuildLookup();
        EnsureColliders();
    }

    private void BuildLookup()
    {
        componentLookup = new Dictionary<string, GameObject>();
        labelLookup = new Dictionary<string, GameObject>();
        reverseLookup = new Dictionary<GameObject, string>();

        foreach (var mapping in mappings)
        {
            if (string.IsNullOrWhiteSpace(mapping.componentId))
                continue;

            if (mapping.componentObject == null)
                continue;

            if (componentLookup.ContainsKey(mapping.componentId))
            {
                Debug.LogError($"[ComponentRegistry] Duplicate component mapping: {mapping.componentId}");
                continue;
            }

            componentLookup.Add(mapping.componentId, mapping.componentObject);
            reverseLookup[mapping.componentObject] = mapping.componentId;

            // Map all child transforms so hits on child sub-meshes resolve to this component
            foreach (Transform child in mapping.componentObject.transform)
            {
                reverseLookup[child.gameObject] = mapping.componentId;
            }

            if (mapping.labelObject != null)
            {
                labelLookup.Add(mapping.componentId, mapping.labelObject);
                mapping.labelObject.SetActive(false);
            }
        }
    }

    private void EnsureColliders()
    {
        foreach (var mapping in mappings)
        {
            if (mapping.componentObject == null)
                continue;

            if (mapping.componentObject.GetComponent<Collider>() == null)
            {
                var meshFilter = mapping.componentObject.GetComponent<MeshFilter>();
                if (meshFilter != null && meshFilter.sharedMesh != null)
                {
                    var mc = mapping.componentObject.AddComponent<MeshCollider>();
                    mc.sharedMesh = meshFilter.sharedMesh;
                }
                else
                {
                    mapping.componentObject.AddComponent<BoxCollider>();
                }
            }
        }
    }

    public GameObject GetComponentObject(string componentId)
    {
        if (TryGetComponentObject(componentId, out var componentObject))
        {
            return componentObject;
        }

        return null;
    }

    public bool TryGetComponentObject(string componentId, out GameObject componentObject)
    {
        componentObject = null;
        if (string.IsNullOrWhiteSpace(componentId))
            return false;

        if (componentLookup == null)
            BuildLookup();

        return componentLookup.TryGetValue(componentId, out componentObject);
    }

    public bool TryGetComponentLabel(string componentId, out GameObject labelObject)
    {
        labelObject = null;
        if (string.IsNullOrWhiteSpace(componentId))
            return false;

        if (labelLookup == null)
            BuildLookup();

        return labelLookup.TryGetValue(componentId, out labelObject);
    }

    public bool TryGetComponentIdFromObject(GameObject obj, out string componentId)
    {
        componentId = null;
        if (obj == null)
            return false;

        if (reverseLookup == null)
            BuildLookup();

        if (reverseLookup.TryGetValue(obj, out componentId))
            return true;

        // Check parent hierarchy if hit collider was on a child
        Transform current = obj.transform.parent;
        while (current != null && current != transform)
        {
            if (reverseLookup.TryGetValue(current.gameObject, out componentId))
                return true;
            current = current.parent;
        }

        return false;
    }


}