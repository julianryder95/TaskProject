using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChecklistController : MonoBehaviour
{
    [SerializeField] private List<Toggle> checklistToggles;

    private ChecklistPersistence persistence;
    private ChecklistState checklistState;

    private void Awake()
    {
        Initialize();
    }
    public void Initialize()
    {
        persistence = new ChecklistPersistence();
        checklistState = persistence.Load();

        foreach (var toggle in checklistToggles)
        {
            string id = toggle.gameObject.name;

            var state = checklistState.items.Find(x => x.checklistItemId == id);

            if (state == null)
            {
                state = new ChecklistItemState
                {
                    checklistItemId = id,
                    isCompleted = false
                };

                checklistState.items.Add(state);
            }

            toggle.SetIsOnWithoutNotify(state.isCompleted);

            string checklistId = id;
            toggle.onValueChanged.AddListener(
                isOn => UpdateChecklist(checklistId, isOn));
        }

        persistence.Save(checklistState);
    }

    private void UpdateChecklist(string id, bool isCompleted)
    {
        var state = checklistState.items.Find(x => x.checklistItemId == id);

        if (state == null)
            return;

        state.isCompleted = isCompleted;
        persistence.Save(checklistState);
    }
}