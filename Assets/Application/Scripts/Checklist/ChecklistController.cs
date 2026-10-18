
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

public class ChecklistController : MonoBehaviour
{
    [SerializeField] private Transform checklistContent;
    [SerializeField] private Toggle checklistTogglePrefab;

    [SerializeField] private ChecklistPersistence persistence;
    [SerializeField] private ChecklistState checklistState;
    private IAppLogger logger;

    public void Initialize(MaintenanceData data, IAppLogger appLogger)
    {
        logger = appLogger;
        persistence = new ChecklistPersistence(logger);
        checklistState = persistence.Load();

        foreach (var item in data.checklistItems)
        {
            if (!checklistState.items.Any(x => x.checklistItemId == item.id))
                checklistState.items.Add(new ChecklistItemState { checklistItemId = item.id, isCompleted = false });
        }



        foreach (var item in data.checklistItems)
        {
            Toggle toggle = Instantiate(checklistTogglePrefab, checklistContent);
            toggle.GetComponentInChildren<Text>().text = item.description;
            toggle.isOn = IsCompleted(item.id);

            string itemId = item.id;
            toggle.onValueChanged.AddListener(isOn => SetCompleted(itemId, isOn));
        }

        logger?.Log("Checklist initialized.");
    }

    public bool IsCompleted(string checklistItemId)
    {
        var item = checklistState.items.Find(x => x.checklistItemId == checklistItemId);
        return item != null && item.isCompleted;
    }

    public void SetCompleted(string checklistItemId, bool isCompleted)
    {
        if (checklistState == null)
            return;

        var item = checklistState.items.Find(x => x.checklistItemId == checklistItemId);

        if (item == null)
        {
            logger?.LogWarning($"Checklist item not found: {checklistItemId}");
            return;
        }

        item.isCompleted = isCompleted;
        persistence.Save(checklistState);
    }
}
