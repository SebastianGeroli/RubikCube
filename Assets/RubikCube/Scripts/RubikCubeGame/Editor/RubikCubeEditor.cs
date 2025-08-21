using UnityEditor;
using UnityEngine;

namespace RubikCubeGame.Editor
{
    [CustomEditor(typeof(RubikCube))]
    public class RubikCubeEditor : UnityEditor.Editor
    {
        RubikCube _rubikCube;
        static int s_rotationIndex = 0;
        static RotationType s_rotationType = RotationType.Column;
        void OnEnable()
        {
            _rubikCube = (RubikCube)target;
        }
       
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.LabelField("Debug Controls");
            s_rotationIndex = EditorGUILayout.IntField("Rotation Index", s_rotationIndex);
            s_rotationType = (RotationType)EditorGUILayout.EnumPopup("Rotation Type", s_rotationType);
            GUI.enabled = AllowGUI();
            if (GUILayout.Button("Rotate Cube"))
            {
                _rubikCube.Rotate(s_rotationType,s_rotationIndex);
            }
            GUI.enabled = true;
        }
        
        bool AllowGUI()
        {
            if (_rubikCube == null) return false;
            if (!Application.isPlaying) return false;
            if (_rubikCube.IsMoving()) return false;
            return true;
        }
    }
}