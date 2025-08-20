using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RubikCubeGame
{
    public class RubikCube : MonoBehaviour
    {
        [SerializeField] Pivot[] _pivots;
        [SerializeField] Face[] _faces;


        Dictionary<int, Pivot> _pivotDictionary;

        void Awake()
        {
            _pivotDictionary = new Dictionary<int, Pivot>();
            foreach (var pivot in _pivots)
            {
                if (!_pivotDictionary.TryAdd(pivot.ID, pivot))
                {
                    Debug.LogError($"Duplicate pivot ID found: {pivot.ID}");
                }
            }
        }

        IEnumerator Start()
        {
            while (true)
            {
                Rotate();
                yield return new WaitUntil(() => !IsMoving());
            }
        }

        void Rotate()
        {
            //transform.forward is for AuxColumn rotation
            //transform.up is for Row rotation
            //transform.right is for Column rotation
            int[] pivotsToRotate = {1, 2, 3, 4, 5, 6, 7, 8, 9};
            var rotationToAdd = Quaternion.AngleAxis(-90,transform.right);
            
            foreach (var pivotID in pivotsToRotate)
            {
                if (_pivotDictionary.TryGetValue(pivotID, out var pivot))
                {
                    pivot.RotatePiece(rotationToAdd);
                }
            }
        }

        bool IsMoving()
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
        class Face
        {
            [SerializeField] Pivot[] _pivots;
        }
    }
}