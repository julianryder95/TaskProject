using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controls the step-by-step procedure UI, component highlighting synchronization,
/// world-space label activation, and audio narration triggers.
/// </summary>
public class ProcedureStepController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private TextMeshProUGUI stepProgressText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;

    [Header("Audio Narration")]
    [SerializeField] private AudioPlayer audioPlayer;

    private IProcedureEngine procedureEngine;
    private IComponentRegistry componentRegistry;
    private IComponentHighlighter highlighter;
    private IAppLogger logger;

    private readonly List<GameObject> activeLabels = new();

    private void Awake()
    {
        if (audioPlayer == null)
        {
            audioPlayer = GetComponent<AudioPlayer>();
        }
    }

    public void Initialize(
        IProcedureEngine engine,
        IComponentRegistry registry,
        IComponentHighlighter componentHighlighter,
        IAppLogger appLogger)
    {
        procedureEngine = engine;
        componentRegistry = registry;
        highlighter = componentHighlighter;
        logger = appLogger;

        if (audioPlayer == null)
        {
            audioPlayer = GetComponent<AudioPlayer>();
        }

        ShowCurrentStep();
    }

    public void OnNextStepButtonPressed()
    {
        if (procedureEngine == null)
            return;

        if (procedureEngine.MoveNext())
        {
            ShowCurrentStep();
        }
    }

    public void OnPreviousStepButtonPressed()
    {
        if (procedureEngine == null)
            return;

        if (procedureEngine.MovePrevious())
        {
            ShowCurrentStep();
        }
    }

    public void OnRestartProcedure()
    {
        if (procedureEngine == null)
            return;

        procedureEngine.Restart();
        ShowCurrentStep();
    }

    private void ShowCurrentStep()
    {
        ClearCurrentStep();

        if (procedureEngine == null)
        {
            if (instructionText != null)
                instructionText.text = "Initializing procedure engine...";
            return;
        }

        if (procedureEngine.IsCompleted)
        {
            if (instructionText != null)
            {
                instructionText.text = "<b><color=#2E7D32>Procedure Completed!</color></b>\n\nAll inspection steps verified successfully.";
            }

            if (stepProgressText != null)
            {
                stepProgressText.text = "Status: Completed";
            }

            audioPlayer.Stop();
            UpdateNavigationButtonStates();
            return;
        }

        var step = procedureEngine.CurrentStep;
        if (step == null)
        {
            if (instructionText != null)
                instructionText.text = "No active procedure step.";
            return;
        }

        // Format instructional display
        string title = !string.IsNullOrWhiteSpace(step.title)
            ? step.title
            : $"Step {procedureEngine.CurrentStepIndex + 1}";

        string formattedText = $"<b><size=110%>{title}</size></b> (Step {procedureEngine.CurrentStepIndex + 1}/{procedureEngine.TotalSteps})\n\n{step.instruction}";

        if (!string.IsNullOrWhiteSpace(step.warning))
        {
            formattedText += $"\n\n<color=#D84315><b>Precaution:</b> {step.warning}</color>";
        }

        if (instructionText != null)
        {
            instructionText.text = formattedText;
        }

        if (stepProgressText != null)
        {
            stepProgressText.text = $"Step {procedureEngine.CurrentStepIndex + 1} of {procedureEngine.TotalSteps}";
        }

        // Activate step component highlighting and 3D labels
        if (step.componentIds != null)
        {
            foreach (string componentId in step.componentIds)
            {
                ActivateComponent(componentId);
            }
        }

        // Trigger corresponding audio narration voiceover
        if (audioPlayer != null && step.audio >= 0)
        {
            audioPlayer.PlayClip(step.audio);
        }

        UpdateNavigationButtonStates();
    }

    private void ActivateComponent(string componentId)
    {
        if (componentRegistry == null)
            return;

        if (componentRegistry.TryGetComponentObject(componentId, out var component))
        {
            highlighter?.Highlight(component);
        }
        else
        {
            logger?.LogError($"[ProcedureStepController] Component mapping not found in registry: {componentId}");
        }

        if (componentRegistry.TryGetComponentLabel(componentId, out var label))
        {
            label.SetActive(true);
            activeLabels.Add(label);
        }
    }

    private void ClearCurrentStep()
    {
        highlighter?.ClearHighlight();

        foreach (var label in activeLabels)
        {
            if (label != null)
            {
                label.SetActive(false);
            }
        }
        activeLabels.Clear();
    }

    private void UpdateNavigationButtonStates()
    {
        if (previousButton != null && procedureEngine != null)
        {
            previousButton.interactable = procedureEngine.CanMovePrevious;
        }

        if (nextButton != null && procedureEngine != null)
        {
            nextButton.interactable = procedureEngine.CanMoveNext;
        }
    }


    public void DisplayComponentDetails(string componentId, ComponentData data)
    {
        ClearCurrentStep();
        ActivateComponent(componentId);

        if (instructionText != null && data != null)
        {
            string detailText = $"<b><size=115%>{data.name}</size></b> (Component Inspection)\n\n{data.description}";
            if (!string.IsNullOrWhiteSpace(data.inspectionTips))
            {
                detailText += $"\n\n<b>Inspection Tips:</b> {data.inspectionTips}";
            }
            if (!string.IsNullOrWhiteSpace(data.safetyNotes))
            {
                detailText += $"\n<color=#D84315>{data.safetyNotes}</color>";
            }
            detailText += "\n\n<i>(Tap 'Next' or 'Previous' to resume workflow)</i>";
            instructionText.text = detailText;
        }
    }
}