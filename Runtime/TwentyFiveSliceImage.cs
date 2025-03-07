using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace TwentyFiveSlicer.Runtime {
    [RequireComponent(typeof(CanvasRenderer))]
    public class TwentyFiveSliceImage : Image {
        private struct SliceRect {
            public Vector2 Position;
            public Vector2 Size;
            public Vector2 UVMin;
            public Vector2 UVMax;
            public override string ToString() {
                return UVMin.ToString("0.000") + " " + UVMax.ToString("0.000");
            }
        }

        public bool DebuggingView {
            get => debuggingView;
            set {
                if(debuggingView != value) {
                    debuggingView = value;
                    SetVerticesDirty();
                }
            }
        }

        public Vector2 Ratio {
            get { return _ratio; }
            set {
                _ratio = value;
                SetVerticesDirty();
            }
        }

        public Direction Direction {
            get { return _direction; }
            set { _direction = value; }
        }

        public Margin Margin {
            get { return TrueMargin(); }
            set { _margin = value; }
        }

        [SerializeField] private bool debuggingView = false;
        [SerializeField] private Vector2 _ratio = Vector2.one * 0.5f;
        [SerializeField] private Margin _margin;
        [SerializeField] private Direction _direction = Direction.None;

        [SerializeField][HideInInspector] private Sprite _known;

        public Rect GetRealRect() {
            var rect = rectTransform.rect;
            var margin = TrueMargin();
            rect.yMax += margin.Top;
            rect.yMin -= margin.Bottom;
            rect.xMax += margin.Right;
            rect.xMin -= margin.Left;
            return rect;
        }

        protected override void OnPopulateMesh(VertexHelper vh) {

            if(_known != sprite) {
                _known = sprite;
                ResetRatio();
            }

            if(sprite == null) {
                base.OnPopulateMesh(vh);
                return;
            }

            if(!SliceDataManager.Instance.TryGetSliceData(sprite, out var sliceData)) {
                base.OnPopulateMesh(vh);
                return;
            }

            vh.Clear();

            Rect rect = GetPixelAdjustedRect();
            Vector4 outer = UnityEngine.Sprites.DataUtility.GetOuterUV(sprite);
            Rect spriteRect = sprite.rect;

            float[] xBordersPercent = GetXBordersPercent(sliceData);
            float[] yBordersPercent = GetYBordersPercent(sliceData);

            float[] uvXBorders = GetUVBorders(outer.x, outer.z, xBordersPercent);
            float[] uvYBorders = GetUVBorders(outer.y, outer.w, yBordersPercent);

            float[] originalWidths = GetOriginalSizes(xBordersPercent, spriteRect.width);
            float[] originalHeights = GetOriginalSizes(yBordersPercent, spriteRect.height);

            Margin margin = TrueMargin();

            switch(_direction) {
                case Direction.Deg90: {
                    (originalWidths, originalHeights) = (originalHeights, originalWidths);
                    originalWidths = originalWidths.Reverse().ToArray();
                    uvYBorders = uvYBorders.Reverse().ToArray();
                    break;
                }
                case Direction.Deg180: {
                    originalWidths = originalWidths.Reverse().ToArray();
                    originalHeights = originalHeights.Reverse().ToArray();
                    uvXBorders = uvXBorders.Reverse().ToArray();
                    uvYBorders = uvYBorders.Reverse().ToArray();
                    break;
                }
                case Direction.Deg270: {
                    (originalWidths, originalHeights) = (originalHeights, originalWidths);
                    uvXBorders = uvXBorders.Reverse().ToArray();
                    break;
                }
            }

            float[] widths = GetAdjustedSizes(rect.width + margin.Left + margin.Right, originalWidths, _ratio.x);
            float[] heights = GetAdjustedSizes(rect.height + margin.Top + margin.Bottom, originalHeights, _ratio.y);

            float[] xPositions = GetPositions(rect.xMin - margin.Left, widths);
            float[] yPositions = GetPositions(rect.yMin - margin.Bottom, heights);

            SliceRect[,] slices = GetSlices(xPositions, yPositions, uvXBorders, uvYBorders, widths, heights);

            DrawSlices(vh, slices);
        }

        private Margin TrueMargin() {
            switch(_direction) {
                case Direction.Deg90:
                    return new Margin() {
                        Top = _margin.Right,
                        Bottom = _margin.Left,
                        Right = _margin.Bottom,
                        Left = _margin.Top
                    };
                case Direction.Deg180:
                    return new Margin() {
                        Top = _margin.Bottom,
                        Bottom = _margin.Top,
                        Right = _margin.Left,
                        Left = _margin.Right
                    };
                case Direction.Deg270:
                    return new Margin() {
                        Top = _margin.Left,
                        Bottom = _margin.Right,
                        Right = _margin.Top,
                        Left = _margin.Bottom
                    };
                default: return _margin;
            }
        }

        public void SetTraget(Vector3 targetInWorldSpace, TargetAxis axis = TargetAxis.Both) {

            if(!SliceDataManager.Instance.TryGetSliceData(sprite, out var sliceData)) {
                return;
            }

            Rect spriteRect = sprite.rect;
            Rect rect = GetPixelAdjustedRect();
            var targetInLocal = PixelAdjustPoint(transform.InverseTransformPoint(targetInWorldSpace));

            var margin = TrueMargin();

            if((axis & TargetAxis.X) > 0) {

                float[] originalWidths;
                switch(_direction) {
                    case Direction.Deg90:
                        originalWidths = GetOriginalSizes(GetYBordersPercent(sliceData), spriteRect.height);
                        originalWidths = originalWidths.Reverse().ToArray();
                        break;
                    case Direction.Deg180:
                        originalWidths = GetOriginalSizes(GetXBordersPercent(sliceData), spriteRect.width);
                        originalWidths = originalWidths.Reverse().ToArray();
                        break;
                    case Direction.Deg270:
                        originalWidths = GetOriginalSizes(GetYBordersPercent(sliceData), spriteRect.height);
                        break;
                    default:
                        originalWidths = GetOriginalSizes(GetXBordersPercent(sliceData), spriteRect.width);
                        break;
                }

                var width = rect.width + margin.Left + margin.Right;

                var variableWidth = width - originalWidths[0] - originalWidths[2] - originalWidths[4];

                var rTrans = transform as RectTransform;
                var x = (targetInLocal.x + (width * rTrans.pivot.x) - originalWidths[0] - (originalWidths[2] / 2)) / variableWidth;

                _ratio = new Vector2(x, _ratio.y);
            }

            if((axis & TargetAxis.Y) > 0) {

                float[] originalHeights;
                switch(_direction) {
                    case Direction.Deg90:
                        originalHeights = GetOriginalSizes(GetXBordersPercent(sliceData), spriteRect.width);
                        break;
                    case Direction.Deg180:
                        originalHeights = GetOriginalSizes(GetYBordersPercent(sliceData), spriteRect.height);
                        originalHeights = originalHeights.Reverse().ToArray();
                        break;
                    case Direction.Deg270:
                        originalHeights = GetOriginalSizes(GetXBordersPercent(sliceData), spriteRect.width);
                        break;
                    default:
                        originalHeights = GetOriginalSizes(GetYBordersPercent(sliceData), spriteRect.height);
                        break;
                }

                var height = rect.height + margin.Top + margin.Bottom;

                var variableHeight = height - originalHeights[0] - originalHeights[2] - originalHeights[4];

                var rTrans = transform as RectTransform;
                var y = (targetInLocal.y + (height * rTrans.pivot.y) - originalHeights[0] - (originalHeights[2] / 2)) / variableHeight;

                _ratio = new Vector2(_ratio.x, y);
            }

            SetVerticesDirty();
        }

        public void ResetRatio() {

            if(!SliceDataManager.Instance.TryGetSliceData(sprite, out var sliceData)) {
                return;
            }

            float[] xBordersPercent = GetXBordersPercent(sliceData);
            float[] yBordersPercent = GetYBordersPercent(sliceData);

            float[] widths = GetOriginalSizes(xBordersPercent, sprite.rect.width);
            float[] heights = GetOriginalSizes(yBordersPercent, sprite.rect.height);

            _ratio = new Vector2(widths[1] / (widths[1] + widths[3]), heights[1] / (heights[1] + heights[3]));
        }

        private float[] GetXBordersPercent(TwentyFiveSliceData sliceData) {
            return new float[] {
                0f, sliceData.xBorders[0], sliceData.xBorders[1], sliceData.xBorders[2],
                sliceData.xBorders[3], 1
            };
        }

        private float[] GetYBordersPercent(TwentyFiveSliceData sliceData) {
            return new float[] {
                0f, 1f - sliceData.yBorders[3], 1f - sliceData.yBorders[2],
                1f - sliceData.yBorders[1], 1f - sliceData.yBorders[0], 1f
            };
        }

        private float[] GetUVBorders(float min, float max, float[] bordersPercent) {
            return new float[] {
                min, Mathf.Lerp(min, max, bordersPercent[1]), Mathf.Lerp(min, max, bordersPercent[2]),
                Mathf.Lerp(min, max, bordersPercent[3]), Mathf.Lerp(min, max, bordersPercent[4]), max
            };
        }

        private float[] GetOriginalSizes(float[] bordersPercent, float totalSize) {
            return new float[] {
                (bordersPercent[1] - bordersPercent[0]) * totalSize / pixelsPerUnitMultiplier,
                (bordersPercent[2] - bordersPercent[1]) * totalSize / pixelsPerUnitMultiplier,
                (bordersPercent[3] - bordersPercent[2]) * totalSize / pixelsPerUnitMultiplier,
                (bordersPercent[4] - bordersPercent[3]) * totalSize / pixelsPerUnitMultiplier,
                (bordersPercent[5] - bordersPercent[4]) * totalSize / pixelsPerUnitMultiplier
            };
        }

        private float[] GetAdjustedSizes(float totalSize, float[] originalSizes, float ratio) {

            float totalFixedSize = originalSizes[0] + originalSizes[2] + originalSizes[4];

            float totalStretchableSize = Mathf.Max(0, totalSize - totalFixedSize);
            float[] adjustedSizes; // = new float[5];

            // If total size is less than total fixed size, scale down the fixed sizes
            if(totalSize < totalFixedSize) {
                float scaleRatio = totalSize / totalFixedSize;
                adjustedSizes = new[] {
                    originalSizes[0] * scaleRatio,
                    0,
                    originalSizes[2] * scaleRatio,
                    0,
                    originalSizes[4] * scaleRatio
                };
            }
            // Otherwise, distribute the remaining size proportionally to the stretchable sizes
            else {
                ratio = Mathf.Clamp01(ratio);
                adjustedSizes = new[] {
                    originalSizes[0],
                    totalStretchableSize * ratio,
                    originalSizes[2],
                    totalStretchableSize * (1 - ratio),
                    originalSizes[4]
                };
            }

            return adjustedSizes;
        }

        private float[] GetPositions(float start, float[] sizes) {
            float[] positions = new float[6];
            positions[0] = start;
            for(int i = 1; i < 6; i++) {
                positions[i] = positions[i - 1] + sizes[i - 1];
            }

            return positions;
        }

        private SliceRect[,] GetSlices(float[] xPositions, float[] yPositions, float[] uvXBorders, float[] uvYBorders,
            float[] widths, float[] heights) {
            SliceRect[,] slices = new SliceRect[5, 5];
            for(int y = 0; y < 5; y++) {
                for(int x = 0; x < 5; x++) {


                    slices[x, y] = new SliceRect {
                        Position = new Vector2(xPositions[x], yPositions[y]),
                        Size = new Vector2(widths[x], heights[y]),
                    };

                    if(_direction is Direction.Deg90) {
                        slices[x, y].UVMin = new Vector2(uvXBorders[y + 1], uvYBorders[x]);
                        slices[x, y].UVMax = new Vector2(uvXBorders[y], uvYBorders[x + 1]);
                    } else if(_direction is Direction.Deg270) {
                        slices[x, y].UVMin = new Vector2(uvXBorders[y], uvYBorders[x + 1]);
                        slices[x, y].UVMax = new Vector2(uvXBorders[y + 1], uvYBorders[x]);
                    } else {
                        slices[x, y].UVMin = new Vector2(uvXBorders[x], uvYBorders[y]);
                        slices[x, y].UVMax = new Vector2(uvXBorders[x + 1], uvYBorders[y + 1]);
                    }
                }
            }

            return slices;
        }

        private void DrawSlices(VertexHelper vh, SliceRect[,] slices) {
            for(int y = 0; y < 5; y++) {
                for(int x = 0; x < 5; x++) {
                    var slice = slices[x, y];
                    Color sliceColor = DebuggingView ? new Color((float)x / 4, (float)y / 4, (float)(x + y) / 8) : color;
                    if(slice.Size.x > 0 && slice.Size.y > 0) {
                        switch(_direction) {
                            case Direction.Deg90:
                                AddQuad270(vh, slice.Position, slice.Position + slice.Size, slice.UVMin, slice.UVMax, sliceColor);
                                break;
                            case Direction.Deg270:
                                AddQuad90(vh, slice.Position, slice.Position + slice.Size, slice.UVMin, slice.UVMax, sliceColor);
                                break;
                            default:
                                AddQuad(vh, slice.Position, slice.Position + slice.Size, slice.UVMin, slice.UVMax, sliceColor);
                                break;
                        }
                    }
                }
            }
        }

        private void AddQuad(VertexHelper vh, Vector2 bottomLeft, Vector2 topRight, Vector2 uvBottomLeft,
            Vector2 uvTopRight, Color color) {
            int vertexIndex = vh.currentVertCount;
            vh.AddVert(new Vector3(bottomLeft.x, bottomLeft.y), color, new Vector2(uvBottomLeft.x, uvBottomLeft.y)); // 0
            vh.AddVert(new Vector3(bottomLeft.x, topRight.y), color, new Vector2(uvBottomLeft.x, uvTopRight.y)); // 1
            vh.AddVert(new Vector3(topRight.x, topRight.y), color, new Vector2(uvTopRight.x, uvTopRight.y)); // 2
            vh.AddVert(new Vector3(topRight.x, bottomLeft.y), color, new Vector2(uvTopRight.x, uvBottomLeft.y)); // 3
            vh.AddTriangle(vertexIndex, vertexIndex + 1, vertexIndex + 2);
            vh.AddTriangle(vertexIndex, vertexIndex + 2, vertexIndex + 3);
        }

        private void AddQuad90(VertexHelper vh, Vector2 bottomLeft, Vector2 topRight, Vector2 uvBottomLeft, Vector2 uvTopRight, Color color) {
            int vertexIndex = vh.currentVertCount;
            vh.AddVert(new Vector3(bottomLeft.x, bottomLeft.y), color, new Vector2(uvBottomLeft.x, uvTopRight.y)); // 0
            vh.AddVert(new Vector3(bottomLeft.x, topRight.y), color, new Vector2(uvTopRight.x, uvTopRight.y)); // 1
            vh.AddVert(new Vector3(topRight.x, topRight.y), color, new Vector2(uvTopRight.x, uvBottomLeft.y)); // 2
            vh.AddVert(new Vector3(topRight.x, bottomLeft.y), color, new Vector2(uvBottomLeft.x, uvBottomLeft.y)); // 3
            vh.AddTriangle(vertexIndex, vertexIndex + 1, vertexIndex + 2);
            vh.AddTriangle(vertexIndex, vertexIndex + 2, vertexIndex + 3);
        }

        private void AddQuad270(VertexHelper vh, Vector2 bottomLeft, Vector2 topRight, Vector2 uvBottomLeft, Vector2 uvTopRight, Color color) {
            int vertexIndex = vh.currentVertCount;
            vh.AddVert(new Vector3(bottomLeft.x, bottomLeft.y), color, new Vector2(uvTopRight.x, uvBottomLeft.y)); // 0
            vh.AddVert(new Vector3(bottomLeft.x, topRight.y), color, new Vector2(uvBottomLeft.x, uvBottomLeft.y)); // 1
            vh.AddVert(new Vector3(topRight.x, topRight.y), color, new Vector2(uvBottomLeft.x, uvTopRight.y)); // 2
            vh.AddVert(new Vector3(topRight.x, bottomLeft.y), color, new Vector2(uvTopRight.x, uvTopRight.y)); // 3
            vh.AddTriangle(vertexIndex, vertexIndex + 1, vertexIndex + 2);
            vh.AddTriangle(vertexIndex, vertexIndex + 2, vertexIndex + 3);
        }
    }
}