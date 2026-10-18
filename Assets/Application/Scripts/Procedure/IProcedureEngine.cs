using System;
using System.Collections.Generic;

/// <summary>
/// Core contract for the data-driven procedure engine.
/// Decouples workflow logic, step progression, and state evaluation from UI and AR rendering.
/// </summary>
public interface IProcedureEngine
{
    ProcedureData CurrentProcedure { get; }
    ProcedureStepData CurrentStep { get; }
    int CurrentStepIndex { get; }
    int TotalSteps { get; }
    bool IsCompleted { get; }
    bool CanMoveNext { get; }
    bool CanMovePrevious { get; }

    event Action<ProcedureStepData, int, int> OnStepChanged;
    event Action<ProcedureData> OnProcedureStarted;
    event Action<ProcedureData> OnProcedureCompleted;

    bool StartProcedure(string procedureId);
    bool MoveNext();
    bool MovePrevious();
    bool JumpToStep(int stepIndex);
    void Restart();
}