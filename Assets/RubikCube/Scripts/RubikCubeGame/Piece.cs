using UnityEngine;

namespace RubikCubeGame
{
    public class Piece : MonoBehaviour
    {
        public int ID { get; private set; }

        public void SetID(int id)
        {
            ID = id;
        }
    }
}