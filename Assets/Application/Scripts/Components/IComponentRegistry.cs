using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Registry mapping logical data IDs (from JSON) to physical 3D GameObjects and UI annotation labels.
/// </summary>
public interface IComponentRegistry
{
    GameObject GetComponentObject(string componentId);
    bool TryGetComponentObject(string componentId, out GameObject componentObject);
    bool TryGetComponentLabel(string componentId, out GameObject labelObject);
    bool TryGetComponentIdFromObject(GameObject obj, out string componentId);
}