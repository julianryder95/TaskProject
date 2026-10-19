 AR Industrial Maintenance & Training Assistant

An enterprise-grade, data-driven Augmented Reality (AR) application built with Unity and AR Foundation for industrial maintenance training, equipment inspection, and step-by-step procedural workflows.

Developed as a technical assessment demonstrating robust software architecture, data-driven procedural state management, AR component interaction, local persistence, and mobile performance optimization.


Key Functional Features :

Interactive Component Selection & Identification :
   - Direct AR Touch Raycasting: Operators can tap directly on physical 3D parts (Electric Motor, Shaft Coupling, Pump Casing) in augmented space.
   - Dynamic collider synthesis at runtime enables accurate raycast selection on imported CAD meshes without altering source assets.
   - Inspects component name, description, maintenance tips, and safety cautions directly retrieved from JSON.

Data-Driven Procedure Engine :
   - Workflows are dynamically loaded from JSON rather than hardcoded in C#.
   - Implements an interface-segregated state machine (`IProcedureEngine`) supporting forward/backward step sequencing, direct jump, and completion evaluation.
   - Dispatches decoupled C# events (`OnStepChanged`, `OnProcedureStarted`, `OnProcedureCompleted`) to synchronize UI and AR visuals.

Component Highlighting & Annotation System :
   - Synchronized component highlighting via Universal Render Pipeline (URP) outline pass (`Linework Lite`).
   - Automatically retags all recursive sub-mesh nodes to the `highlight` layer (layer 6) when active and restores them cleanly to `Default`.
   - Spatially anchored 3D world-space annotation callouts (`TextMeshProUGUI`) orienting toward the operator.

Checklist with Local Persistence :
   - Data-driven inspection checklist populated dynamically from JSON metadata.
   - Local state persistence using JSON serialization in `Application.persistentDataPath/ChecklistState.json`.

Audio Narration & Operator Voiceover :
   - Step-synchronized audio guidance triggered via `AudioPlayer` for hands-free industrial training.

Automated Unit Test Suite
  comprehensive EditMode unit tests (`ProcedureEngineTests.cs`) covering procedure sequencing, boundary conditions, invalid IDs, checklist association, and multi-procedure extensibility.

Technical Specifications & Prerequisites :

Unity Version : 2023.2.22f1
Render Pipeline : Universal Render Pipeline (URP)
AR Framework: AR Foundation `5.1.5` + ARCore Extensions
Target Platform : Android (ARM64, Android API Level 26 - 34) 
Testing - Unity Test Framework (NUnit EditMode) 

Quick Start & Setup Instructions

1. Clone the Repository

git clone https://github.com/julianryder95/TaskProject.git
cd TaskProject

Open in Unity
1. Launch Unity Hub
2. Click Add -> **Add project from disk.
3. Select the `TaskProject` directory.
4. Open with Unity  2023.2+
5. Open the main scene: `Assets/Application/Scenes/AR_Interaction.unity`.

Editor Testing (XR Simulation)
The project includes Unity XR Simulation settings:
1. Press Play in the Unity Editor.
2. Use `W/A/S/D` and Right-Click + Mouse to navigate the simulated room.
3. Left-Click on a simulated tabletop/floor plane to spawn the pump vertically upright.
4. Walk through the inspection workflow using the Next and Previous buttons.
5. Tap on pump components to view component inspection details.

Running Unit Tests
1. In the Unity Editor menu, navigate to Window> General > Test Runner
2. Select the EditMode tab.
3. Click Run All to execute the test suite in `ProcedureEngineTests`.

Building the Android APK
You can build the APK directly through the automated build script or Unity Editor:
1. In the top menu, go to File > Build Settings...
2. Ensure Platform is set to Android.
3. Ensure `Assets/Application/Scenes/AR_Interaction.unity` is checked.
4. Click Build and choose your output destination.



 Extensible JSON Architecture

All equipment specifications, procedures are located in `Assets/StreamingAssets/`:
- `PayloadSteps.json` 

JSON Schema Breakdown

{
  "schemaVersion": "1.1",
  "equipment": {
    "id": "pump_001",
    "name": "Centrifugal Pump Assembly",
    "modelNumber": "CP-450-X",
    "description": "Equipment description...",
    "components": [
      {
        "id": "motor",
        "name": "Electric Motor",
        "description": "Component description...",
        "inspectionTips": "Inspection tips...",
        "safetyNotes": "Safety precautions..."
      }
    ]
  },
  "procedures": [
    {
      "id": "step_001",
      "name": "Pump Inspection Procedure",
      "steps": [
        {
          "id": "step_01",
          "stepNumber": 1,
          "title": "Motor Inspection",
          "instruction": "Instruction details...",
          "audio": 0,
          "componentIds": [ "motor" ],
           "warning": "Safety warning for this step..."
        }
      ]
    }
  ]
 }

Performance Optimizations Implemented

1. AR Plane Tracking Suppression: Once the machine is placed on a detected surface, `ARObjectSpawner.HideExistingPlanes()` disables the `ARPlaneManager` component and hides all tracked plane meshes, halting CPU/GPU plane boundary recalculations.
2. Fixed 60 FPS Target: `Application.targetFrameRate = 60` locks frame pacing, preventing thermal throttling and camera jitter on mobile devices.
3. Zero-Allocation Touch Raycasting: Reuses a static `List<ARRaycastHit> hits` buffer for plane detection, eliminating per-frame heap allocations and garbage collection spikes.
4. Recursive Layer Masking: Component highlighting modifies GameObject layers directly rather than cloning materials, maintaining SRP batching efficiency and low draw call counts.


Future Improvements :
1. Atomic loading JSON via temp storage, to avoid sudden switch off & battery dead scenarios. 


