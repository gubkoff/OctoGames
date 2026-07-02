using UnityEngine;

namespace Samples
{
    public class Character : MonoBehaviour
    {
        public float Value { get; private set; }

        public void SetValue(float value) => Value = value;
    }
}
