using UnityEditor;

namespace TwentyFiveSlicer.TFSEditor.Editor
{
    public class DraggingInfo
    {
        private DraggingState _state = DraggingState.None;
        private int _xIndex = -1;
        private int _yIndex = -1;
        private MouseCursor _currentDragCursor = MouseCursor.Arrow;

        public void SetDraggingState(DraggingState state, int xIndex = -1, int yIndex = -1,
            MouseCursor dragCursor = MouseCursor.Arrow)
        {
            UnityEngine.Debug.Assert(
                state != DraggingState.Intersection || (xIndex != -1 && yIndex != -1),
                "Intersection requires valid indices");
            _state = state;
            _xIndex = xIndex;
            _yIndex = yIndex;
            _currentDragCursor = dragCursor;
        }

        public (DraggingState, int, int) GetDraggingState()
        {
            return (_state, _xIndex, _yIndex);
        }

        public MouseCursor GetCurrentDragCursor()
        {
            return _currentDragCursor;
        }

        public void ClearDraggingState()
        {
            _state = DraggingState.None;
            _xIndex = -1;
            _yIndex = -1;
            _currentDragCursor = MouseCursor.Arrow;
        }

        public bool IsDragging()
        {
            return _state != DraggingState.None;
        }
    }
}