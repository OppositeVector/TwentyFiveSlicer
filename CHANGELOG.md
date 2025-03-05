# Changelog

## [1.3.3] - 2025-03-05
### Changes
- Added save changes to the editor window, so that changes are not automatically lost when exiting the boarder editor.

## [1.3.2] - 2025-03-05
### Changes
- Removed the initial setup step, as it happens automatically now.

## [1.3.1] - 2025-03-04
### Changes
- Changed the parameter to the SetTarget function, didnt need the Transform, only its position in world space, so i changed it to Vector3.

## [1.3.0] - 2025-03-04
### Changes
- Fixed SpriteRenderer established features, changed its editor to be more in line with unity's editor.
- Added Ratio feature to the SpriteRenderer.

## [1.2.0] - 2025-03-03
### Changes
- Original release after forking from [TwentyFiveSlicer](https://github.com/kwan3854/TwentyFiveSlicer).
- Changed editor functionality to more closely resemble unity's sprite editor, now its working with pixels instead of arbitaty %.
- Added Ratio field to the Image Renderer to control the sprite better.
- Added PixelsPerUnitMultiplier functionality back to Image Renderer.