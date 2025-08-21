using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RubikCubeGame
{
    public class RubikCube : MonoBehaviour
    {
        [SerializeField] Pivot[] _pivots;
        [SerializeField] PivotGroup[] _faces;
        [SerializeField] PivotGroup[] _columns;
        [SerializeField] PivotGroup[] _rows;
        [SerializeField] PivotGroup[] _auxColumns;
        
        public void Rotate(RotationType rotationType, int index)
        {
            var pivotsToRotate = GetPivots(rotationType,index);
            var rotationToAdd = GetRotationToAdd(rotationType);
            foreach (var pivot in pivotsToRotate)
            {
                pivot.RotatePiece(rotationType,rotationToAdd);
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
        
        Quaternion GetRotationToAdd(RotationType rotationType)
        {
            return rotationType switch
            {
                RotationType.Column => Quaternion.AngleAxis(-90, transform.right),
                RotationType.Row => Quaternion.AngleAxis(-90, transform.up),
                RotationType.AuxColumn => Quaternion.AngleAxis(90, transform.forward),
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
    }
}