using System;
using System.Collections.Generic;

[Serializable]
public class ChecklistState
{
    public List<ChecklistItemState> items = new List<ChecklistItemState>();
}

[Serializable]
public class ChecklistItemState
{
    public string checklistItemId;
    public bool isCompleted;
}