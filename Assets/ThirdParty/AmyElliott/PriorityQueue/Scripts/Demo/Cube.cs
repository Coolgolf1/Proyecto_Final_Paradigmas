namespace AE
{
    using UnityEngine;

    public class Cube : MonoBehaviour
    {
        public int Size = 1;

        public Cube(int size)
        {
            Size = size;
        }

        public void ResetNode()
        {
            Size = 0;
        }
    }
}