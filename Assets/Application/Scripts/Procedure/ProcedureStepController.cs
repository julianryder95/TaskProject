using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProcedureStepController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI instructionText;
    private IProcedureEngine procedureEngine;
    private IComponentRegistry componentRegistry;
    private IComponentHighlighter highlighter;
    private IAppLogger logger;

    [SerializeField]
    private AudioPlayer audioPlayer;

    private List<GameObject> activeLabels = new();

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

        audioPlayer = GetComponent<AudioPlayer>();

        ShowCurrentStep();
    }

    public void OnNextStepButtonPressed()
    {
        if (procedureEngine == null)
            return;

        if (procedureEngine.MoveNext())
            ShowCurrentStep();
    }

    public void OnPreviousStepButtonPressed()
    {
        if (procedureEngine == null)
            return;

        if (procedureEngine.MovePrevious())
            ShowCurrentStep();
    }

    private void ShowCurrentStep()
    {
        ClearCurrentStep();

        var step = procedureEngine.CurrentStep;


        if (step == null)
        {
            if (instructionText != null)
            {
                instructionText.text =
                    procedureEngine.IsCompleted
                        ? "Procedure completed!"
                        : "No active procedure.";
            }

            return;
        }

        audioPlayer.Play(audioPlayer.audioClips[step.audio]);

        if (instructionText != null)
            instructionText.text = step.instruction;

        foreach (string componentId in step.componentIds)
        {
            ActivateComponent(componentId);
        }
    }

    private void ActivateComponent(string componentId)
    {
        if (componentRegistry.TryGetComponentObject(
            componentId,
            out var component))
        {
            highlighter.Highlight(component);
        }
        else
        {
            logger?.LogError(
                $"Component mapping not found: {componentId}");
        }

        if (componentRegistry.TryGetComponentLabel(
            componentId,
            out var label))
        {
            label.SetActive(true);
            activeLabels.Add(label);
        }
    }

    private void ClearCurrentStep()
    {
        highlighter.ClearHighlight();
        audioPlayer.Stop();
        foreach (var label in activeLabels)
        {
            if (label != null)
                label.SetActive(false);
        }

        activeLabels.Clear();
    }
}