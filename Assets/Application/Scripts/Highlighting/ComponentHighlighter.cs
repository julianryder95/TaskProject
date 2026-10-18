using System.Collections.Generic;
using UnityEngine;

public class ComponentHighlighter : MonoBehaviour, IComponentHighlighter
{
    [SerializeField] private string highlightLayer = "highlight";
    [SerializeField] private string normalLayer = "Default";

    private readonly List<GameObject> activeHighlightedObjects = new List<GameObject>();
    private int highlightLayerId;
    private int normalLayerId;

    private void Awake()
    {
        highlightLayerId = LayerMask.NameToLayer(highlightLayer);
        normalLayerId = LayerMask.NameToLayer(normalLayer);

        if (highlightLayerId == -1)
        {
            Debug.LogWarning($"[ComponentHighlighter] Highlight layer '{highlightLayer}' not defined in TagManager. Falling back to default.");
        }

        if (normalLayerId == -1)
        {
            normalLayerId = 0; // Layer 0 = Default
        }
    }

    public void Highlight(GameObject component)
    {
        if (component == null)
            return;

        if (highlightLayerId != -1)
        {
            SetLayerRecursively(component, highlightLayerId);
        }

        if (!activeHighlightedObjects.Contains(component))
        {
            activeHighlightedObjects.Add(component);
        }
    }

    public void ClearHighlight()
    {
        if (normalLayerId != -1)
        {
            foreach (var comp in activeHighlightedObjects)
            {
                if (comp != null)
                {
                    SetLayerRecursively(comp, normalLayerId);
                }
            }
        }

        activeHighlightedObjects.Clear();
    }

    private void SetLayerRecursively(GameObject obj, int targetLayer)
    {
        if (obj == null)
            return;

        obj.layer = targetLayer;
        Transform t = obj.transform;
        int childCount = t.childCount;
        for (int i = 0; i < childCount; i++)
        {
            SetLayerRecursively(t.GetChild(i).gameObject, targetLayer);
        }
    }

    private void OnDisable()
    {
        ClearHighlight();
    }

    private void OnDestroy()
    {
        ClearHighlight();
    }
}
