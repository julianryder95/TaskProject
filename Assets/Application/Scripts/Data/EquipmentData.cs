
using System;
using System.Collections.Generic;

[Serializable]
public class MaintenanceData
{
    public EquipmentData equipment;
    public List<ProcedureData> procedures;

}

[Serializable]
public class EquipmentData
{
    public string id;
    public string name;
    public List<ComponentData> components;
}

[Serializable]
public class ComponentData
{
    public string id;
    public string name;
    public string description;
}

[Serializable]
public class ProcedureData
{
    public string id;
    public string name;
    public List<ProcedureStepData> steps;
}

[Serializable]
public class ProcedureStepData
{
    public string id;
    public string instruction;
    public List<string> componentIds;
    public List<string> checklistItemIds;
}


