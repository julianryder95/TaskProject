using System.Linq;

public class ProcedureEngine : IProcedureEngine
{
    private MaintenanceData maintenanceData;
    private int currentStepIndex = -1;

    public ProcedureData CurrentProcedure { get; private set; }

    public ProcedureStepData CurrentStep
    {
        get
        {
            if (CurrentProcedure == null ||
                currentStepIndex >= CurrentProcedure.steps.Count)
            {
                return null;
            }

            return CurrentProcedure.steps[currentStepIndex];
        }
    }

    public bool IsCompleted { get; private set; }

    public ProcedureEngine(MaintenanceData data)
    {
        maintenanceData = data;
    }

    public bool StartProcedure(string procedureId)
    {
        var procedure = maintenanceData?.procedures?
            .FirstOrDefault(p => p.id == procedureId);

        if (procedure?.steps == null || procedure.steps.Count == 0)
            return false;

        CurrentProcedure = procedure;
        currentStepIndex = 0;
        IsCompleted = false;

        return true;
    }

    public bool MoveNext()
    {
        if (CurrentProcedure == null || IsCompleted)
            return false;

        if (currentStepIndex < CurrentProcedure.steps.Count - 1)
        {
            currentStepIndex++;
        }
        else
        {
            currentStepIndex = CurrentProcedure.steps.Count;
            IsCompleted = true;
        }

        return true;
    }

    public bool MovePrevious()
    {
        if (CurrentProcedure == null || currentStepIndex <= 0)
            return false;

        currentStepIndex--;
        IsCompleted = false;

        return true;
    }
}