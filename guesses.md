# Guesses and Ambiguities

This document records guesses and ambiguous items encountered during the PDF code extraction.

## Directory Structure

The folder structure was created based on the PDF's "2.5.1 – File Structure" section. The original project is a Unity project with the following structure:

```
Assets/
├── Materials/
│   ├── OceanFloor.mat
│   └── PieceMaterials/
├── Models/
├── Prefabs/
├── Resources/
├── Scenes/
├── Scripts/
│   ├── AI/               - Classes that control or are AI
│   ├── Game/             - Scripts that manage game rules and gameplay
│   ├── Pieces/           - Piece scripts
│   ├── DataStructures/   - Custom data structures
│   ├── Input/            - User input handling (non-UI)
│   ├── Sound/            - Sound handling
│   ├── Interfaces/       - Interface/linking scripts
│   ├── UI/               - UI control scripts
│   └── Utils/            - Utility scripts
├── Sounds/
├── ScriptableObjects/
├── Fonts/
├── Images/
└── TextMeshPro/
```

## Code Organization Guesses

### Class Labels vs Section Headers

In the PDF, class names like "AI5", "AI MANAGER", "BOARD" appear as standalone lines before their `using` statements and class declarations. These were incorrectly being skipped as section headers in early extraction attempts. The final extraction correctly identifies these as class labels and includes them properly.

### Multiple Class Occurrences

Many classes appear twice in the PDF:
1. First occurrence in the analysis/technical discussion section (e.g., around line 6800-9500)
2. Second occurrence in the "ALL CODE" section (e.g., around line 13800+)

The extraction uses the second occurrence (ALL CODE section) as it contains the complete implementation.

### Class Name Anomalies

- `AI4` at line 14013 shows `public class AI3` in its header - might be a PDF copy-paste error in the original document, or the class body is actually AI3's implementation
- `CameraFreeLook` appears twice (lines 13646 and 23499) - the second one in the "UI Controllers" section was extracted

### Missing Classes

The grep found additional classes that were not initially included but were added later:
- `InputReciever` (abstract base class)
- `IObjectTweener` (interface)
- `BoardButton`
- `MaterialSetter`
- `OfficialNotation`
- `TimerHelper`
- `RandomNumber`
- `UIButton`

### Large Files Issue

Several extracted files are suspiciously large (2001 lines):
- `AIManager.cs` (2001 lines) - Contains both AIManager AND likely AI5's parent functions
- `NeuralNetwork.cs` (1493 lines) - May contain more than just NeuralNetwork

This suggests the extraction boundary detection may have merged adjacent code.

### File Names with Typos

Original document contains typos that were preserved:
- `ColliderInputReciever` (should probably be `ColliderInputReceiver`)
- `SqaureSelectorCreator` (should probably be `SquareSelectorCreator`)
- `UIInputReciever` (should probably be `UIInputReceiver`)
- `ChessUIManager` vs `Chess UI Manager` in TOC

### Line Number Accuracy

Line numbers in the PDF extraction are approximate due to:
1. PDF text extraction artifacts (line wraps, missing spaces)
2. Section headers and page numbers being included in line count
3. Some code blocks spanning multiple pages

## Code Quality Issues in Extraction

### Missing Opening Brace
Some extracted classes don't have a proper `using` block before the class declaration (e.g., `AI1.cs` starts with `using` statements but the `class AI1` declaration appears on line 4 with no blank line separator).

### Extra Blank Lines
Extracted code contains many empty lines due to the PDF layout (each line on its own line in the PDF).

### Truncated Lines
Some lines in the PDF are truncated with line continuation, but the extraction may not properly handle all cases.

## Items to Review

1. Verify `AI4.cs` actually contains AI4 or AI3 implementation
2. Check if `RandomNumber` and `UIButton` are actually separate classes or incorrectly extracted
3. Review `AIManager.cs` to see if it contains AI5's parent functions
4. Verify `NeuralNetwork.cs` doesn't contain additional classes
5. The `TimerHelper` class name conflicts with Unity's `Timer` - worth verifying actual class name