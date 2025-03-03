
# Twenty Five Slicer Forked

**Twenty Five Slicer** forked from the [original](https://github.com/kwan3854/TwentyFiveSlicer), and added some functionality of my own.

---

## 9-slice vs 25-slice

<p align="center">
  <img src="Documentation~/Images/9slice_VS_25slice_3.gif" alt="9-slice vs 25-slice" width="700" />
</p>

---

## Key Concept

<p align="center">
  <img src="Documentation~/Images/25slice_debugging_view.gif" alt="25-slice Debugging View" width="700" />
</p>

- **9 slices**: Non-stretchable areas.
- **6 slices**: Stretch horizontally only.
- **6 slices**: Stretch vertically only.
- **4 slices**: Stretch in both directions.

This allows for far more detailed slicing. Where traditional 9-slice images often require stacking multiple image layers to achieve complex UI shapes (e.g., speech bubbles, boxes with icons or separators at the center), a 25-slice configuration can often handle these scenarios with just a single image.

---

## Added Functionality

<p align="center">
  <img src="Documentation~/Images/added-features.gif" alt="25-slice Debugging View" width="700" />
</p>

---

## Installing the Package

### Install via Git URL

1. Open the Unity **Package Manager**.
2. Select **Add package from Git URL**.
3. Enter: `https://github.com/OppositeVector/TwentyFiveSlicer.git`
4. To install a specific version, append a version tag, for example:  
   `https://github.com/OppositeVector/TwentyFiveSlicer.git#v1.0.0`

---

## How to Use

### Create Slice Data Map (First-time Setup)

1. Navigate to the `Assets/Resources` folder. (Create it if it doesn’t exist.)
2. Right-click → **Create → TwentyFiveSlicer → SliceDataMap**

<p align="center">
  <img src="Documentation~/Images/how_to_add_25slice_datamap.png" alt="How to Add 25-slice DataMap" width="550" style="display:inline-block; margin-right:20px;" />
  <img src="Documentation~/Images/sliceDataMap.png" alt="SliceDataMap Example" width="200" style="display:inline-block;" />
</p>

---

### Editing a Sprite

1. **Open the 25-Slice Editor**
  - **Window → 2D → 25-Slice Editor**

   <p align="center">
     <img src="Documentation~/Images/how_to_open_editor.png" alt="How to Open 25-Slice Editor" width="700" />
   </p>

2. **Load Your Sprite**
  - Drag and drop your sprite into the editor or select it via the provided field.

3. **Adjust the Slices**
  - Use the sliders to define horizontal and vertical cut lines, dividing the sprite into 25 sections.
  - Borders are displayed visually for accurate adjustments.

   <p align="center">
     <img src="Documentation~/Images/editor.png" alt="25-Slice Editor" width="700" />
   </p>

4. **Save the Configuration**
  - Click **Save Borders** to store the 25-slice settings.

---

### Using the 25-Sliced Sprite

#### 1. Using with **UI (TwentyFiveSliceImage)**

This is the **UI** approach, similar to `UnityEngine.UI.Image`:
1. **Create a TwentyFiveSliceImage GameObject** or add `TwentyFiveSliceImage` to an existing **UI** element in a Canvas.
2. Assign your 25-sliced sprite to the `TwentyFiveSliceImage`.
3. Adjust the RectTransform size to see how each slice region scales or remains fixed.

<p align="center">
  <img src="Documentation~/Images/how_to_add_25slice_gameobject.png" alt="How to Add 25-Slice GameObject" width="700" />
</p>
<p align="center">
  <img src="Documentation~/Images/image_component.png" alt="How to Add 25-Slice GameObject" width="700" />
</p>

#### 2. Using with **2D Scenes (TwentyFiveSliceSpriteRenderer)**

This is the **MeshRenderer**-based approach, similar to `SpriteRenderer`:
1. You can create a **25-Sliced Sprite** in the **Hierarchy**:
  - **Right-click → 2D Object → Sprites → 25-Sliced**  
    *This will instantiate a GameObject named `25-Sliced Sprite` with `TwentyFiveSliceSpriteRenderer` attached.*
2. In the Inspector, assign your 25-sliced sprite to its `Sprite` field.
3. Adjust the **Size** property in the Inspector (instead of using `transform.localScale`) to properly stretch or preserve corners/edges as needed.
4. **Sorting Layer** and **Order in Layer** are also available, just like a normal SpriteRenderer.

<p align="center">
  <img src="Documentation~/Images/sprite_renderer_menu.png" alt="How to Add 25-Slice GameObject" width="700" />
</p>
<p align="center">
  <img src="Documentation~/Images/sprite_renderer_component.png" alt="How to Add 25-Slice GameObject" width="700" />
</p>

---

## Key Features

- Divide sprites into a 5x5 grid for highly detailed control.
- Seamlessly scale and stretch specific sprite regions.
- **UI approach** (`TwentyFiveSliceImage`) for usage in UGUI-based canvases.
- **2D Mesh approach** (`TwentyFiveSliceSpriteRenderer`) for usage in 2D scenes without UI.
- Compatible with Unity’s 2D workflow, supports Sorting Layers.
- Intuitive editor window with clear visual guidance for precise adjustments.

---

## Delete Unused Data

You can remove slice data that is no longer needed:
**Tools → Twenty Five Slicer Tools → Slice Data Cleaner**

---

For more information or contributions, visit the [repository](https://github.com/kwan3854/TwentyFiveSlicer).
