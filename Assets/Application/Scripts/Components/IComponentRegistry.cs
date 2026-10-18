using UnityEngine;

public interface IComponentRegistry
{
    GameObject GetComponentObject(string componentId);
    bool TryGetComponentObject(string componentId, out GameObject componentObject);
}