using System;
using System.Collections;
using System.Collections.Generic;
using Contracts;
using UnityEngine;

namespace RubikCubeGame
{
    public class RubikCube : MonoBehaviour
    {
        [SerializeField] float _rotationDuration = 0.5f;

        [SerializeField] PivotGroup[] _faces;
        [SerializeField] PivotGroup[] _columns;
        [SerializeField] PivotGroup[] _rows;
        [SerializeField] PivotGroup[] _auxColumns;

        Pivot[] _pivots;
        List<RotationCommand> _commandsHistory = new();

        int[] _faceValues;

        void Awake()
        {
            _pivots = GetComponentsInChildren<Pivot>();
            _faceValues = new int[_faces.Length];
            for (int i = 0; i < _faces.Length; i++)
            {
                var pivotGroup = _faces[i];
                var sum = 0;
                foreach (var pivot in pivotGroup.Pivots)
                {
                    sum += pivot.ID;
                }

                _faceValues[i] = sum;
            }
        }

        public void ResetCube()
        {
            if (IsMoving())
            {
                Debug.LogWarning("Cube is currently moving, cannot reset.");
                return;
            }

            StartCoroutine(ResetCubeCoroutine());
        }

        IEnumerator ResetCubeCoroutine()
        {
            for (int i = _commandsHistory.Count - 1; i >= 0; i--)
            {
                var command = _commandsHistory[i];
                command.Undo();
                yield return new WaitUntil(() => !IsMoving());
            }

            _commandsHistory.Clear();
        }

        void Rotate(RotationType rotationType, int index, bool forward)
        {
            if (IsMoving())
            {
                Debug.LogWarning("Cube is currently moving, cannot rotate.");
                return;
            }

            var pivotsToRotate = GetPivots(rotationType, index);
            var rotationToAdd = GetRotationToAdd(rotationType, forward);
            foreach (var pivot in pivotsToRotate)
            {
                pivot.RotatePiece(rotationType, rotationToAdd, _rotationDuration, forward);
            }

            StartCoroutine(CheckSolutionCoroutine());
        }

        Pivot[] GetPivots(RotationType rotationType, int index)
        {
            return rotationType switch
            {
                RotationType.Column => _columns[index].Pivots,
                RotationType.Row => _rows[index].Pivots,
                RotationType.AuxColumn => _auxColumns[index].Pivots,
                _ => throw new ArgumentOutOfRangeException($"RotationType: {rotationType} is not handled")
            };
        }

        Quaternion GetRotationToAdd(RotationType rotationType, bool forward)
        {
            return rotationType switch
            {
                RotationType.Column => forward
                    ? Quaternion.AngleAxis(-90, transform.right)
                    : Quaternion.AngleAxis(90, transform.right),
                RotationType.Row => forward
                    ? Quaternion.AngleAxis(-90, transform.up)
                    : Quaternion.AngleAxis(90, transform.up),
                RotationType.AuxColumn => forward
                    ? Quaternion.AngleAxis(90, transform.forward)
                    : Quaternion.AngleAxis(-90, transform.forward),
                _ => throw new ArgumentOutOfRangeException($"RotationType: {rotationType} is not handled")
            };
        }

        IEnumerator CheckSolutionCoroutine()
        {
            yield return new WaitUntil(() => !IsMoving());
            if (IsSolved())
            {
                Debug.Log("Cube is solved!");
            }
        }

        bool IsSolved()
        {
            var availableSums = new List<int>(_faceValues);
            foreach (var face in _faces)
            {
                var sum = 0;
                foreach (var pivot in face.Pivots)
                {
                    sum += pivot.PieceID;
                }

                if (!availableSums.Contains(sum))
                {
                    return false;
                }

                availableSums.Remove(sum);
            }
            return true;
        }

        public bool IsMoving()
        {
            foreach (var pivot in _pivots)
            {
                if (pivot.IsMovingPiece())
                {
                    return true;
                }
            }

            return false;
        }

        [Serializable]
        class PivotGroup
        {
            [SerializeField] Pivot[] _pivots;
            public Pivot[] Pivots => _pivots;
        }

        public class RotationCommand : IUndoableCommand
        {
            RubikCube _rubikCube;
            RotationType _rotationType;
            int _index;
            bool _forward;

            public RotationCommand(RubikCube rubikCube, RotationType rotationType, int index, bool forward)
            {
                _rubikCube = rubikCube;
                _rotationType = rotationType;
                _index = index;
                _forward = forward;
            }

            public void Execute()
            {
                if (_rubikCube.IsMoving()) return;
                _rubikCube.Rotate(_rotationType, _index, _forward);
                _rubikCube._commandsHistory.Add(this);
            }

            public void Undo()
            {
                _rubikCube.Rotate(_rotationType, _index, !_forward);
            }
        }
    }
}