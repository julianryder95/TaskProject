
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ComponentHighlighter : MonoBehaviour, IComponentHighlighter
{
    [SerializeField]
    private GameObject componentGameObj;

    [SerializeField] private string highlightLayer = "highlight";
    [SerializeField] private string normalLayer = "Default";

    public void Highlight(GameObject component)
    {
        component.layer = LayerMask.NameToLayer(highlightLayer);
        componentGameObj = component;
    }



    public void ClearHighlight()
    {
        componentGameObj.layer = LayerMask.NameToLayer(normalLayer);

    }

    private void OnDestroy()
    {



    }
}
