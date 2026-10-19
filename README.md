# AR Industrial Maintenance & Training Assistant

An enterprise-grade, data-driven Augmented Reality (AR) application built with Unity and AR Foundation for industrial maintenance training, equipment inspection, and step-by-step procedural workflows.

Developed as a technical assessment demonstrating robust software architecture, data-driven procedural state management, AR component interaction, local persistence, and mobile performance optimization.

---

## 🎯 Key Functional Features

1. **Equipment Visualization & Upright Placement**
   - Automatically detects horizontal workshop surfaces using AR Foundation plane detection.
   - Places the 3D industrial CAD assembly (`Centrifugal Pump`) oriented vertically upright on its base plate (`Euler(-90, 0, 0)` plane-relative compensation).
   - Automatically disables AR plane rendering and subsystem tracking post-placement to conserve battery and GPU resources.

2. **Interactive Component Selection & Identification**
   - Direct AR Touch Raycasting: Operators can tap directly on physical 3D parts (Electric Motor, Shaft Coupling, Pump Casing) in augmented space.
   - Dynamic collider synthesis at runtime enables accurate raycast selection on imported CAD meshes without altering source assets.
   - Inspects component name, description, maintenance tips, and safety cautions directly retrieved from JSON.

3. **Data-Driven Procedure Engine**
   - Workflows are dynamically loaded from JSON rather than hardcoded in C#.
   - Implements an interface-segregated state machine (`IProcedureEngine`) supporting forward/backward step sequencing, direct jump, and completion evaluation.
   - Dispatches decoupled C# events (`OnStepChanged`, `OnProcedureStarted`, `OnProcedureCompleted`) to synchronize UI and AR visuals.

4. **Component Highlighting & Annotation System**
   - Synchronized component highlighting via Universal Render Pipeline (URP) outline pass (`Linework Lite`).
   - Automatically retags all recursive sub-mesh nodes to the `highlight` layer (layer 6) when active and restores them cleanly to `Default`.
   - Spatially anchored 3D world-space annotation callouts (`TextMeshProUGUI`) orienting toward the operator.

5. **Inspection Checklist with Local Persistence**
   - Data-driven inspection checklist populated dynamically from JSON metadata.
   - Local state persistence using JSON serialization in `Application.persistentDataPath/ChecklistState.json`.
   - **Fault-Tolerant Atomic Writes**: Employs a `.tmp` swap pattern to prevent file corruption during mobile battery cutoff or OS suspension.
   - Live completion progress counter (e.g., `Inspection Checklist (5/6)`).

6. **Audio Narration & Operator Voiceover**
   - Step-synchronized audio guidance triggered via `AudioPlayer` for hands-free industrial training.

7. **Automated Unit Test Suite**
   - 8 comprehensive EditMode unit tests (`ProcedureEngineTests.cs`) covering procedure sequencing, boundary conditions, invalid IDs, checklist association, and multi-procedure extensibility.

---

## 🏗️ Technical Specifications & Prerequisites

| Component | Specification |
| :--- | :--- |
| **Unity Version** | `6000.3.3f1` (Compatible with `2023.2.22f1+`) |
| **Render Pipeline** | Universal Render Pipeline (URP) |
| **AR Framework** | AR Foundation `5.1.5` + ARCore Extensions |
| **Target Platform** | Android (ARM64, Android API Level 26 - 34) |
| **Scripting Backend** | IL2CPP |
| **Testing** | Unity Test Framework (NUnit EditMode) |

---

## 🚀 Quick Start & Setup Instructions

### 1. Clone the Repository
```bash
git clone https://github.com/julianryder95/TaskProject.git
cd TaskProject
```

### 2. Open in Unity
1. Launch **Unity Hub**.
2. Click **Add** -> **Add project from disk**.
3. Select the `TaskProject` directory.
4. Open with Unity `6000.3.3f1` (or compatible Unity 6 / 2023.2+).
5. Open the main scene: `Assets/Application/Scenes/AR_Interaction.unity`.

### 3. Editor Testing (XR Simulation)
The project includes Unity XR Simulation settings:
1. Press **Play** in the Unity Editor.
2. Use `W/A/S/D` and Right-Click + Mouse to navigate the simulated room.
3. Left-Click on a simulated tabletop/floor plane to spawn the pump vertically upright.
4. Walk through the inspection workflow using the **Next** and **Previous** buttons.
5. Tap on pump components to view component inspection details.
6. Open the Checklist panel to verify and persist checklist items.

### 4. Running Unit Tests
1. In the Unity Editor menu, navigate to **Window** > **General** > **Test Runner**.
2. Select the **EditMode** tab.
3. Click **Run All** to execute the test suite in `ProcedureEngineTests`.

### 5. Building the Android APK
You can build the APK directly through the automated build script or Unity Editor:

#### Option A: Command Line / Headless Build
```powershell
& "C:\Program Files\Unity\Hub\Editor\6000.3.3f1\Editor\Unity.exe" `
  -quit -batchmode -buildTarget Android `
  -projectPath . `
  -executeMethod BuildScript.PerformAndroidBuild `
  -logFile build.log
```
The output APK will be generated at `Builds/Android/TaskProject.apk`.

#### Option B: Unity Editor
1. In the top menu, go to **File** > **Build Settings...**
2. Ensure Platform is set to **Android**.
3. Ensure `Assets/Application/Scenes/AR_Interaction.unity` is checked.
4. Click **Build** and choose your output destination.

---

## 📂 Extensible JSON Architecture

All equipment specifications, procedures, and checklists are located in `Assets/StreamingAssets/`:
- `PayloadSteps.json` — Default Centrifugal Pump payload.
- `MultiEquipmentSample.json` — Sample demonstrating an Industrial Air Compressor with multiple procedures and custom safety checklists.

### JSON Schema Breakdown
```json
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
          "checklistItemIds": [ "check_ppe", "check_motor_housing" ],
          "warning": "Safety warning for this step..."
        }
      ]
    }
  ],
  "checklistItems": [
    {
      "id": "check_ppe",
      "description": "Wear Personal Protective Equipment",
      "isRequired": true,
      "category": "Safety"
    }
  ]
}
```

### Adding New Equipment or Procedures
To deploy a new machine or maintenance procedure:
1. Create or edit a JSON file in `Assets/StreamingAssets/` following the schema above.
2. In `JsonDataLoader` on the `AppManager` GameObject, update `fileName` to your new JSON file (or invoke `JsonDataLoader.LoadFile("NewPayload.json")` at runtime).
3. **No C# code modification is required.** The procedure engine, UI cards, audio mapper, and checklist will automatically adapt.

---

## ⚡ Performance Optimizations Implemented

1. **AR Plane Tracking Suppression**: Once the machine is placed on a detected surface, `ARObjectSpawner.HideExistingPlanes()` disables the `ARPlaneManager` component and hides all tracked plane meshes, halting CPU/GPU plane boundary recalculations.
2. **Fixed 60 FPS Target**: `Application.targetFrameRate = 60` locks frame pacing, preventing thermal throttling and camera jitter on mobile devices.
3. **Zero-Allocation Touch Raycasting**: Reuses a static `List<ARRaycastHit> hits` buffer for plane detection, eliminating per-frame heap allocations and garbage collection spikes.
4. **Recursive Layer Masking**: Component highlighting modifies GameObject layers directly rather than cloning materials, maintaining SRP batching efficiency and low draw call counts.
5. **Atomic Local I/O**: File persistence writes to a `.tmp` file before renaming, preventing file corruption on Android lifecycle interruptions.
