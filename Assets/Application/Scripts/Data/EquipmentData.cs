using System;
using System.Collections.Generic;

/// <summary>
/// Root data container representing the full maintenance payload loaded from JSON.
/// Designed for extensibility across multiple industrial equipment types, procedures, and checklists.
/// </summary>
[Serializable]
public class MaintenanceData
{
    public string schemaVersion = "1.1";
    public EquipmentData equipment;
    public List<EquipmentData> additionalEquipment = new();
    public List<ProcedureData> procedures = new();
}

/// <summary>
/// Metadata and component breakdown for a specific piece of industrial equipment.
/// </summary>
[Serializable]
public class EquipmentData
{
    public string id;
    public string name;
    public string modelNumber;
    public string description;
    public List<ComponentData> components = new();
}

/// <summary>
/// Individual inspectable component belonging to an equipment assembly.
/// </summary>
[Serializable]
public class ComponentData
{
    public string id;
    public string name;
    public string description;
    public string inspectionTips;
    public string safetyNotes;
}

/// <summary>
/// Structured workflow procedure containing sequential inspection or maintenance steps.
/// </summary>
[Serializable]
public class ProcedureData
{
    public string id;
    public string name;
    public string description;
    public string estimatedDuration;
    public List<ProcedureStepData> steps = new();
}

/// <summary>
/// Single step within a procedure with associated components, audio narration, and safety notes.
/// </summary>
[Serializable]
public class ProcedureStepData
{
    public string id;
    public int stepNumber;
    public string title;
    public string instruction;
    public int audio = -1;
    public string audioClipName;
    public List<string> componentIds = new();
    public string warning;
}

/// <summary>
/// Dynamic checklist item for safety, mechanical, or operational verification.
/// </summary>
[Serializable]
public class ChecklistItemData
{
    public string id;
    public string description;
    public bool isRequired = true;
    public string category = "General";
}
