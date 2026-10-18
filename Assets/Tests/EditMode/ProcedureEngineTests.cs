using NUnit.Framework;
using System.Collections.Generic;

/*public class ProcedureEngineTests
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
                            checklistItemIds = new List<string> { "motor_check" }
                        },
                        new ProcedureStepData
                        {
                            id = "step_02",
                            instruction = "Inspect coupling",
                            componentIds = new List<string> { "coupling" },
                            checklistItemIds = new List<string> { "coupling_check" }
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
        Assert.That(engine.CurrentStep.id, Is.EqualTo("step_01"));
        Assert.That(engine.IsCompleted, Is.False);
    }

    [Test]
    public void StartProcedure_WithInvalidId_ReturnsFalse()
    {
        var data = CreateTestData();
        var engine = new ProcedureEngine(data);

        bool result = engine.StartProcedure("invalid");

        Assert.That(result, Is.False);
        Assert.That(engine.CurrentProcedure, Is.Null);
    }

    [Test]
    public void MoveNext_MovesToNextStep()
    {
        var data = CreateTestData();
        var engine = new ProcedureEngine(data);

        engine.StartProcedure("procedure_01");

        bool result = engine.MoveNext();

        Assert.That(result, Is.True);
        Assert.That(engine.CurrentStep.id, Is.EqualTo("step_02"));
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
        Assert.That(engine.CurrentStep.id, Is.EqualTo("step_01"));
        Assert.That(engine.IsCompleted, Is.False);
    }
}*/