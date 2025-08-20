using System;
using System.Collections;
using UnityEngine;

namespace RubikCubeGame
{
    public class Pivot : MonoBehaviour
    {
        [SerializeField] int _id;
        [SerializeField] Piece _piece;
        [SerializeField] PivotData _rotationData;
        public int ID => _id;
        
        bool _isMovingPiece;
        public void Awake()
        {
            _piece.SetID(_id);
        }

        public bool IsMovingPiece()
        {
            return _isMovingPiece;
        }

        public void RotatePiece()
        {
            StartCoroutine(RotatePieceCoroutine());
        }

        IEnumerator RotatePieceCoroutine()
        {
            _isMovingPiece = true;
            var pieceToMove = _piece;
            var elapsedTime = 0f;
            var rotationDuration = 3f;
            var targetPivot = _rotationData.Column.NextPivot;
            var originalTransform = pieceToMove.transform;
            while (elapsedTime < rotationDuration)
            {
                elapsedTime += Time.deltaTime;
                var newPosition =  Vector3.Slerp(originalTransform.position, targetPivot.transform.position, elapsedTime / rotationDuration);
                pieceToMove.transform.position = newPosition;
                yield return null;
            }

            _isMovingPiece = false;
            targetPivot._piece = pieceToMove;
        }

        [Serializable]
        class PivotData
        {
            [SerializeField] RotationData _column;
            [SerializeField] RotationData _row;
            [SerializeField] RotationData _auxColumn;
            
            public RotationData Column => _column;
            public RotationData Row => _row;
            public RotationData AuxColumn => _auxColumn;
        }

        [Serializable]
        class RotationData
        {
            [SerializeField] Pivot _nextPivot;
            [SerializeField] Pivot _centerPivot;
            [SerializeField] Pivot _previousPivot;
            
            
            public Pivot NextPivot => _nextPivot;
            public Pivot CenterPivot => _centerPivot;
            public Pivot PreviousPivot => _previousPivot;
        }
    }
}