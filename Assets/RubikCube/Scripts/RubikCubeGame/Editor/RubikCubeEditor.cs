using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RubikCubeGame.Editor
{
    [CustomEditor(typeof(RubikCube))]
    public class RubikCubeEditor : UnityEditor.Editor
    {
        RubikCube _rubikCube;
        static int s_rotationIndex = 0;
        static int s_shuffleCount = 10;
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
            s_shuffleCount = EditorGUILayout.IntField("Shuffle Count", s_shuffleCount);
            GUI.enabled = AllowGUI();
            if (GUILayout.Button("Rotate Cube"))
            {
                _rubikCube.Rotate(s_rotationType,s_rotationIndex);
            }
            if (GUILayout.Button("Shuffle Cube"))
            {
                ShuffleCubeAsync();
            }
            
            if (GUILayout.Button("Reset Cube"))
            {
                _rubikCube.ResetCube();
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
        
        async Task ShuffleCubeAsync()
        {
            if (_rubikCube == null) return;
            if(_rubikCube.IsMoving()) return;
            for (int i = 0; i < s_shuffleCount; i++)
            {
                var randomRotationType = (RotationType)Random.Range(0, Enum.GetValues(typeof(RotationType)).Length);
                var randomIndex = Random.Range(0, 2);
                _rubikCube.Rotate(randomRotationType, randomIndex);
                await WaitForCubeToStop();
            }
        }

        async Task WaitForCubeToStop()
        {
            if (_rubikCube == null) return;
            while (_rubikCube.IsMoving())
            {
                await Task.Yield();
            }
        }

    }
}