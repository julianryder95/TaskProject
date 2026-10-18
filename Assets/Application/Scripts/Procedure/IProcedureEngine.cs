public interface IProcedureEngine
{
    ProcedureData CurrentProcedure { get; }
    ProcedureStepData CurrentStep { get; }
    bool IsCompleted { get; }

    bool StartProcedure(string procedureId);
    bool MoveNext();
    bool MovePrevious();
}