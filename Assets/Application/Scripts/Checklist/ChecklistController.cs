
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChecklistController : MonoBehaviour
{
    [SerializeField] private Transform checklistContent;
    [SerializeField] private Toggle checklistTogglePrefab;

    private ChecklistPersistence persistence;
    private ChecklistState checklistState;
    private MaintenanceData maintenanceData;
    private IProcedureEngine procedureEngine;
    private IAppLogger logger;

    private readonly List<GameObject> spawnedToggles = new();

    public void Initialize(MaintenanceData data, IProcedureEngine engine, IAppLogger appLogger)
    {
        maintenanceData = data;
        procedureEngine = engine;
        logger = appLogger;
        persistence = new ChecklistPersistence(logger);
        checklistState = persistence.Load();

        if (checklistState.items == null)
            checklistState.items = new List<ChecklistItemState>();

        foreach (var item in procedureEngine.CurrentStep.checklistItemIds)
        {
            if (!checklistState.items.Any(x => x.checklistItemId == item))
                checklistState.items.Add(new ChecklistItemState { checklistItemId = item, isCompleted = false });
        }


        ShowStep(procedureEngine.CurrentStep);
    }

    public void ShowStep(ProcedureStepData step)
    {
        ClearToggles();

        if (step?.checklistItemIds == null)
            return;

        foreach (string item in step.checklistItemIds)
        {

            if (string.IsNullOrEmpty(item))
            {
                logger?.LogWarning($"Checklist definition not found: {item}");
                continue;
            }

            Toggle toggle = Instantiate(checklistTogglePrefab, checklistContent);
            spawnedToggles.Add(toggle.gameObject);

            var label = toggle.GetComponentInChildren<Text>();

            if (label != null)
                label.text = item;

            toggle.SetIsOnWithoutNotify(IsCompleted(item));

            string checklistItemId = item;
            toggle.onValueChanged.AddListener(isOn => SetCompleted(checklistItemId, isOn));
        }
    }

    public bool IsCompleted(string checklistItemId)
    {
        var item = checklistState?.items.Find(x => x.checklistItemId == checklistItemId);
        return item != null && item.isCompleted;
    }

    public void SetCompleted(string checklistItemId, bool isCompleted)
    {
        if (checklistState == null)
            return;

        var item = checklistState.items.Find(x => x.checklistItemId == checklistItemId);

        if (item == null)
        {
            logger?.LogWarning($"Checklist item not found: {checklistItemId} and so adding it");
            checklistState.items.Add(new ChecklistItemState { checklistItemId = checklistItemId, isCompleted = isCompleted });
            persistence.Save(checklistState);
            return;
        }

        item.isCompleted = isCompleted;
        persistence.Save(checklistState);
    }

    private void ClearToggles()
    {
        foreach (var toggle in spawnedToggles)
        {
            if (toggle != null)
                Destroy(toggle);
        }

        spawnedToggles.Clear();
    }
}
