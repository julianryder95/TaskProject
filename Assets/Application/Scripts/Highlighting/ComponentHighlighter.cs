
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ComponentHighlighter : MonoBehaviour, IComponentHighlighter
{
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private Vector3 boundsPadding = new Vector3(0.02f, 0.02f, 0.02f);

    private readonly Dictionary<GameObject, GameObject> boundsBoxes = new();
    private readonly HashSet<GameObject> activeComponents = new();

    public void Highlight(GameObject component)
    {
        if (component == null || outlineMaterial == null)
            return;

        if (!boundsBoxes.TryGetValue(component, out var box))
        {
            box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.name = component.name + "_HighlightBounds";

            var collider = box.GetComponent<Collider>();

            if (collider != null)
                Destroy(collider);

            var renderer = box.GetComponent<MeshRenderer>();
            renderer.sharedMaterial = outlineMaterial;
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;

            boundsBoxes.Add(component, box);
        }

        if (!CalculateBounds(component, out var bounds))
        {
            box.SetActive(false);
            return;
        }

        box.transform.SetPositionAndRotation(bounds.center, Quaternion.identity);
        box.transform.localScale = bounds.size + boundsPadding;
        box.SetActive(true);

        activeComponents.Add(component);
    }

    private bool CalculateBounds(GameObject component, out Bounds bounds)
    {
        var renderers = component.GetComponentsInChildren<MeshRenderer>(true);

        bounds = default;
        bool hasBounds = false;

        foreach (var meshRenderer in renderers)
        {
            if (!meshRenderer.enabled)
                continue;

            if (!hasBounds)
            {
                bounds = meshRenderer.bounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(meshRenderer.bounds);
            }
        }

        return hasBounds;
    }

    public void ClearHighlight()
    {
        foreach (var component in activeComponents)
        {
            if (component != null &&
                boundsBoxes.TryGetValue(component, out var box) &&
                box != null)
            {
                box.SetActive(false);
            }
        }

        activeComponents.Clear();
    }

    private void OnDestroy()
    {
        foreach (var box in boundsBoxes.Values)
        {
            if (box != null)
                Destroy(box);
        }

        boundsBoxes.Clear();
        activeComponents.Clear();
    }
}
