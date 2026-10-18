using UnityEngine;

public interface IComponentHighlighter
{
    void Highlight(GameObject component);
    void ClearHighlight();
}