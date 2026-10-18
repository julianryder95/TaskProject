
using UnityEngine;

public class ProcedureStepController : MonoBehaviour
{
    private IProcedureEngine procedureEngine;
    private IComponentRegistry componentRegistry;
    private IComponentHighlighter highlighter;
    private IAppLogger logger;

    public void Initialize(IProcedureEngine engine, IComponentRegistry registry, IComponentHighlighter componentHighlighter, IAppLogger appLogger)
    {
        procedureEngine = engine;
        componentRegistry = registry;
        highlighter = componentHighlighter;
        logger = appLogger;

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
        highlighter.ClearHighlight();

        var step = procedureEngine.CurrentStep;

        if (step == null)
            return;

        if (componentRegistry.TryGetComponentObject(
            step.componentId, out GameObject component))
        {
            highlighter.Highlight(component);
        }
        else
        {
            logger.LogError($"Component mapping not found: {step.componentId}");
        }
    }
}
