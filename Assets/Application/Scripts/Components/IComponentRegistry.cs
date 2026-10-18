using UnityEngine;

public interface IComponentRegistry
{
    GameObject GetComponentObject(string componentId);

    bool TryGetComponentObject(string componentId, out GameObject componentObject);

    bool TryGetComponentLabel(string componentId, out GameObject labelObject);
}