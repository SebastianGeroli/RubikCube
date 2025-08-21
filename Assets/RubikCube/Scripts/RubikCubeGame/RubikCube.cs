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
        
        [SerializeField] Pivot[] _pivots;
        [SerializeField] PivotGroup[] _faces;
        [SerializeField] PivotGroup[] _columns;
        [SerializeField] PivotGroup[] _rows;
        [SerializeField] PivotGroup[] _auxColumns;
        
        List<RotationCommand> _commandsHistory = new();
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
        
        public bool TryExecuteCommand(RotationCommand command)
        {
            if (IsMoving())
            {
                Debug.LogWarning("Cube is currently moving, cannot execute command.");
                return false;
            }
            command.Execute();
            _commandsHistory.Add(command);
            return true;
        }

        void Rotate(RotationType rotationType, int index,bool forward)
        {
            if (IsMoving())
            {
                Debug.LogWarning("Cube is currently moving, cannot rotate.");
                return;
            }
            var pivotsToRotate = GetPivots(rotationType,index);
            var rotationToAdd = GetRotationToAdd(rotationType,forward);
            foreach (var pivot in pivotsToRotate)
            {
                pivot.RotatePiece(rotationType,rotationToAdd, _rotationDuration,forward);
            }
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

        Quaternion GetRotationToAdd(RotationType rotationType,bool forward)
        {
            return rotationType switch
            {
                RotationType.Column => forward? Quaternion.AngleAxis(-90, transform.right): Quaternion.AngleAxis(90, transform.right) ,
                RotationType.Row => forward? Quaternion.AngleAxis(-90, transform.up) : Quaternion.AngleAxis(90, transform.up),
                RotationType.AuxColumn => forward? Quaternion.AngleAxis(90, transform.forward) : Quaternion.AngleAxis(-90, transform.forward),
                _ => throw new ArgumentOutOfRangeException($"RotationType: {rotationType} is not handled")
            };
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
            public RotationCommand(RubikCube rubikCube,RotationType rotationType, int index, bool forward)
            {
                _rubikCube = rubikCube;
                _rotationType = rotationType;
                _index = index;
                _forward = forward;
            }
            public void Execute()
            {
                _rubikCube.Rotate(_rotationType,_index,_forward);
            }

            public void Undo()
            {
                _rubikCube.Rotate(_rotationType,_index,!_forward);
            }
        }
    }
}