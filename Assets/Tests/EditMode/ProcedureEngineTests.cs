using NUnit.Framework;
using System.Collections.Generic;

public class ProcedureEngineTests
{
    private MaintenanceData CreateTestData()
    {
        return new MaintenanceData
        {
            procedures = new List<ProcedureData>
            {
                new ProcedureData
                {
                    id = "procedure_01",
                    name = "Pump Inspection",
                    steps = new List<ProcedureStepData>
                    {
                        new ProcedureStepData
                        {
                            id = "step_01",
                            instruction = "Inspect motor",
                            componentIds = new List<string> { "motor" },
                        },
                        new ProcedureStepData
                        {
                            id = "step_02",
                            instruction = "Inspect coupling",
                            componentIds = new List<string> { "coupling" },
                        }
                    }
                },
                new ProcedureData
                {
                    id = "procedure_02",
                    name = "Emergency Shutdown",
                    steps = new List<ProcedureStepData>
                    {
                        new ProcedureStepData
                        {
                            id = "step_e01",
                            instruction = "Hit emergency stop button",
                            componentIds = new List<string> { "motor" }
                        }
                    }
                }
            }
        };
    }

    [Test]
    public void StartProcedure_WithValidId_StartsAtFirstStep()
    {
        var data = CreateTestData();
        var engine = new ProcedureEngine(data);

        bool result = engine.StartProcedure("procedure_01");

        Assert.That(result, Is.True);
        Assert.That(engine.CurrentStep, Is.Not.Null);
        Assert.That(engine.CurrentStep.id, Is.EqualTo("step_01"));
        Assert.That(engine.CurrentStepIndex, Is.EqualTo(0));
        Assert.That(engine.TotalSteps, Is.EqualTo(2));
        Assert.That(engine.IsCompleted, Is.False);
    }

    [Test]
    public void StartProcedure_WithInvalidId_ReturnsFalse()
    {
        var data = CreateTestData();
        var engine = new ProcedureEngine(data);

        bool result = engine.StartProcedure("invalid_id");

        Assert.That(result, Is.False);
        Assert.That(engine.CurrentProcedure, Is.Null);
        Assert.That(engine.CurrentStep, Is.Null);
    }

    [Test]
    public void MoveNext_MovesToNextStep()
    {
        var data = CreateTestData();
        var engine = new ProcedureEngine(data);

        engine.StartProcedure("procedure_01");

        bool result = engine.MoveNext();

        Assert.That(result, Is.True);
        Assert.That(engine.CurrentStep, Is.Not.Null);
        Assert.That(engine.CurrentStep.id, Is.EqualTo("step_02"));
        Assert.That(engine.CurrentStepIndex, Is.EqualTo(1));
    }

    [Test]
    public void MoveNext_OnLastStep_CompletesProcedure()
    {
        var data = CreateTestData();
        var engine = new ProcedureEngine(data);

        engine.StartProcedure("procedure_01");
        engine.MoveNext();

        bool result = engine.MoveNext();

        Assert.That(result, Is.True);
        Assert.That(engine.IsCompleted, Is.True);
        Assert.That(engine.CurrentStep, Is.Null);
        Assert.That(engine.CanMoveNext, Is.False);
    }

    [Test]
    public void MovePrevious_ReturnsToPreviousStep()
    {
        var data = CreateTestData();
        var engine = new ProcedureEngine(data);

        engine.StartProcedure("procedure_01");
        engine.MoveNext();

        bool result = engine.MovePrevious();

        Assert.That(result, Is.True);
        Assert.That(engine.CurrentStep, Is.Not.Null);
        Assert.That(engine.CurrentStep.id, Is.EqualTo("step_01"));
        Assert.That(engine.IsCompleted, Is.False);
    }

    [Test]
    public void JumpToStep_NavigatesDirectlyToStep()
    {
        var data = CreateTestData();
        var engine = new ProcedureEngine(data);

        engine.StartProcedure("procedure_01");
        bool result = engine.JumpToStep(1);

        Assert.That(result, Is.True);
        Assert.That(engine.CurrentStepIndex, Is.EqualTo(1));
        Assert.That(engine.CurrentStep.id, Is.EqualTo("step_02"));
    }



    [Test]
    public void Extensibility_MultipleProcedures_SwitchesCleanly()
    {
        var data = CreateTestData();
        var engine = new ProcedureEngine(data);

        engine.StartProcedure("procedure_01");
        Assert.That(engine.TotalSteps, Is.EqualTo(2));

        engine.StartProcedure("procedure_02");
        Assert.That(engine.CurrentProcedure.name, Is.EqualTo("Emergency Shutdown"));
        Assert.That(engine.TotalSteps, Is.EqualTo(1));
        Assert.That(engine.CurrentStep.id, Is.EqualTo("step_e01"));
    }
}