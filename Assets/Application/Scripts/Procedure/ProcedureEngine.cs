using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Data-driven procedure state machine.
/// Governs step sequencing, lifecycle events, and step-level checklist association.
/// </summary>
public class ProcedureEngine : IProcedureEngine
{
    private readonly MaintenanceData maintenanceData;
    private int currentStepIndex = -1;

    public ProcedureData CurrentProcedure { get; private set; }

    public ProcedureStepData CurrentStep
    {
        get
        {
            if (CurrentProcedure?.steps == null ||
                currentStepIndex < 0 ||
                currentStepIndex >= CurrentProcedure.steps.Count)
            {
                return null;
            }

            return CurrentProcedure.steps[currentStepIndex];
        }
    }

    public int CurrentStepIndex => currentStepIndex;

    public int TotalSteps => CurrentProcedure?.steps?.Count ?? 0;

    public bool IsCompleted { get; private set; }

    public bool CanMoveNext => CurrentProcedure != null && !IsCompleted;

    public bool CanMovePrevious => CurrentProcedure != null && currentStepIndex > 0;

    public event Action<ProcedureStepData, int, int> OnStepChanged;
    public event Action<ProcedureData> OnProcedureStarted;
    public event Action<ProcedureData> OnProcedureCompleted;

    public ProcedureEngine(MaintenanceData data)
    {
        maintenanceData = data;
    }

    public bool StartProcedure(string procedureId)
    {
        if (maintenanceData?.procedures == null)
            return false;

        var procedure = maintenanceData.procedures
            .FirstOrDefault(p => string.Equals(p.id, procedureId, StringComparison.OrdinalIgnoreCase));

        if (procedure?.steps == null || procedure.steps.Count == 0)
            return false;

        CurrentProcedure = procedure;
        currentStepIndex = 0;
        IsCompleted = false;

        OnProcedureStarted?.Invoke(CurrentProcedure);
        OnStepChanged?.Invoke(CurrentStep, currentStepIndex, TotalSteps);

        return true;
    }

    public bool MoveNext()
    {
        if (CurrentProcedure == null || IsCompleted)
            return false;

        if (currentStepIndex < CurrentProcedure.steps.Count - 1)
        {
            currentStepIndex++;
            OnStepChanged?.Invoke(CurrentStep, currentStepIndex, TotalSteps);
        }
        else
        {
            currentStepIndex = CurrentProcedure.steps.Count;
            IsCompleted = true;
            OnProcedureCompleted?.Invoke(CurrentProcedure);
            OnStepChanged?.Invoke(null, currentStepIndex, TotalSteps);
        }

        return true;
    }

    public bool MovePrevious()
    {
        if (CurrentProcedure == null || currentStepIndex <= 0)
            return false;

        currentStepIndex--;
        IsCompleted = false;

        OnStepChanged?.Invoke(CurrentStep, currentStepIndex, TotalSteps);
        return true;
    }

    public bool JumpToStep(int stepIndex)
    {
        if (CurrentProcedure?.steps == null || stepIndex < 0 || stepIndex >= CurrentProcedure.steps.Count)
            return false;

        currentStepIndex = stepIndex;
        IsCompleted = false;

        OnStepChanged?.Invoke(CurrentStep, currentStepIndex, TotalSteps);
        return true;
    }

    public void Restart()
    {
        if (CurrentProcedure != null)
        {
            StartProcedure(CurrentProcedure.id);
        }
    }


}