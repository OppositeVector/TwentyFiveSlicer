using TwentyFiveSlicer.Runtime;
using UnityEditor;
using UnityEngine;

namespace TwentyFiveSlicer.TFSEditor.Editor {
    [CustomEditor(typeof(TwentyFiveSliceImage))]
    [CanEditMultipleObjects]
    public class TwentyFiveSliceImageComponentInspector : UnityEditor.UI.ImageEditor {

        private SerializedProperty _spSprite;
        private SerializedProperty _pixesPreUnitMultiplierProp;
        private SerializedProperty _spDebuggingView;
        private SerializedProperty _ratioProp;
        private SerializedProperty _marginProp;
        private SerializedProperty _directionProp;

        private bool _shouldShowDebuggingMenus = false;

        //Help box messages
        private const string NoSliceDataWarning =
                "The selected sprite does not have 25-slice data. Please slice the sprite in Window -> 2D -> 25-Slice Editor.";

        protected override void OnEnable() {
            base.OnEnable();
            _spSprite = serializedObject.FindProperty("m_Sprite");
            _pixesPreUnitMultiplierProp = serializedObject.FindProperty("m_PixelsPerUnitMultiplier");
            _spDebuggingView = serializedObject.FindProperty("debuggingView");
            _ratioProp = serializedObject.FindProperty("_ratio");
            _marginProp = serializedObject.FindProperty("_margin");
            _directionProp = serializedObject.FindProperty("_direction");
        }
        public override void OnInspectorGUI() {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            SpriteGUI();
            AppearanceControlsGUI();
            EditorGUILayout.PropertyField(_ratioProp);
            EditorGUILayout.PropertyField(_marginProp);
            EditorGUILayout.PropertyField(_directionProp);
            RaycastControlsGUI();
            MaskableControlsGUI();

            EditorGUILayout.PropertyField(_pixesPreUnitMultiplierProp);

            NativeSizeButtonGUI();

            EditorGUILayout.Space();

            _shouldShowDebuggingMenus = EditorGUILayout.Foldout(_shouldShowDebuggingMenus, "Debugging");
            if(_shouldShowDebuggingMenus) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_spDebuggingView, new GUIContent("Debugging View"));
                EditorGUI.indentLevel--;
            }

            ShowSliceDataWarning();

            serializedObject.ApplyModifiedProperties();
        }
        private void ShowSliceDataWarning() {
            // If there's a sprite but no 25-slice data, show the warning
            Sprite spriteObj = _spSprite.objectReferenceValue as Sprite;
            if(spriteObj != null) {
                if(!SliceDataManager.Instance.TryGetSliceData(spriteObj, out _)) {
                    EditorGUILayout.HelpBox(NoSliceDataWarning, MessageType.Warning);
                }
            }
        }
    }
}