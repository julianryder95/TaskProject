
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ComponentHighlighter : MonoBehaviour, IComponentHighlighter
{
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private Vector3 boundsPadding = new Vector3(0.02f, 0.02f, 0.02f);

    private List<GameObject> activeComponents = new();

    public void Highlight(GameObject component)
    {

        activeComponents.Add(component);
    }



    public void ClearHighlight()
    {
        foreach (var component in activeComponents)
        {

            // box.SetActive(false);

        }

        activeComponents.Clear();
    }

    private void OnDestroy()
    {


        activeComponents.Clear();
    }
}
