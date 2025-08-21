using System;
using System.Collections;
using System.Threading.Tasks;
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

        public void RotatePiece(RotationType rotationType, Quaternion rotationToAdd, float rotationDuration)
        {
            var rotationData = GetRotationData(rotationType);
            StartCoroutine(RotatePieceCoroutine(rotationData, rotationToAdd, rotationDuration));
        }

        RotationData GetRotationData(RotationType rotationType)
        {
            return rotationType switch
            {
                RotationType.Column => _rotationData.Column,
                RotationType.Row => _rotationData.Row,
                RotationType.AuxColumn => _rotationData.AuxColumn,
                _ => throw new ArgumentOutOfRangeException($"RotationType: {rotationType} is not handled")
            };
        }

        IEnumerator RotatePieceCoroutine(RotationData rotationData, Quaternion rotationToAdd, float rotationDuration)
        {
            _isMovingPiece = true;
            var pieceToMove = _piece;
            var elapsedTime = 0f;
            var targetPivot = rotationData.NextPivot;
            var centerPivot = rotationData.CenterPivot;
            var originalTransform = transform;
            var originalRotation = pieceToMove.transform.localRotation;
            var targetRotation = rotationToAdd * pieceToMove.transform.rotation;
            var centerToOrigin = originalTransform.position - centerPivot.transform.position;
            var centerToTarget = targetPivot.transform.position - centerPivot.transform.position;
            var magnitude = centerToOrigin.magnitude;
            var originNormalized = centerToOrigin.normalized;
            var targetNormalized = centerToTarget.normalized;
            while (elapsedTime < rotationDuration)
            {
                elapsedTime += Time.deltaTime;
                var newRotation = Quaternion.Slerp(originalRotation, targetRotation, elapsedTime / rotationDuration);
                var newPosition = Vector3.Slerp(originNormalized, targetNormalized, elapsedTime / rotationDuration);
                newPosition = centerPivot.transform.position + newPosition * magnitude;
                pieceToMove.transform.SetPositionAndRotation(newPosition, newRotation);
                yield return null;
            }
            pieceToMove.transform.SetPositionAndRotation(targetPivot.transform.position, targetRotation);
            targetPivot._piece = pieceToMove;
            _isMovingPiece = false;
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