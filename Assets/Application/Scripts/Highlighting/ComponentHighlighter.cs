
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ComponentHighlighter : MonoBehaviour, IComponentHighlighter
{
    [SerializeField] private Material outlineMaterial;

    private Dictionary<GameObject, List<GameObject>> overlays = new Dictionary<GameObject, List<GameObject>>();
    private HashSet<GameObject> activeComponents = new HashSet<GameObject>();

    public void Highlight(GameObject component)
    {
        if (component == null || outlineMaterial == null)
            return;

        if (!overlays.TryGetValue(component, out var objects))
        {
            objects = CreateOverlays(component);
            overlays.Add(component, objects);
        }

        foreach (var overlay in objects)
            overlay.SetActive(true);

        activeComponents.Add(component);
    }

    public void ClearHighlight()
    {
        foreach (var component in activeComponents)
        {
            if (component != null &&
                overlays.TryGetValue(component, out var objects))
            {
                foreach (var overlay in objects)
                {
                    if (overlay != null)
                        overlay.SetActive(false);
                }
            }
        }

        activeComponents.Clear();
    }

    private List<GameObject> CreateOverlays(GameObject component)
    {
        var result = new List<GameObject>();
        var renderers = component.GetComponentsInChildren<MeshRenderer>(true);

        foreach (var source in renderers)
        {
            var filter = source.GetComponent<MeshFilter>();

            if (filter == null || filter.sharedMesh == null)
                continue;

            var overlay = new GameObject(source.name + "_Outline");

            overlay.transform.SetParent(source.transform.parent, false);
            overlay.transform.SetLocalPositionAndRotation(
                source.transform.localPosition,
                source.transform.localRotation);
            overlay.transform.localScale = source.transform.localScale;
            overlay.layer = source.gameObject.layer;

            overlay.AddComponent<MeshFilter>().sharedMesh = filter.sharedMesh;

            var renderer = overlay.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = outlineMaterial;
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;

            overlay.SetActive(false);
            result.Add(overlay);
        }

        return result;
    }

    private void OnDestroy()
    {
        foreach (var objects in overlays.Values)
        {
            foreach (var overlay in objects)
            {
                if (overlay != null)
                    Destroy(overlay);
            }
        }
    }
}
